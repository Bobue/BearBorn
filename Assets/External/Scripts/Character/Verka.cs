using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Verka : PlayerCharacter
{
    public string CharacterName => "Verka";

    protected void Awake()
    {
        // 부모 클래스의 변수 characterName 설정 및 UI 초기화
        characterName = "Verka";
        UpdateUI();
    }

    protected override void UseSkill()
    {
        // 1. MP 체크: 최대치(baseStats.MP)일 때만 발동 가능
        if (currentStats.MP < baseStats.MP)
        {
            Debug.Log($"{this.name} MP가 부족하여 스킬 사용 불가");
            return;
        }

        // 2. 스킬 비용 지불: MP를 0으로 초기화
        currentStats.MP = 0;

        // 3. SkillManager 연동: 적 대상 선택 모드 활성화
        // - 첫 번째 인자(this): 스킬 시전자가 베르카임을 알림 (SkillManager에서 이 정보를 보고 턴 지연 효과를 적용함)
        // - 두 번째 인자(35f): 적에게 입힐 기본 데미지 수치
        SkillManager.Instance.ActivateTargetSelectMode(this, 35f);

        // 4. 상태 반영 및 로그
        UpdateUI();
        Debug.Log($"{this.name} 스킬 시전: 적 1체 타겟팅 및 1턴 지연 효과 준비");
        MySoundManager.Instance.PlayVerkaSkill();
    }
}