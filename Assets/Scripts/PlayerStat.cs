using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public float spawnRateBonus = 0f;
    public float gaugeSizeBonus = 0f;
    public float rareChanceBonus = 0f;
    public float scoreBonus = 0f;
    public int failProtectCount = 0;
    public float controlEaseBonus = 0f;

    public void ApplyItem(ItemData item)
    {
        switch (item.itemType)
        {
            case ItemType.SpawnRateUp:
                spawnRateBonus += item.value;
                break;

            case ItemType.GaugeSizeUp:
                gaugeSizeBonus += item.value;
                break;

            case ItemType.RareChanceUp:
                rareChanceBonus += item.value;
                break;

            case ItemType.ScoreUp:
                scoreBonus += item.value;
                break;

            case ItemType.FailProtect:
                failProtectCount += (int)item.value;
                break;

            case ItemType.ControlEase:
                controlEaseBonus += item.value;
                break;
        }

        Debug.Log(
            $"{item.itemName} 적용 완료\n" +
            $"spawnRateBonus: {spawnRateBonus}\n" +
            $"gaugeSizeBonus: {gaugeSizeBonus}\n" +
            $"rareChanceBonus: {rareChanceBonus}\n" +
            $"scoreBonus: {scoreBonus}\n" +
            $"failProtectCount: {failProtectCount}\n" +
            $"controlEaseBonus: {controlEaseBonus}"
        );
    }
}