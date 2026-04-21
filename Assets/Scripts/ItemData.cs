using UnityEngine;

public enum ItemType
{
    SpawnRateUp,
    GaugeSizeUp,
    RareChanceUp,
    ScoreUp,
    FailProtect,
    ControlEase
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public ItemType itemType;
    public float value;
    public Sprite icon;
}