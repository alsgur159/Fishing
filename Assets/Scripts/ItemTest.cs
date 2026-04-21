using UnityEngine;
using UnityEngine.InputSystem;

public class ItemTest : MonoBehaviour
{
    public ItemManager itemManager;
    public ItemPopupUI itemPopupUI;

    private void Start()
    {
        if (itemManager == null)
        {
            itemManager = FindObjectOfType<ItemManager>();
        }
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (itemManager == null)
            {
                Debug.LogError("ItemManager가 연결되지 않았습니다.");
                return;
            }

            ItemData item = itemManager.GiveRandomItem();

            if (itemPopupUI != null)
            {
                itemPopupUI.ShowItem(item);
            }
        }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (ScoreManager.Instance == null)
            {
                Debug.LogError("ScoreManager.Instance가 없습니다.");
                return;
            }

            ScoreManager.Instance.AddFish();
            Debug.Log("물고기 1마리 획득 처리");
            Debug.Log("현재 점수: " + ScoreManager.Instance.GetCurrentScore());
        }
    }
}