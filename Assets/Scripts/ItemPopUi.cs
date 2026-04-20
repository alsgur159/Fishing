using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ItemPopupUI : MonoBehaviour
{
    public GameObject popupPanel;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;

    private Coroutine currentCoroutine;

    public void ShowItem(ItemData item)
    {
        if (item == null) return;
        
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        popupPanel.SetActive(true);
       
        popupPanel.transform.localScale = Vector3.zero;
   
        itemIcon.sprite = item.icon;
        itemNameText.text = item.itemName + " È¹µæ!";

        currentCoroutine = StartCoroutine(PopupAnimation());
    }

    IEnumerator PopupAnimation()
    {
        float duration = 0.2f;
        float time = 0f;


        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;


            float scale = Mathf.Lerp(0f, 1.1f, t);
            popupPanel.transform.localScale = new Vector3(scale, scale, 1f);

            yield return null;
        }

        popupPanel.transform.localScale = Vector3.one;
      
        yield return new WaitForSeconds(1.5f);

        time = 0f;
        while (time < 0.15f)
        {
            time += Time.deltaTime;
            float t = time / 0.15f;

            float scale = Mathf.Lerp(1f, 0f, t);
            popupPanel.transform.localScale = new Vector3(scale, scale, 1f);

            yield return null;
        }

        popupPanel.SetActive(false);
    }
}