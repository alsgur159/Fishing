using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class FishingMinigame : MonoBehaviour
{
    [Header("Current Fish")]
    public FishData currentFish;

    [Header("Minigame Settings")]
    public float baseCatchMultiplier = 15f; // 기본 포획 속도
    public float loseMultiplier = 15f;

    [Header("Chest Settings")]
    public float chestSpawnChance = 0.3f; // 30% 확률로 등장
    public float chestCaptureTime = 1.5f; // 상자를 획득하기 위해 머물러야 하는 시간
    public RectTransform chestUI;         // 상자 아이콘 UI
    public Image chestFillImage;          // 상자 획득 진행도를 보여줄 이미지 (Filled 타입)

    [Header("PowerUp Settings")]
    public float powerUpDuration = 3f;    // 파워업 지속 시간
    public float powerUpSizeMultiplier = 1.5f; // 파워업 시 바 크기 증가 비율

    [Header("UI Elements")]
    public Slider progressBar;
    public RectTransform backgroundBar;
    public RectTransform playerBarUI;
    public RectTransform fishUI;

    [Header("Input Settings")]
    public InputAction reelAction;

    // 로직 변수
    private float fishPosition = 0.5f;
    private float fishDestination = 0.5f;
    private float fishTimer = 0f;

    private float playerPosition = 0f;
    private float playerVelocity = 0f;
    private float playerGravity = -1.5f;
    private float playerPower = 3f;

    private float catchProgress = 0f;
    private bool isFishing = false;

    // 상자 및 파워업 관련 변수
    private bool isChestActive = false;
    private float chestPosition = 0f;
    private float currentChestProgress = 0f;
    private bool isPowerUpActive = false;

    private void OnEnable() { reelAction.Enable(); }
    private void OnDisable() { reelAction.Disable(); }

    public void StartFishing(FishData fish)
    {
        currentFish = fish;
        catchProgress = 30f;
        fishPosition = 0.5f;
        playerPosition = 0f;
        isFishing = true;

        // [수정] 여기서 isPowerUpActive = false; 를 지웠습니다! 
        // 그래야 이전 물고기 때 먹은 파워업이 다음 물고기까지 유지됩니다.

        if (Random.value <= chestSpawnChance)
        {
            isChestActive = true;
            chestPosition = Random.Range(0.1f, 0.9f);
            currentChestProgress = 0f;
            chestUI.gameObject.SetActive(true);
        }
        else
        {
            isChestActive = false;
            chestUI.gameObject.SetActive(false);
        }
    }

    // 핵심: 현재 플레이어 바의 크기를 실시간으로 계산하는 함수 (유물 효과 + 일시적 파워업 효과 통합)
    private float GetCurrentZoneSize()
    {
        float size = currentFish.catchZoneSize + GameManager.Instance.bonusBarSize;
        if (isPowerUpActive) size *= powerUpSizeMultiplier;
        return size;
    }

    void Update()
    {
        if (!isFishing) return;

        UpdatePlayerBar();
        UpdateFishAI();
        CheckCollisions(); // 물고기 및 상자 판정 통합
        UpdateVisuals();
    }

    private void UpdatePlayerBar()
    {
        if (reelAction.IsPressed()) playerVelocity += playerPower * Time.deltaTime;
        else playerVelocity += playerGravity * Time.deltaTime;

        playerPosition += playerVelocity * Time.deltaTime;

        // 실시간으로 변하는 바 크기를 반영하여 최댓값 계산 (화면 뚫림 방지)
        float currentZoneSize = GetCurrentZoneSize();
        float maxPlayerPosition = 1f - currentZoneSize;

        if (playerPosition <= 0f)
        {
            playerPosition = 0f;
            playerVelocity = 0f;
        }
        else if (playerPosition >= maxPlayerPosition)
        {
            playerPosition = maxPlayerPosition;
            playerVelocity = 0f;
        }
    }

    private void UpdateFishAI()
    {
        fishTimer -= Time.deltaTime;
        if (fishTimer <= 0)
        {
            fishTimer = currentFish.directionChangeTimer * Random.Range(0.8f, 1.2f);
            fishDestination = Random.Range(0f, 1f);
        }
        fishPosition = Mathf.MoveTowards(fishPosition, fishDestination, currentFish.fishSpeed * Time.deltaTime);
    }

    private void CheckCollisions()
    {
        float currentZoneSize = GetCurrentZoneSize();

        // 1. 물고기 포획 판정 (유물로 인한 스피드 보너스 적용)
        bool isCaught = (fishPosition >= playerPosition) && (fishPosition <= playerPosition + currentZoneSize);
        float actualCatchMultiplier = baseCatchMultiplier + GameManager.Instance.bonusCatchSpeed;

        if (isCaught) catchProgress += actualCatchMultiplier * Time.deltaTime;
        else catchProgress -= loseMultiplier * Time.deltaTime;

        catchProgress = Mathf.Clamp(catchProgress, 0f, 100f);

        // 2. 상자 획득 판정
        if (isChestActive)
        {
            bool isChestCaught = (chestPosition >= playerPosition) && (chestPosition <= playerPosition + currentZoneSize);

            if (isChestCaught)
            {
                currentChestProgress += Time.deltaTime;
                if (currentChestProgress >= chestCaptureTime)
                {
                    CatchChest();
                }
            }
            else
            {
                // 상자에서 벗어나면 진행도가 서서히 감소 (스타듀밸리 스타일)
                currentChestProgress = Mathf.Max(0f, currentChestProgress - Time.deltaTime);
            }
        }

        // 결과 판정
        if (catchProgress >= 100f)
        {
            isFishing = false;
            GameManager.Instance.AddScore(currentFish.score);
        }
        else if (catchProgress <= 0f)
        {
            isFishing = false;
            GameManager.Instance.FailFishing();
        }
    }

    private void CatchChest()
    {
        isChestActive = false;
        chestUI.gameObject.SetActive(false); // 획득 시 상자 숨김

        // 1. 파워업 아이템 발동 (일시적)
        StartCoroutine(PowerUpRoutine());

        // 2. 유물 획득 (영구적 강화)
        GameManager.Instance.OpenChestReward();
    }

    private IEnumerator PowerUpRoutine()
    {
        isPowerUpActive = true;
        Debug.Log("파워업 시작! " + powerUpDuration + "초간 유지됩니다.");

        yield return new WaitForSeconds(powerUpDuration);

        isPowerUpActive = false;
        Debug.Log("파워업 종료!");
    }

    private void UpdateVisuals()
    {
        progressBar.value = catchProgress / 100f;

        float currentZoneSize = GetCurrentZoneSize();

        // 플레이어 바 크기 동기화 (파워업, 유물 효과가 실시간으로 반영됨)
        float calculatedHeight = backgroundBar.rect.height * currentZoneSize;
        playerBarUI.sizeDelta = new Vector2(playerBarUI.sizeDelta.x, calculatedHeight);

        // UI 위치 이동
        playerBarUI.anchoredPosition = new Vector2(playerBarUI.anchoredPosition.x, playerPosition * backgroundBar.rect.height);

        float maxFishY = backgroundBar.rect.height - fishUI.rect.height;
        fishUI.anchoredPosition = new Vector2(fishUI.anchoredPosition.x, fishPosition * maxFishY);

        if (isChestActive)
        {
            // 상자 위치
            float maxChestY = backgroundBar.rect.height - chestUI.rect.height;
            chestUI.anchoredPosition = new Vector2(chestUI.anchoredPosition.x, chestPosition * maxChestY);

            // 상자 획득 게이지 UI (있을 경우)
            if (chestFillImage != null)
            {
                chestFillImage.fillAmount = currentChestProgress / chestCaptureTime;
            }
        }
    }
    // [수정] 파워업 로직: 물고기 스폰과 상관없이 별도의 타이머로 작동합니다.
    public void TriggerInstantPowerUp()
    {
        // 이미 파워업 중이라면 코루틴을 새로 시작하여 시간만 초기화하거나 중첩시킬 수 있습니다.
        // 여기서는 간단하게 새로 시작하는 방식을 사용합니다.
        StopCoroutine("PowerUpRoutine");
        StartCoroutine("PowerUpRoutine");
    }
}