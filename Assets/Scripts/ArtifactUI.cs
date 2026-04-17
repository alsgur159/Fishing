using UnityEngine;
using TMPro;


public class ArtifactUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject artifactPanel;

    public void Start()
    {
        if (artifactPanel != null)
            artifactPanel.SetActive(false);
    }

    public void TogglePanel()
    {
        if (artifactPanel == null) return;
        artifactPanel.SetActive(!artifactPanel.activeSelf);
    }
}
