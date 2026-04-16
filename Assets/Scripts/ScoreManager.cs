using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public const int SCORE_PER_FISH = 10;
    public const int Max_RECORDS = 5;
    private const string SAVE_KEY = "FishingScores";

    private int currentScore = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance =this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddFish(int count = 1)
    {
        currentScore += SCORE_PER_FISH * count;
    }
    public int GetCurrentScore() => currentScore;
  
    public void SaveRecord()
    {
        List<FishingScore> records = LoadRecords();
        records.Add(new FishingScore(currentScore, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

        records = records
        .OrderByDescending(r => r.score)
        .Take(Max_RECORDS)
        .ToList();

        PlayerPrefs.SetInt("RecordCount", records.Count);
        for (int i = 0; i < records.Count; i++)
        {
            PlayerPrefs.SetInt($"Record_{i}_Score", records[i].score);
            PlayerPrefs.SetString($"Record_{i}_Date", records[i].date);
        }

        PlayerPrefs.Save();

        currentScore = 0;
    }

    public List<FishingScore> LoadRecords()
    {
        List<FishingScore> records = new List<FishingScore>();
        int count = PlayerPrefs.GetInt("RecordCount", 0);

        for (int i = 0; i < count; i++)
        {
            int score = PlayerPrefs.GetInt($"Record_{i}_Score", 0);
            string date = PlayerPrefs.GetString($"Record_{i}_Date", "Unknown");
            records.Add(new FishingScore(score, date));
        }
        return records;
    }
}
