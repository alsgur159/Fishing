using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ItemPopUi : MonoBehaviour
{
    public GameObject popupPanel;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;

    private Coroutine currentCoroutine;

    private void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void ShowArtifact(ArtifactData artifact)
    {
        //Debug.Log("ShowArtifact ½ÇÇàµÊ");

        if (artifact == null)
        {
            Debug.LogError("artifact°¡ null");
            return;
        }

        if (popupPanel == null)
        {
            Debug.LogError("popupPanel ¿¬°á ¾È µÊ");
            return;
        }

        if (itemIcon == null)
        {
            Debug.LogError("itemIcon ¿¬°á ¾È µÊ");
            return;
        }

        if (itemNameText == null)
        {
            Debug.LogError("itemNameText ¿¬°á ¾È µÊ");
            return;
        }

        popupPanel.SetActive(true);
        popupPanel.transform.localScale = Vector3.zero;

        itemIcon.gameObject.SetActive(true);
        itemIcon.sprite = artifact.icon;
        itemNameText.text = artifact.artifactName;

        currentCoroutine = StartCoroutine(PopupAnimation());
    }

    private IEnumerator PopupAnimation()
    {
        float time = 0f;
        float showDuration = 0.2f;

        while (time < showDuration)
        {
            time += Time.deltaTime;
            float t = time / showDuration;
            float scale = Mathf.Lerp(0f, 1.1f, t);
            popupPanel.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        popupPanel.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(1.5f);

        time = 0f;
        float hideDuration = 0.15f;

        while (time < hideDuration)
        {
            time += Time.deltaTime;
            float t = time / hideDuration;
            float scale = Mathf.Lerp(1f, 0f, t);
            popupPanel.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        popupPanel.SetActive(false);
    }
}