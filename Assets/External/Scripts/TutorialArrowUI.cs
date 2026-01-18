using DG.Tweening;
using UnityEngine;

public class TutorialArrowUI : MonoBehaviour
{
    [Header("Move Settings")]
    public float moveDistance = 30f;   // 위아래 이동 거리
    public float duration = 0.6f;      // 왕복 시간

    private RectTransform rect;
    private Vector2 startPos;
    private Tween arrowTween;

    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;

        StartArrowMotion();
    }

    private void OnDisable()
    {
        arrowTween?.Kill();
        rect.anchoredPosition = startPos;
    }

    private void StartArrowMotion()
    {
        arrowTween = rect
            .DOAnchorPosY(startPos.y + moveDistance, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
