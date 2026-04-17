using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/FishData")]
public class FishData : ScriptableObject
{
    public string fishName;      // 물고기 이름
    public int score;            // 획득 점수

    [Header("Difficulty Settings")]
    public float fishSpeed;      // 물고기 이동 속도 (높을수록 빠름)
    public float directionChangeTimer; // 방향 전환 주기 (짧을수록 예측 불허)
    public float catchZoneSize;  // 플레이어의 포획 영역 크기 (어려울수록 좁아짐)
}