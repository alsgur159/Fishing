using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private TextMeshProUGUI[] rankTexts;

    void Start()
    {
        if (rankingPanel != null)
            rankingPanel.SetActive(false);
    }

    public void TogglePanel()
    {
        if (rankingPanel == null) return;

        bool isActive = rankingPanel.activeSelf;
        rankingPanel.SetActive(!isActive);
        if (!isActive)
            DisplayRecords();

    }

    private void DisplayRecords()
    {
        if (ScoreManager.Instance == null) return;

        List<FishingScore> records = ScoreManager.Instance.LoadRecords();

        for (int i = 0; i < rankTexts.Length; i++)
        {
            if (rankTexts[i] == null) continue;

            if (i < records.Count)
                rankTexts[i].text = $"{i + 1} {records[i].score:N0}점  |  {records[i].date}";
            else
                rankTexts[i].text = $"{i + 1} -";
        }
    }
}
