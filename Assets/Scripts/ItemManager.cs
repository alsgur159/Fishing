using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    public PlayerStat playerStat;
    public List<ItemData> itemList = new List<ItemData>();

    [Header("아이템 아이콘")]
    public Sprite goldenBaitIcon;
    public Sprite heartOfSeaIcon;
    public Sprite luckyCloverIcon;
    public Sprite pirateCoinIcon;
    public Sprite slimeReelIcon;
    public Sprite zephyrLureIcon;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (playerStat == null)
            playerStat = Object.FindFirstObjectByType<PlayerStat>();

        CreateDefaultItems();
    }

    void CreateDefaultItems()
    {
        itemList.Clear();

        itemList.Add(new ItemData
        {
            itemName = "황금 미끼",
            itemType = ItemType.SpawnRateUp,
            value = 0.2f,
            icon = goldenBaitIcon
        });

        itemList.Add(new ItemData
        {
            itemName = "바다의 심장",
            itemType = ItemType.GaugeSizeUp,
            value = 0.3f,
            icon = heartOfSeaIcon
        });

        itemList.Add(new ItemData
        {
            itemName = "행운의 클로버",
            itemType = ItemType.RareChanceUp,
            value = 0.15f,
            icon = luckyCloverIcon
        });

        itemList.Add(new ItemData
        {
            itemName = "해적의 금화",
            itemType = ItemType.ScoreUp,
            value = 0.25f,
            icon = pirateCoinIcon
        });

        itemList.Add(new ItemData
        {
            itemName = "슬라임 릴",
            itemType = ItemType.FailProtect,
            value = 1f,
            icon = slimeReelIcon
        });

        itemList.Add(new ItemData
        {
            itemName = "바람의 루어",
            itemType = ItemType.ControlEase,
            value = 0.2f,
            icon = zephyrLureIcon
        });
    }

    public ItemData GiveRandomItem()
    {
        if (itemList.Count == 0)
            return null;

        int randomIndex = Random.Range(0, itemList.Count);
        ItemData randomItem = itemList[randomIndex];

        if (playerStat != null)
            playerStat.ApplyItem(randomItem);

        return randomItem;
    }
}