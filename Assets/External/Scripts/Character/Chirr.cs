using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chirr : PlayerCharacter
{
    public string CharacterName => "Chirr";

    protected void Awake()
    {
        characterName = "Chirr";
        UpdateUI();
    }

    protected override void UseSkill()
    {
        // 1. MP 체크
        if (currentStats.MP < baseStats.MP)
        {
            Debug.Log($"{this.name} MP가 부족하여 스킬 사용 불가");
            return;
        }
        // [삭제됨] 공격력 15% 강화 로직을 제거함. 
        // 2. 스킬 비용 지불
        currentStats.MP = 0;

        // 3. SkillManager 연동: 적 대상 선택 모드 활성화
        // - 시전자가 Chirr임을 알림으로써 SkillManager가 적에게 ApplyBleed(출혈)를 실행하도록 함
        // - 두 번째 인자(20f): 적에게 입힐 기본 데미지 수치
        SkillManager.Instance.ActivateTargetSelectMode(this, 20f);

        // 4. 상태 반영
        UpdateUI();
        Debug.Log($"{this.name} 스킬 시전: 적 1체 타겟팅 및 출혈(2턴간 3%) 준비");
        MySoundManager.Instance.PlayChirrSkill();
    }
}