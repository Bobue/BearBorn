using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting;

public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField] protected Stats baseStats; //기준스탯
    [SerializeField] protected Stats currentStats;//일회성스탯
    [SerializeField] protected string characterName;//캐릭터 이름


    public Stats BaseStats => baseStats;//읽기 전용
    public Stats CurrentStats //읽기, 쓰기
    {
        get => currentStats;
        //set => currentStats = value;
        set
        {
#if UNITY_EDITOR
            Debug.Log(
                $"[STAT SET] {characterName}\n" +
                $"ATK: {currentStats?.ATK} → {value?.ATK}\n" +
                $"{System.Environment.StackTrace}"
            );
#endif
            currentStats = value;
        }
    }

    [SerializeField] protected GameObject hpUI;
    [SerializeField] protected GameObject mpUI;
    [SerializeField] protected TMPro.TMP_Text shieldText;
    [SerializeField] protected TMPro.TMP_Text hpText;

    [Header("Growth")]
    [SerializeField] protected StepGrowthTable growthTable;
    [Header("Level")]
    [SerializeField] protected int level = 1;
    public int Level => level;

    public GameObject HPUI
    {
        get => hpUI;
        set => hpUI = value;
    }

    public GameObject MPUI
    {
        get => mpUI;
        set => mpUI = value;
    }

    public void ApplyBaseStats(Stats newStats)
    {
        baseStats = newStats.Clone();
        CurrentStats = baseStats.Clone();
        UpdateUI();
    }


    public System.Action<DamageResult> OnDamaged;
    public struct DamageResult//데미지결과 반환용 구조체
    {
        public float rawDamage;
        public float hpDamage;
        public float shieldDamage;
        public bool isDead;
        public bool shieldBroken;
    }

    protected virtual void Awake()
    {
        Debug.Log(
        $"[Character Awake] {characterName}\n" +
        $"LV: {baseStats.LV}, Exp: {baseStats.Exp}\n" +
        $"HP: {baseStats.HP}, MP: {baseStats.MP}, ATK: {baseStats.ATK}, DEF: {baseStats.DEF}, SHD: {baseStats.SHD}"
    );
        //currentStats = baseStats.Clone();//임시
    }
    protected virtual void Start()
    {
        //CharacterPlayerHolder.Instance.ApplyToCharacter(this);

        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.name == "Hp")
                hpUI = child.gameObject;
            else if (child.name == "Mana")
                mpUI = child.gameObject;
            else if (child.name == "Shd")
                shieldText = child.GetComponentInChildren<TMPro.TMP_Text>();
            else if (child.name == "HpText")
                hpText = child.GetComponentInChildren<TMPro.TMP_Text>();
        }

        UpdateUI();
    }

    public virtual void ResetStats()
    {
        currentStats = baseStats.Clone();
    }
    public virtual void TakeDamage(float damageAmount)
    {
        float prevShield = currentStats.SHD;

        float remainingDamage = damageAmount;
        float shieldDamage = 0f;
        float hpDamage = 0f;

        if (currentStats.SHD > 0)
        {
            if (currentStats.SHD >= remainingDamage)
            {
                shieldDamage = remainingDamage;
                currentStats.SHD -= remainingDamage;
                remainingDamage = 0;
            }
            else
            {
                shieldDamage = currentStats.SHD;
                remainingDamage -= currentStats.SHD;
                currentStats.SHD = 0;
            }

            bool shieldBroken = prevShield > 0 && currentStats.SHD <= 0;

            if (shieldText != null)
                shieldText.text = currentStats.SHD.ToString();
        }

        if (remainingDamage > 0)
        {
            float reducedDamage = Mathf.Max(
                remainingDamage - currentStats.DEF,
                0f
            );

            hpDamage = reducedDamage;
            currentStats.HP -= reducedDamage;
            currentStats.HP = Mathf.Max(currentStats.HP, 0);
        }

        if (hpUI != null)
            hpUI.GetComponent<UnityEngine.UI.Slider>().value =
                currentStats.HP / baseStats.HP;

        OnDamaged?.Invoke(new DamageResult
        {
            rawDamage = damageAmount,
            hpDamage = hpDamage,
            shieldDamage = shieldDamage,
            isDead = currentStats.HP <= 0
        });
        UpdateUI();
    }



    public virtual void ChangeMP(float useMP) //MP 변경 코드 추가
    {
        print("마나감소");
        currentStats.MP -= useMP;
        currentStats.MP = Mathf.Max(currentStats.MP, 0);
        mpUI.GetComponent<UnityEngine.UI.Slider>().value = currentStats.MP / baseStats.MP;
    }

    public virtual void UpdateUI()
    {
        if (hpUI != null)
            hpUI.GetComponent<UnityEngine.UI.Slider>().value =
                currentStats.HP / baseStats.HP;

        if (hpText != null)
            hpText.text = Mathf.CeilToInt(currentStats.HP).ToString();

        if (mpUI != null)
            mpUI.GetComponent<UnityEngine.UI.Slider>().value =
                currentStats.MP / baseStats.MP;

        if (shieldText != null)
            shieldText.text = currentStats.SHD.ToString();
    }


    public virtual void ChangeShield(float amount)
    {
        Debug.Log($"[ChangeShield] BEFORE: {currentStats.SHD}");
        currentStats.SHD += amount;
        Debug.Log($"[ChangeShield] AFTER: {currentStats.SHD}");

        if (shieldText != null)
            shieldText.text = currentStats.SHD.ToString();
    }

    public void LevelUp(int amount = 1)
    {
        level += amount;
        level = Mathf.Max(level, 1);

        RecalulateStats();
    }

    public void RecalulateStats()
    {
        if (growthTable == null) 
        {
            Debug.LogWarning("성장 테이블이 없습니다");
            return;
        }

        Stats baseStat = CharacterPlayerHolder.Instance.GetStats(characterName);
        Stats result = baseStats.Clone();

        for(int lv = 1;lv<result.LV; lv++)
        {
            GrowthStep step = growthTable.GetStep(lv);
            if (step == null) continue;

            result.HP += step.hpPerLevel;
            result.ATK += step.atkPerLevel;
            result.DEF += step.defPerLevel;
            result.LUK += step.lukPerLevel;
        }

        baseStats = result;
        currentStats = result.Clone();

        UpdateUI();
    }
}
