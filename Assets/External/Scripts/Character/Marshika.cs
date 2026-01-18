using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marshika : PlayerCharacter
{
    public string CharacterName => "Marshika";

    protected void Awake()
    {
        characterName = "Marshika";
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

        // 2. 스킬 비용 지불
        currentStats.MP = 0;

        // 3. SkillManager 연동: 아군 대상 선택 모드 활성화
        // - ActivateAllyTargetSelectMode를 호출하여 아군을 클릭할 수 있는 상태로 만듦
        // - 마르쉬카의 경우 SkillManager 내부에서 대상의 최대 HP 10%를 계산하여 회복시킴
        SkillManager.Instance.ActivateAllyTargetSelectMode(this, 0f);

        // 4. 상태 반영 및 로그
        Debug.Log($"[Marshika 스킬 발동]: 아군 대상 선택 모드 활성화 (10% 회복)");
        UpdateUI();
        MySoundManager.Instance.PlayMarshikaSkill();
    }
    
}