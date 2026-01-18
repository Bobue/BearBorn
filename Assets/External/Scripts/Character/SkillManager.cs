using System.Collections;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
    private Board board;

    private bool isTargetingMode = false;
    private bool isAllyTargeting = false;
    public PlayerCharacter activeSkillUser;
    private float skillValue;
    public GameObject vPopup;
    public GameObject cPopup;
    public GameObject mPopup;

    public bool IsSkillPlaying { get; private set; }//스킬 사용시 타일이동 멈춤


    public enum TempAttackType
    {
        None,
        Arrow
    }
    public TempAttackType currentAttackType = TempAttackType.None;


    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }


    public void ActivateTargetSelectMode(PlayerCharacter user, float value)
    {
        IsSkillPlaying = true;
        isTargetingMode = true;
        isAllyTargeting = false;
        activeSkillUser = user;
        skillValue = value;
        Debug.Log($"{user.name} 적 타겟팅 모드 활성화");
        StartCoroutine(ShowPopup(user));
    }

    public void ActivateAllyTargetSelectMode(PlayerCharacter user, float value)
    {
        IsSkillPlaying = true;
        isAllyTargeting = true;
        isTargetingMode = false;
        activeSkillUser = user;
        skillValue = value;
        Debug.Log($"{user.name} 아군 타겟팅 모드 활성화");
        StartCoroutine(ShowPopup(user));
    }

    public bool IsTargetSelectModeActive() => isTargetingMode;
    public bool IsAllyTargetSelectModeActive() => isAllyTargeting;

    public void ApplySkillToTarget(EnemyCharacter target)
    {
        if (!isTargetingMode || activeSkillUser == null) return;

        // 데미지 적용
        target.TakeDamage(skillValue);

        // 캐릭터별 특수 효과
        if (activeSkillUser is Verka)
        {
            target.DelayTurn(1);
        }
        else if (activeSkillUser is Chirr) // 클래스명이 Chirr
        {
            target.ApplyBleed(2, 0.03f);
        }
        else if (activeSkillUser is Marshika)
        {

        }

        FinishSkill();
    }

    public void ApplySkillToAlly(PlayerCharacter targetAlly)
    {
        if (!isAllyTargeting || activeSkillUser == null) return;

        // 수정: characterName 대신 name 사용
        Debug.Log($"[Heal Start] 대상: {targetAlly.name}, 시전자: {activeSkillUser.name}");

        if (activeSkillUser is Marshika)
        {
            float healAmount = targetAlly.BaseStats.HP * 0.1f;
            float beforeHP = targetAlly.CurrentStats.HP;

            targetAlly.CurrentStats.HP = Mathf.Min(targetAlly.BaseStats.HP, targetAlly.CurrentStats.HP + healAmount);
            targetAlly.TakeDamage(-Mathf.Min(targetAlly.BaseStats.HP, targetAlly.CurrentStats.HP + healAmount));
        

        // 수정: characterName 대신 name 사용
        Debug.Log($"[Marshika Heal] {targetAlly.name}: {beforeHP} -> {targetAlly.CurrentStats.HP} (회복량: {healAmount})");
        }
        else
        {
            targetAlly.CurrentStats.HP = Mathf.Min(targetAlly.BaseStats.HP, targetAlly.CurrentStats.HP + skillValue);
            // 수정: characterName 대신 name 사용
            Debug.Log($"[Normal Heal] {targetAlly.name}에게 {skillValue}만큼 힐 적용");
        }

        targetAlly.UpdateUI();
        FinishSkill();
    }

    public GameObject GetCurrentSkillEffectPrefab()
    {
        if(activeSkillUser is Verka)
            return Resources.Load<GameObject>("CharacterAttak/VerkaSkill/VerkaSkill");
        if(activeSkillUser is Chirr)
            return Resources.Load<GameObject>("CharacterAttak/ChirrSkill/ChirrSkill");
        if(activeSkillUser is Marshika)
            return Resources.Load<GameObject>("CharacterAttak/MarshikaSkill/MarshikaSkill");
        return null;
    }

    private IEnumerator ShowPopup(PlayerCharacter user)
    {
        GameObject popup = null;
        if (user is Verka)
            popup = vPopup;
        else if (user is Chirr)
            popup = cPopup;
        else if (user is Marshika)
            popup = mPopup;
        if (popup != null)
        {
            popup.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            popup.SetActive(false);
        }
    }

    private void FinishSkill()
    {
        IsSkillPlaying = false;
        isTargetingMode = false;
        isAllyTargeting = false;
        activeSkillUser = null;
        skillValue = 0f;
        Debug.Log("스킬 사용 완료 및 모드 초기화");
    }
}