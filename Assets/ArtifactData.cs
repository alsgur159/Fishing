using UnityEngine;

[CreateAssetMenu(fileName = "NewArtifact", menuName = "Fishing/ArtifactData")]
public class ArtifactData : ScriptableObject
{
    public string artifactID;
    public string artifactName;
    public Sprite icon;
    [TextArea]
    public string description;

    // 기획하신 6가지 유물 효과 목록
    public enum EffectType
    {
        IncreaseBarSize,       // 1. 바 크기 영구 증가 (effectValue: 0.05 등 비율)
        IncreaseCatchSpeed,    // 2. 잡는 속도 영구 증가 (effectValue: 5 등 수치)
        HighScoreFishRateUp,   // 3. 높은 점수 물고기 등장 확률 증가 (effectValue: 0.2 등 20% 확률)
        ExtraScoreBonus,       // 4. 물고기 획득 시 추가 점수 (effectValue: 50 등 고정 점수)
        PermanentExtraLife,    // 5. 게임 시작 시 영구 추가 생명 (도감 해금 시 영구 적용)
        ScoreCriticalChance    // 6. 점수 크리티컬 확률 (effectValue: 0.1 등 10% 확률)
    }

    public EffectType effectType;
    public float effectValue;     // 인스펙터에서 효과의 수치를 결정합니다.
}