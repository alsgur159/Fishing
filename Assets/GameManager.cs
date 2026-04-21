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

    private List<ArtifactData> availableArtifacts = new List<ArtifactData>();

    [Header("Chest Probability Weights")]
    public int weightArtifact = 100;
    public int weightPowerUp = 0;
    public int weightScore = 0;
    public int weightTrash = 0;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public ItemPopUi itemPopUi;

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
        // PlayerPrefs.DeleteAll(); 테스트용 초기화 코드

        StartGameLoop();
    }

    public void StartGameLoop()
    {
        totalScore = 0;
        lives = 3;


        int lifeLevel = GetArtifactLevel("art_extralife");
        lives += lifeLevel;

        bonusBarSize = 0f;
        bonusCatchSpeed = 0f;
        highScoreFishChance = 0f;
        extraScoreBonus = 0;
        scoreCriticalChance = 0f;

        foreach (Transform child in topBarUIParent)
        {
            Destroy(child.gameObject);
        }

        LoadUnlockedArtifacts();


        PrepareArtifactPool();

        UpdateUI();
        gameOverPanel.SetActive(false);
        SpawnNextFish();
    }

    private void LoadUnlockedArtifacts()
    {
        foreach (ArtifactData art in artifactDatabase)
        {
            int level = GetArtifactLevel(art.artifactID);
            if (level > 0)
            {
                for (int i = 0; i < level; i++)
                {
                    ApplyArtifactEffect(art);
                }

                CreateOrUpdateArtifactUI(art, level);
            }
        }
    }

    private void PrepareArtifactPool()
    {
        availableArtifacts.Clear();
        foreach (ArtifactData art in artifactDatabase)
        {
            if (GetArtifactLevel(art.artifactID) < 3)
            {
                availableArtifacts.Add(art);
            }
        }
    }

    public void OpenChestReward()
    {
        Debug.Log("OpenChestReward 호출됨");
        Debug.Log("availableArtifacts 개수: " + availableArtifacts.Count);

        int totalWeight = weightArtifact + weightPowerUp + weightScore + weightTrash;
        if (availableArtifacts.Count == 0) totalWeight -= weightArtifact;

        int roll = Random.Range(0, totalWeight);
        Debug.Log("roll: " + roll);

        if (availableArtifacts.Count > 0 && roll < weightArtifact)
        {
            // Debug.Log("아티팩트 분기 진입");

            ArtifactData artifact = ObtainArtifact();
            // Debug.Log("획득 아티팩트: " + (artifact != null ? artifact.artifactName : "null"));
            // Debug.Log("itemPopUi 연결 상태: " + (itemPopUi != null));

            if (artifact != null && itemPopUi != null)
            {
                // Debug.Log("ShowArtifact 호출 직전");
                itemPopUi.ShowArtifact(artifact);
            }
        }
        else if (roll < weightArtifact + weightPowerUp)
        {
            // Debug.Log("파워업 분기 진입");
            minigame.TriggerInstantPowerUp();
        }
        else if (roll < weightArtifact + weightPowerUp + weightScore)
        {
            // Debug.Log("점수 분기 진입");
            AddScore(Random.Range(50, 151));
        }
        else
        {
            // Debug.Log("꽝 분기 진입");
        }
    }
    private ArtifactData ObtainArtifact()
    {
        if (availableArtifacts.Count == 0) return null;

        int randomIndex = Random.Range(0, availableArtifacts.Count);
        ArtifactData acquired = availableArtifacts[randomIndex];

        int currentLevel = GetArtifactLevel(acquired.artifactID);
        currentLevel++;
        SetArtifactLevel(acquired.artifactID, currentLevel);

        ApplyArtifactEffect(acquired);
        CreateOrUpdateArtifactUI(acquired, currentLevel);

        if (currentLevel >= 3)
        {
            availableArtifacts.RemoveAt(randomIndex);
            Debug.Log($"{acquired.artifactName} 최대 레벨!");
        }

        UpdateUI();
        return acquired;
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
                // PermanentExtraLife�� StartGameLoop���� ó����
        }
    }

    // --- UI ���� ---
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

    // --- ���� �ý��� ---
    private void SetArtifactLevel(string id, int level)
    {
        PlayerPrefs.SetInt("ArtLevel_" + id, level);
        PlayerPrefs.Save();
    }

    public int GetArtifactLevel(string id)
    {
        return PlayerPrefs.GetInt("ArtLevel_" + id, 0);
    }

    // --- �����Ǿ��� �ٽ� ������ ���� �Ϸ�! ---

    public void AddScore(int amount)
    {
        int finalAddScore = amount + extraScoreBonus;
        if (scoreCriticalChance > 0f && Random.value <= scoreCriticalChance)
        {
            finalAddScore *= 2;
            Debug.Log("ũ��Ƽ��! ���� 2�� ȹ��!");
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

        // ���� ������ ���� Ȯ�� �ߵ�
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
        if (ScoreManager.Instance != null) ScoreManager.Instance.SaveRecord(totalScore);
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {totalScore}";
        if (livesText != null) livesText.text = $"Lives: {lives}";
    }
}