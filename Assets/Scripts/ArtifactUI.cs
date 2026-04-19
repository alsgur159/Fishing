using UnityEngine;

public class ArtifactUI : MonoBehaviour
{
    // Inspector에서 연결: ArtifactPanel GameObject
    [SerializeField] private GameObject artifactPanel;
    // Inspector에서 연결: ArtifactIconSlot 프리팹
    [SerializeField] private GameObject slotPrefab;
    // Inspector에서 연결: 슬롯들이 들어갈 Content (Grid Layout Group 붙은 것)
    [SerializeField] private Transform content;

    public void Start()
    {
        // 게임 시작 시 패널 숨김
        if (artifactPanel != null)
            artifactPanel.SetActive(false);
    }

    // 유물 버튼 클릭 시 호출 (Button의 OnClick에 연결)
    public void TogglePanel()
    {
        if (artifactPanel == null) return;
        bool next = !artifactPanel.activeSelf;
        artifactPanel.SetActive(next);
        // 패널 열릴 때만 슬롯 새로 생성 (닫을 땐 불필요)
        if (next) PopulateSlots();
    }

    private void PopulateSlots()
    {
        // 이전에 생성된 슬롯 모두 제거 (중복 방지)
        foreach (Transform child in content)
            Destroy(child.gameObject);

        if (ArtifactManager.Instance == null) return;

        // 아티팩트마다 슬롯 프리팹 생성 후 데이터 초기화
        foreach (var data in ArtifactManager.Instance.GetAllArtifacts())
        {
            var go = Instantiate(slotPrefab, content);
            var slot = go.GetComponent<ArtifactIconSlot>();
            // PlayerPrefs에서 레벨 읽어서 슬롯에 표시 (0이면 잠김)
            int level = ArtifactManager.Instance.GetLevel(data.artifactID);
            slot.Init(data, level);
        }
    }
}
