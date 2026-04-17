using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtifactIconSlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI levelText;
    public string artifactID;

    public void Init(ArtifactData data, int level)
    {
        artifactID = data.artifactID;
        iconImage.sprite = data.icon;
        UpdateLevel(level);
    }

    public void UpdateLevel(int level)
    {
        levelText.text = "Lv." + level;
    }
}