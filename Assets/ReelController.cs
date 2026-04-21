using UnityEngine;

public class ReelController : MonoBehaviour
{
    public Animator reelAnimator;

    [Header("Animation Speed Settings")]
    // 바의 이동 속도(playerVelocity)를 애니메이션 속도에 얼마나 반영할지 정하는 배율입니다.
    // 애니메이션이 너무 빠르거나 느리면 이 값을 조절하세요. (예: 0.1 ~ 0.5)
    public float velocityWeight = 0.5f;

    void Start()
    {
        if (reelAnimator != null) reelAnimator.SetFloat("SpinSpeed", 0f);
    }

    // 이제 bool 상태가 아니라, 플레이어 바의 '속도' 자체를 받습니다.
    public void UpdateReelAnimation(float barVelocity)
    {
        if (reelAnimator == null) return;

        // 바가 위로 가면 양수(+) -> 릴 정방향
        // 바가 아래로 가면 음수(-) -> 릴 역방향
        // 바가 멈추면 0 -> 릴 멈춤
        float targetSpeed = barVelocity * velocityWeight;

        reelAnimator.SetFloat("SpinSpeed", targetSpeed);
    }

    public void StopReel()
    {
        if (reelAnimator != null) reelAnimator.SetFloat("SpinSpeed", 0f);
    }
}