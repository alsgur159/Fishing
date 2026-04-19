using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public FishingMinigame minigame;
    public FishData[] fishDatabase;
    public ArtifactData[] artifactDatabase;
    public float delayBetweenFish = 1.5f;

    [Header("Player Stats")]
    public int totalScore = 0;
    public int lives = 3;

    [Header("Roguelike Stats (Stacked)")]
    public float bonusBarSize = 0f;
    public float bonusCatchSpeed = 0f;
    public float highScoreFishChance = 0f;
    public int extraScoreBonus = 0;
    public float scoreCriticalChance = 0f;

    // 유물 뽑기 풀: 아직 3레벨이 안 된 유물들을 담습니다.
    private List<ArtifactData> availableArtifacts = new List<ArtifactData>();

    [Header("Chest Probability Weights")]
    public int weightArtifact = 10;
    public int weightPowerUp = 30;
    public int weightScore = 40;
    public int weightTrash = 20;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Artifact UI")]
    public Transform topBarUIParent;
    public GameObject artifactIconPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartGameLoop();
    }

    public void StartGameLoop()
    {
        totalScore = 0;
        lives = 3;

        // [도감 연동] 영구 생명 유물 레벨만큼 목숨 추가
        int lifeLevel = GetArtifactLevel("art_extralife");
        lives += lifeLevel;

        // 스탯 및 UI 초기화
        bonusBarSize = 0f;
        bonusCatchSpeed = 0f;
        highScoreFishChance = 0f;
        extraScoreBonus = 0;
        scoreCriticalChance = 0f;

        foreach (Transform child in topBarUIParent)
        {
            Destroy(child.gameObject);
        }

        // 1. 저장된 유물들(레벨 1~3) 모두 불러와서 효과 중첩 적용
        LoadUnlockedArtifacts();

        // 2. 3레벨 미만인 유물들만 뽑기 풀에 세팅
        PrepareArtifactPool();

        UpdateUI();
        gameOverPanel.SetActive(false);
        SpawnNextFish(); // 여기서 호출되는 함수 복구 완료!
    }

    private void LoadUnlockedArtifacts()
    {
        foreach (ArtifactData art in artifactDatabase)
        {
            int level = GetArtifactLevel(art.artifactID);
            if (level > 0)
            {
                // 레벨만큼 효과 반복 적용 (수치가 중첩됨)
                for (int i = 0; i < level; i++)
                {
                    ApplyArtifactEffect(art);
                }

                // UI 생성 및 레벨 표시
                CreateOrUpdateArtifactUI(art, level);
            }
        }
    }

    private void PrepareArtifactPool()
    {
        availableArtifacts.Clear();
        foreach (ArtifactData art in artifactDatabase)
        {
            // 레벨이 3 미만인 유물만 상자에서 등장 가능
            if (GetArtifactLevel(art.artifactID) < 3)
            {
                availableArtifacts.Add(art);
            }
        }
    }

    public void OpenChestReward()
    {
        int totalWeight = weightArtifact + weightPowerUp + weightScore + weightTrash;
        if (availableArtifacts.Count == 0) totalWeight -= weightArtifact;

        int roll = Random.Range(0, totalWeight);

        if (availableArtifacts.Count > 0 && roll < weightArtifact)
        {
            ObtainArtifact();
        }
        else if (roll < weightArtifact + weightPowerUp)
        {
            minigame.TriggerInstantPowerUp();
        }
        else if (roll < weightArtifact + weightPowerUp + weightScore)
        {
            AddScore(Random.Range(50, 151));
            Debug.Log("보너스 점수 획득!");
        }
        else
        {
            Debug.Log("꽝! 쓰레기가 나왔습니다.");
        }
    }

    private void ObtainArtifact()
    {
        if (availableArtifacts.Count == 0) return;

        int randomIndex = Random.Range(0, availableArtifacts.Count);
        ArtifactData acquired = availableArtifacts[randomIndex];

        // 1. 레벨업 및 저장 (최대 3레벨)
        int currentLevel = GetArtifactLevel(acquired.artifactID);
        currentLevel++;
        SetArtifactLevel(acquired.artifactID, currentLevel);

        // 2. 효과 적용 (중첩)
        ApplyArtifactEffect(acquired);

        // 3. UI 업데이트
        CreateOrUpdateArtifactUI(acquired, currentLevel);

        // 4. 3레벨이 되면 뽑기 풀에서 제거
        if (currentLevel >= 3)
        {
            availableArtifacts.RemoveAt(randomIndex);
            Debug.Log($"{acquired.artifactName} 만렙 달성!");
        }

        UpdateUI();
    }

    private void ApplyArtifactEffect(ArtifactData acquired)
    {
        switch (acquired.effectType)
        {
            case ArtifactData.EffectType.IncreaseBarSize: bonusBarSize += acquired.effectValue; break;
            case ArtifactData.EffectType.IncreaseCatchSpeed: bonusCatchSpeed += acquired.effectValue; break;
            case ArtifactData.EffectType.HighScoreFishRateUp: highScoreFishChance += acquired.effectValue; break;
            case ArtifactData.EffectType.ExtraScoreBonus: extraScoreBonus += (int)acquired.effectValue; break;
            case ArtifactData.EffectType.ScoreCriticalChance: scoreCriticalChance += acquired.effectValue; break;
                // PermanentExtraLife는 StartGameLoop에서 처리됨
        }
    }

    // --- UI 관리 ---
    private void CreateOrUpdateArtifactUI(ArtifactData art, int level)
    {
        ArtifactIconSlot existingSlot = null;
        foreach (Transform child in topBarUIParent)
        {
            var slot = child.GetComponent<ArtifactIconSlot>();
            if (slot != null && slot.artifactID == art.artifactID)
            {
                existingSlot = slot;
                break;
            }
        }

        if (existingSlot != null)
        {
            existingSlot.UpdateLevel(level);
        }
        else
        {
            GameObject newIcon = Instantiate(artifactIconPrefab, topBarUIParent);
            var slot = newIcon.GetComponent<ArtifactIconSlot>();
            if (slot != null) slot.Init(art, level);
        }
    }

    // --- 저장 시스템 ---
    private void SetArtifactLevel(string id, int level)
    {
        PlayerPrefs.SetInt("ArtLevel_" + id, level);
        PlayerPrefs.Save();
    }

    public int GetArtifactLevel(string id)
    {
        return PlayerPrefs.GetInt("ArtLevel_" + id, 0);
    }

    // --- 삭제되었던 핵심 로직들 복구 완료! ---

    public void AddScore(int amount)
    {
        int finalAddScore = amount + extraScoreBonus;
        if (scoreCriticalChance > 0f && Random.value <= scoreCriticalChance)
        {
            finalAddScore *= 2;
            Debug.Log("크리티컬! 점수 2배 획득!");
        }
        totalScore += finalAddScore;
        UpdateUI();
        StartCoroutine(WaitAndSpawnNextFish());
    }

    public void FailFishing()
    {
        lives--;
        UpdateUI();
        if (lives <= 0) GameOver();
        else StartCoroutine(WaitAndSpawnNextFish());
    }

    private IEnumerator WaitAndSpawnNextFish()
    {
        yield return new WaitForSeconds(delayBetweenFish);
        SpawnNextFish();
    }

    private void SpawnNextFish()
    {
        FishData nextFish = fishDatabase[Random.Range(0, fishDatabase.Length)];

        // 고급 물고기 등장 확률 발동
        if (highScoreFishChance > 0f && Random.value <= highScoreFishChance)
        {
            FishData reroll = fishDatabase[Random.Range(0, fishDatabase.Length)];
            if (reroll.score > nextFish.score) nextFish = reroll;
        }

        minigame.StartFishing(nextFish);
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = $"Final Score: {totalScore}";
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {totalScore}";
        if (livesText != null) livesText.text = $"Lives: {lives}";
    }
}