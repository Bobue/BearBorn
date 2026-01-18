using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    private Canvas worldCanvas;
    public GameObject damagePopupPrefab;

    [Header("타격 이펙트")]
    public GameObject playerEffectPrefab;
    public GameObject enemyEffectPrefab;
    public GameObject arrowHitEffect;

    private CharacterBase character;
    private bool ShieldBroken = false;

    private void Awake()
    {
        character = GetComponent<CharacterBase>();
        character.OnDamaged += OnDamaged;

        if(worldCanvas == null)
        {
            worldCanvas = GameObject.Find("DamageUICanvas").GetComponent<Canvas>();
        }
    }

    void OnDamaged(CharacterBase.DamageResult result)
    {
        Vector3 pos = transform.position + Vector3.up * 1.5f;


        if(result.hpDamage>0)
        {
            SpawnPopup(result.hpDamage, pos, Color.red);
        }
        if(result.shieldDamage>0)
        {
            SpawnPopup(result.shieldDamage, pos + Vector3.right * 0.3f, Color.cyan);
            ShieldBroken = true;
        }
        SpawnHitEffect(pos);
    }

    void SpawnPopup(float value, Vector3 pos, Color color)
    {
        var popup = Instantiate(damagePopupPrefab, worldCanvas.transform);
        popup.transform.position = pos;
        popup.GetComponent<DamagePopup>().Init(value, color);
    }

    void SpawnHitEffect(Vector3 pos)
    {
        GameObject effectPrefab = null;

        
        if(CompareTag("Enemy"))
        {
            if(SkillManager.Instance.activeSkillUser != null)
                effectPrefab = SkillManager.Instance?.GetCurrentSkillEffectPrefab();
            else
                effectPrefab = playerEffectPrefab;
        }
        else if(CompareTag("Player"))
        {
            if (SkillManager.Instance.activeSkillUser is Marshika && SkillManager.Instance.activeSkillUser != null)
            {
                effectPrefab = SkillManager.Instance?.GetCurrentSkillEffectPrefab();
            }
                
            else
                effectPrefab = enemyEffectPrefab;
        }
        arrowHitEffect = Resources.Load<GameObject>("CharacterAttak/Arrow/ArrowEffect");
        if (SkillManager.Instance.currentAttackType == SkillManager.TempAttackType.Arrow)
        {
            effectPrefab = arrowHitEffect;
            MySoundManager.Instance.PlayArrowFire();
        }
        if(ShieldBroken)
        {
            effectPrefab = Resources.Load<GameObject>("CharacterAttak/Shield/ShieldEffect");
            MySoundManager.Instance.PlayShieldBreak();
            ShieldBroken = false;
        }
        

        if (effectPrefab == null)
        {
            return;
        }
        var effect = Instantiate(effectPrefab, worldCanvas.transform);
        effect.transform.position = pos;
    }

    void SpawnShieldBreakEffect(Vector3 pos)
    {
        GameObject ShieldBreakEffect = Resources.Load<GameObject>("CharacterAttak/Shield/ShieldEffect");
        if (ShieldBreakEffect == null) return;

        Instantiate(ShieldBreakEffect, worldCanvas.transform).transform.position = pos ;
    }

    void OnDestroy()
    {
        character.OnDamaged -= OnDamaged;
    }
}
