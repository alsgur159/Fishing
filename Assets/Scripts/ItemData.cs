using UnityEngine;

public enum ItemType
{
    SpawnRateUp,      // 황금 미끼
    GaugeSizeUp,      // 바다의 심장
    RareChanceUp,     // 행운의 클로버
    ScoreUp,          // 해적의 금화
    FailProtect,      // 슬라임 릴
    ControlEase       // 바람의 루어
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public ItemType itemType;
    public float value;
    public Sprite icon;
}