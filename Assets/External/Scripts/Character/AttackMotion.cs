using UnityEngine;
using DG.Tweening;

public class AttackMotion : MonoBehaviour
{
    public float backOffset = 1f;
    public float duration = 0.15f;

    public void PlayAttack(Transform targetTransform)
    {
        // [수정] 지연 상태 체크: 스스로 지연 상태라면 연출을 원천 차단합니다.
        EnemyCharacter enemy = GetComponent<EnemyCharacter>();
        if (enemy != null && enemy.ShouldSkipTurn)
        {
            Debug.Log($"[AttackMotion] {gameObject.name}는 지연 상태이므로 연출을 취소합니다.");
            return;
        }

        // 이전 진행 중인 모든 트윈을 강제 종료하여 겹침 현상 방지
        transform.DOKill();

        Vector3 originalPos = transform.position;
        Vector3 dir = (targetTransform.position - originalPos).normalized;

        // 태그에 따른 방향 설정 (플레이어는 뒤로, 적은 앞으로 예비 동작)
        Vector3 backPos = gameObject.CompareTag("Player") ? originalPos - dir * backOffset : originalPos + dir * backOffset;

        // 타격 지점 설정
        float hitDistance = Vector3.Distance(originalPos, targetTransform.position) * 0.2f;
        Vector3 hitPos = originalPos + dir * hitDistance;

        Debug.Log($"[AttackMotion] {gameObject.name} 돌진 시작");

        // 시퀀스를 사용하여 애니메이션 구성
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMove(backPos, duration)
                .SetEase(Ease.OutQuad))               // 예비 동작

           .AppendCallback(() =>
           {
               MySoundManager.Instance.PlayNormalAttack();
           })

           .Append(transform.DOMove(hitPos, duration * 0.7f)
                .SetEase(Ease.InQuad))                // 돌격

           .Append(transform.DOMove(originalPos, duration)
                .SetEase(Ease.OutCubic));             // 복귀

    }
}