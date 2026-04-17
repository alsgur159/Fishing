using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;

    void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (volumeSlider != null && SettingsManager.Instance != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = SettingsManager.Instance.GetVolume();
            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    public void TogglePanel()
    {
        if (settingsPanel == null) return;
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void OnSliderChanged(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SetVolume(value);
    }
}
