using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtifactIconSlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nameText;
    public string artifactID;

    private void Awake()
    {
        if (iconImage == null) iconImage = GetComponentInChildren<Image>();
        if (levelText == null || nameText == null)
        {
            var texts = GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 1 && levelText == null) levelText = texts[0];
            if (texts.Length >= 2 && nameText == null) nameText = texts[1];
        }
    }

    public void Init(ArtifactData data, int level)
    {
        artifactID = data.artifactID;
        iconImage.sprite = data.icon;
        if (nameText != null) nameText.text = data.artifactName;
        UpdateLevel(level);
    }

    public void UpdateLevel(int level)
    {
        levelText.text = "Lv." + level;
    }
}
