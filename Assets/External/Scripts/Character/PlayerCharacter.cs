using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCharacter : CharacterBase, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    [Header("Mana Full Glow")]
    [SerializeField] private GameObject manaGlowObject;

    private Tween manaGlowTween;

    protected override void Awake()
    {
        base.Awake();
        CharacterPlayerHolder.Instance.ApplyToCharacter(this);
    }

    protected override void Start()
    {
        base.Start();
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))//잠시 스킬 테스트용으로 함수 추가
        {
            currentStats.MP = baseStats.MP;
            UpdateUI();

            Debug.Log($"{this.name}: 스페이스바로 MP가 {baseStats.MP}로 가득 찼습니다!");
        }
    }

    protected virtual void UseSkill()
    {
        currentStats.MP = Mathf.Max(0, currentStats.MP - 100);
        UpdateUI();
        MySoundManager.Instance.PlayOneHundredMana();
        Debug.Log($"{this.name} 스킬 시전");
    }
    public void PlusExp(int exp)
    {
        Debug.Log($"현재 레벨{baseStats.LV}");
        CurrentStats.Exp += exp;

        while(currentStats.Exp >=currentStats.ExpToNextLevel)
        {
            currentStats.Exp -= currentStats.ExpToNextLevel;
            LevelUp();
        }
        //CheckLevelUp();
    }
    protected virtual void CheckLevelUp()
    {
        while (BaseStats.Exp >= GetMaxExp())
        {
            BaseStats.Exp -= GetMaxExp();
            LevelUp();
        }
    }
    protected virtual float GetMaxExp()
    {
        // 1레벨: 100, 2레벨: 200 … 같은 방식으로도 확장 가능
        return BaseStats.LV * 100f;
    }
    protected virtual void LevelUp()
    {
        CurrentStats.LV++;
        BaseStats.LVPoint++;

        GrowthStep step = growthTable.GetStep(currentStats.LV);
        if(step != null)
        {
            BaseStats.HP += step.hpPerLevel;
            BaseStats.ATK += step.atkPerLevel;
            BaseStats.DEF += step.defPerLevel;
            BaseStats.LUK += step.lukPerLevel;

            currentStats.ExpToNextLevel += step.expLevel;
        }

        
        // 현재 스탯 갱신
        currentStats = BaseStats.Clone();
        UpdateUI();

        Debug.Log(
            $"{name} 레벨업! " +
            $"Lv.{BaseStats.LV} / 스킬포인트 +1 (총 {BaseStats.LVPoint})"
        );
        CharacterPlayerHolder.Instance.Setstats(characterName, BaseStats);//홀더 테스트용

    }
    public override void UpdateUI()
    {
        base.UpdateUI();
        UpdateManaGlow();
    }


    private void UpdateManaGlow()
    {
        // 🔐 manaGlowObject가 없으면 아무 것도 하지 않음
        if (manaGlowObject == null)
            return;

        bool isManaFull = currentStats.MP >= baseStats.MP;

        if (isManaFull)
        {
            if (manaGlowTween != null && manaGlowTween.IsActive())
                return;

            manaGlowObject.SetActive(true);
            manaGlowObject.transform.localScale = Vector3.one;

            manaGlowTween = manaGlowObject.transform
                .DOScale(1.15f, 0.7f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        else
        {
            if (manaGlowTween != null)
            {
                manaGlowTween.Kill();
                manaGlowTween = null;
            }

            manaGlowObject.transform.localScale = Vector3.one;
            manaGlowObject.SetActive(false);
        }
    }


    public virtual float CalculateFinalDamage(HandRank rank, List<Card> cardData)
    {
        float multiplier = HandEvaluator.GetDamageMultiplier(rank, cardData);
        float damage = currentStats.ATK * (1 + multiplier);

        Debug.Log($"[Verka] Rank: {rank}, Multiplier: {multiplier:F2}, Base ATK: {currentStats.ATK}, Final Damage: {damage:F2}");

        return damage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        OnPlayerClicked(eventData);
    }
    protected virtual void OnPlayerClicked(PointerEventData eventData)
    {
        // 수정: 특정 이름(Verka) 체크를 지우고, 아군 대상 지정 모드인지만 확인합니다.
        if (SkillManager.Instance.IsAllyTargetSelectModeActive())
        {
            Debug.Log($"[Ally Target Select] {this.characterName}이(가) 타겟으로 선택됨.");
            SkillManager.Instance.ApplySkillToAlly(this); // 이제 치르든 누구든 이 함수를 호출함
            return;
        }

        // 아래는 기존 로직 유지
        if (currentStats.MP >= baseStats.MP && StageManager.Instance.currentTurnState == TurnState.PLAYER)
        {
            UseSkill();
        }
        else
        {
            Debug.Log("마나가 부족하거나 플레이어 턴이 아닙니다!");
        }
    }
    public override void TakeDamage(float damageAmount)
    {
        base.TakeDamage(damageAmount);

        if (currentStats.HP <= 0)
        {
            PlayerManager.Instance.RemovePlayer(this);
            Destroy(gameObject); // 캐릭터 오브젝트 제거
        }
    }
    public virtual void ApplyATKMultiplier(float multiplier)
    {
        /*
        currentStats.ATK = BaseStats.ATK;
        currentStats.HP = BaseStats.HP;
        currentStats.DEF = BaseStats.DEF;
        currentStats.SHD = BaseStats.SHD;
        currentStats.LUK = BaseStats.LUK;
        currentStats.ExpToNextLevel = BaseStats.ExpToNextLevel;
        */
        currentStats.MP = 0;
        float before = currentStats.ATK;
        currentStats.ATK *= (1+multiplier);

        Debug.Log($"{name} ATK 강화! {before} → {currentStats.ATK}");

        UpdateUI();
    }
}
