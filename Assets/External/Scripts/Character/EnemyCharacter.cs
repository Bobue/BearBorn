using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyCharacter : CharacterBase, IPointerClickHandler
{
    public MonsterData monsterData;
    public float attackDuration;

    private int delayedTurns = 0;
    private int bleedDuration = 0;
    private float bleedPercent = 0f;

    // [상태 공개] 외부 매니저나 연출 스크립트에서 현재 지연 상태인지 확인용
    public bool ShouldSkipTurn => delayedTurns > 0;

    protected override void Start()
    {
        base.Start();
        currentStats = baseStats.Clone();
        currentStats.MP = 0;
        UpdateUI();
    }

    // 베르카 스킬 등을 통해 호출됨
    public void DelayTurn(int amount)
    {
        delayedTurns += amount;
        Debug.Log($"[EnemyCharacter] {name} 턴 지연 추가! 현재 스택: {delayedTurns}");
    }

    // 치르 스킬 등을 통해 호출됨
    public void ApplyBleed(int duration, float percent)
    {
        bleedDuration = duration;
        bleedPercent = percent;
        Debug.Log($"[EnemyCharacter] {name} 출혈 효과 적용됨.");
    }

    public IEnumerator Attacking()
    {
        // 1. 지연 상태 체크 (핵심 로직)
        if (delayedTurns > 0)
        {
            Debug.Log($"[Attacking] {name} 지연 상태 확인됨. 공격 루틴을 중단하고 스택을 소모합니다.");

            // 혹시 연출이 돌아가고 있다면 즉시 정지
            AttackMotion am = GetComponent<AttackMotion>();
            if (am != null) am.transform.DOKill();

            delayedTurns--; // 지연 스택 1회 소모
            yield break;    // 이후 모든 로직(출혈, 공격 대기 등) 실행 안 함
        }

        // 2. 출혈 데미지 처리 (지연이 아닐 때만)
        if (bleedDuration > 0)
        {
            float damage = baseStats.HP * bleedPercent;
            TakeDamage(damage);
            bleedDuration--;
            Debug.Log($"[Attacking] {name} 출혈 피해 발생: {damage}. 남은 턴: {bleedDuration}");
        }

        // 3. 실제 공격 대기 로그
        Debug.Log($"[Attacking] {name}의 공격 대기 중...");
        yield return new WaitForSeconds(attackDuration > 0 ? attackDuration : 1.5f);
    }

    public override void TakeDamage(float damageAmount)
    {
        base.TakeDamage(damageAmount);
        Debug.Log($"[TakeDamage] {name}이 {damageAmount} 피해를 입음. 남은 HP: {currentStats.HP}");

        if (currentStats.HP <= 0)
        {
            Debug.Log($"[Death] {name} 사망.");
            EnemyManager.Instance.RemoveEnemy(this);
            Destroy(gameObject);
        }
    }

    // 마우스 클릭 시 스킬 타겟으로 지정
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (SkillManager.Instance != null && SkillManager.Instance.IsTargetSelectModeActive())
        {
            Debug.Log($"[SkillTarget] {name}이 스킬 대상으로 선택되었습니다.");
            SkillManager.Instance.ApplySkillToTarget(this);
        }
    }
}