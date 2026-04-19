using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    public static ArtifactManager Instance { get; private set; }

    // Inspector에서 6개 ArtifactData asset을 여기에 드래그해서 할당
    [SerializeField] private ArtifactData[] artifacts;

    private void Awake()
    {
        // 싱글톤: 씬 이동해도 하나만 유지
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ArtifactUI에서 전체 아티팩트 목록을 가져올 때 사용
    public ArtifactData[] GetAllArtifacts() => artifacts;

    // PlayerPrefs에서 아티팩트 레벨 읽기 (없으면 0 = 잠김)
    public int GetLevel(string artifactID)
    {
        return PlayerPrefs.GetInt("ArtLevel_" + artifactID, 0);
    }

    // 아티팩트 레벨 저장 (게임 중 획득/강화 시 호출)
    public void SetLevel(string artifactID, int level)
    {
        PlayerPrefs.SetInt("ArtLevel_" + artifactID, level);
        PlayerPrefs.Save();
    }
}
