using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TileController : MonoBehaviour
{
    private Board board;
    private MatchChekcing matchChecking;

    private Vector2 mouseDownPosition;
    private Vector2 mouseUpPosition;

    private Tiles FirstCTile;
    private Vector2 FirstCTilePos;

    private int shieldMatchCount = 0;
    private int arrowMatchCount = 0;

    private Tiles SecondCTile;
    private Vector2 SecondCTilePos;
    private int currentWeapon;

    private float moveDuration = 0.25f;

    [SerializeField]
    private int attackCount;
    [SerializeField]
    private GameObject shield;
    [SerializeField]
    private GameObject arrow;
    [SerializeField] private Color inactiveColor = new Color32(0x5B, 0x5B, 0x5B, 0xFF);
    [SerializeField] private Color activeColor = Color.white;
    public float[] MpPlus; // 0 : verka, 1: marshika 2: chirr


    [SerializeField]
    private Image[] arrowCounts = new Image[3];

    [SerializeField]
    private Image[] shieldCounts = new Image[2];


    public event System.Action OnTurnEnd;

    void Start()
    {
        MpPlus = new float[3]; // verka, marshika, chirr
        attackCount = 0;
        board = FindObjectOfType<Board>();
        matchChecking = FindObjectOfType<MatchChekcing>();

        board.OnEndAttack += HandleEndAttack;
        board.OnTilesMatched += HandleMatchedTypes; // 이벤트 추가
    }

    void Update()
    {
        if (StageManager.Instance.currentTurnState != TurnState.PLAYER)
            return;
        if (SkillManager.Instance != null && SkillManager.Instance.IsSkillPlaying)
            return;
        if (Input.GetMouseButtonDown(0) && !board.isComboProcess)
        {
            mouseDownPosition = Input.mousePosition;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseDownPosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                FirstCTile = hit.collider.GetComponent<Tiles>();
                FirstCTilePos = FirstCTile.transform.position;
            }
        }

        if (Input.GetMouseButtonUp(0) && !board.isComboProcess)
        {
            mouseUpPosition = Input.mousePosition;

            if (Vector2.Distance(mouseDownPosition, mouseUpPosition) < 30f)
            {
                board.isComboProcess = false;
                ResetTiles();
                return;
            }

            float angle = Mathf.Atan2(mouseUpPosition.y - mouseDownPosition.y, mouseUpPosition.x - mouseDownPosition.x) * Mathf.Rad2Deg;
            SecondCTile = GetSecondTile(angle);

            if (FirstCTile != null && SecondCTile != null)
            {
                MySoundManager.Instance.PlayDrop();
                SecondCTilePos = SecondCTile.transform.position;
                board.isComboProcess = true;
                board.comboCount = 0;

                Sequence slideSequence = DOTween.Sequence();
                slideSequence.Append(FirstCTile.transform.DOMove(SecondCTilePos, moveDuration));
                slideSequence.Join(SecondCTile.transform.DOMove(FirstCTilePos, moveDuration));

                slideSequence.OnComplete(() =>
                {
                    SwapTiles();
                    StartCoroutine(CheckMatchRoutine());
                });
            }
            else
            {
                ResetTiles();
            }
        }
    }
    void ResetIcons(Image[] icons)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].gameObject.SetActive(true); // 항상 활성
            icons[i].color = inactiveColor;
        }
    }

    void UpdateIcons(Image[] icons, int activeCount)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].color = (i < activeCount) ? activeColor : inactiveColor;
        }
    }
    public void WhatWeapon(int weaponIndex)
    {
        currentWeapon = weaponIndex;
        ActiveWeapon();
    }

    void ActiveWeapon()
    {
        if (currentWeapon == 0) // Arrow
        {
            arrow.SetActive(true);
            shield.SetActive(false);

            arrowMatchCount = 0;
            ResetIcons(arrowCounts);
        }
        else // Shield
        {
            shield.SetActive(true);
            arrow.SetActive(false);

            shieldMatchCount = 0;
            ResetIcons(shieldCounts);
        }
    }



    public void resetAttackCount()
    {
        attackCount = 0;
        Debug.Log($"attack count {attackCount}으로 초기화");
    }
    private void ResetTiles()
    {
        FirstCTile = null;
        FirstCTilePos = Vector2.zero;
        SecondCTile = null;
        SecondCTilePos = Vector2.zero;
    }

    private void SwapTiles()
    {
        int tempx = FirstCTile.x;
        int tempy = FirstCTile.y;

        FirstCTile.x = SecondCTile.x;
        FirstCTile.y = SecondCTile.y;

        SecondCTile.x = tempx;
        SecondCTile.y = tempy;

        board.C_tilesMap[FirstCTile.x, FirstCTile.y] = FirstCTile;
        board.C_tilesMap[SecondCTile.x, SecondCTile.y] = SecondCTile;
    }

    private Tiles GetSecondTile(float angle)
    {
        if (FirstCTile == null)
            return null;

        if (angle > -45 && angle < 45 && FirstCTile.x < board.width - 1)
            return board.C_tilesMap[FirstCTile.x + 1, FirstCTile.y];
        else if (angle > 45 && angle <= 135 && FirstCTile.y < board.height - 1)
            return board.C_tilesMap[FirstCTile.x, FirstCTile.y + 1];
        else if ((angle > 135 || angle <= -135) && FirstCTile.x > 0)
            return board.C_tilesMap[FirstCTile.x - 1, FirstCTile.y];
        else if (angle > -135 && angle <= -45 && FirstCTile.y > 0)
            return board.C_tilesMap[FirstCTile.x, FirstCTile.y - 1];

        return null;
    }

    IEnumerator CheckMatchRoutine()
    {
        yield return new WaitForSeconds(moveDuration * 2f);

        List<C_TileType> matchedTypes = matchChecking.CheckAllMatches();

        if (matchedTypes.Count == 0)
        {
            MySoundManager.Instance?.PlayDropMismatch();
            FirstCTile.transform.DOMove(FirstCTilePos, moveDuration).OnComplete(() =>
            {
                board.isComboProcess = false;
            });
            SecondCTile.transform.DOMove(SecondCTilePos, moveDuration);
            SwapTiles();
            board.comboCount = 0;
        }
        else
        {
            MySoundManager.Instance?.PlayDropMatch();
            board.StartRemoveTilesRoutine();
            HandleMatchedTypes(matchedTypes); // 첫 매치도 처리
        }

        ResetTiles();
    }

    private void HandleMatchedTypes(List<C_TileType> matchedTypes)
    {
        foreach (C_TileType type in matchedTypes.ToList())
        {
            if (type == C_TileType.Arrow)
            {
                arrowMatchCount = Mathf.Min(arrowMatchCount + 1, arrowCounts.Length);
                UpdateIcons(arrowCounts, arrowMatchCount);

                Debug.Log($"Arrow 타일 매치 {arrowMatchCount} / {arrowCounts.Length}");

                if (arrowMatchCount >= arrowCounts.Length)
                {
                    float damage = 5f;

                    SkillManager.Instance.currentAttackType = SkillManager.TempAttackType.Arrow;
                    foreach (var enemy in EnemyManager.Instance.enemies)
                    {
                        AttackMotion motion = enemy.GetComponent<AttackMotion>();
                        if (motion != null)
                            motion.PlayAttack(enemy.transform);

                        enemy.TakeDamage(damage);
                    }
                    SkillManager.Instance.currentAttackType = SkillManager.TempAttackType.None;
                    // ⭐ 풀스택 상태를 잠깐 보여준 뒤 리셋
                    DOVirtual.DelayedCall(0.1f, () =>
                    {
                        arrowMatchCount = 0;
                        ResetIcons(arrowCounts);
                    });
                }


                continue;
            }

            if (type == C_TileType.Shield)
            {
                shieldMatchCount = Mathf.Min(shieldMatchCount + 1, shieldCounts.Length);
                UpdateIcons(shieldCounts, shieldMatchCount);

                Debug.Log($"Shield 타일 매치 {shieldMatchCount} / {shieldCounts.Length}");

                if (shieldMatchCount >= shieldCounts.Length)
                {
                    float shieldAmount = 5f;

                    foreach (var player in PlayerManager.Instance.playerCharacters)
                    {
                        player.ChangeShield(shieldAmount);
                    }

                    Debug.Log($"Shield 효과 발동! 아군 전체 쉴드 +{shieldAmount}");

                    // ⭐ 여기서도 동일하게 딜레이
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        shieldMatchCount = 0;
                        ResetIcons(shieldCounts);
                    });
                }


                continue;
            }
            if (type == C_TileType.Verka)
            {
                PlayerCharacter ally = PlayerManager.Instance.GetCharacterByTileType(type);
                EnemyCharacter target = EnemyManager.Instance.GetRandomEnemy();

                if (ally == null) continue;

                // 1. 공격 애니메이션 실행
                AttackMotion attackMotion = ally.GetComponent<AttackMotion>();
                if (attackMotion != null && target != null)
                {
                    attackMotion.PlayAttack(target.transform);
                }

                // 2.데미지 적용을 애니메이션 타격 시점(예: 0.4초)에 맞춰 지연 실행
                if (target != null)
                {
                    float damage = ally.CurrentStats.ATK;
                    Debug.Log($"[CAPTURE] damage = {damage}");
                    DOVirtual.DelayedCall(0.5f, () => {
                        Debug.Log($"[HIT] ATK now = {ally.CurrentStats.ATK}");
                        if (target != null) target.TakeDamage(damage);
                    });
                    Debug.Log($"{type} 공격 명령! (0.4초 후 데미지)");
                }

                // MP 증가 및 UI 업데이트는 즉시 처리 (유저 피드백)
                ally.CurrentStats.MP = Mathf.Min(ally.BaseStats.MP, ally.CurrentStats.MP + MpPlus[0]);
                Debug.Log($"{type} MP +{MpPlus[0]} (현재 {ally.CurrentStats.MP}/{ally.BaseStats.MP})");
                ally.UpdateUI();
            }
            if (type == C_TileType.Chirr)
            {
                PlayerCharacter ally = PlayerManager.Instance.GetCharacterByTileType(type);
                EnemyCharacter target = EnemyManager.Instance.GetRandomEnemy();

                if (ally == null) continue;

                // 1. 공격 애니메이션 실행
                AttackMotion attackMotion = ally.GetComponent<AttackMotion>();
                if (attackMotion != null && target != null)
                {
                    attackMotion.PlayAttack(target.transform);
                }

                // 2.데미지 적용을 애니메이션 타격 시점(예: 0.4초)에 맞춰 지연 실행
                if (target != null)
                {
                    float damage = ally.CurrentStats.ATK;
                    DOVirtual.DelayedCall(0.5f, () => {
                        if (target != null) target.TakeDamage(damage);
                    });
                    Debug.Log($"{type} 공격 명령! (0.4초 후 데미지)");
                }

                // MP 증가 및 UI 업데이트는 즉시 처리 (유저 피드백)
                ally.CurrentStats.MP = Mathf.Min(ally.BaseStats.MP, ally.CurrentStats.MP + MpPlus[2]);
                ally.UpdateUI();
            }
            if (type == C_TileType.Marshika)
            {
                PlayerCharacter marshika =
                    PlayerManager.Instance.GetCharacterByTileType(C_TileType.Marshika);

                if (marshika == null)
                {
                    Debug.LogWarning("Marshika 캐릭터를 찾을 수 없습니다!");
                    continue;
                }

                float healAmount = marshika.CurrentStats.ATK;

                // 아군 전체 회복
                foreach (var ally in PlayerManager.Instance.playerCharacters)
                {
                    ally.CurrentStats.HP = Mathf.Min(
                        ally.BaseStats.HP,
                        ally.CurrentStats.HP + healAmount
                    );

                    ally.UpdateUI();
                }

                // ⭐ Marshika 본인 MP +10
                marshika.CurrentStats.MP = Mathf.Min(
                    marshika.BaseStats.MP,
                    marshika.CurrentStats.MP + MpPlus[1]
                );
                marshika.UpdateUI();

                Debug.Log($"Marshika 효과! 아군 전체 체력 +{healAmount}, Marshika MP +10");
                continue;
            }


            // 일반 타일 (Normal)
            if (type == C_TileType.Normal)
            {
                foreach (var ally in PlayerManager.Instance.playerCharacters)
                {
                    ally.CurrentStats.MP = Mathf.Min(ally.BaseStats.MP, ally.CurrentStats.MP + 10f);
                    ally.UpdateUI();
                    Debug.Log($"{ally.name} MP +10 (현재 {ally.CurrentStats.MP}/{ally.BaseStats.MP})");
                }
                continue;
            }

        }
    }



    void HandleEndAttack()
    {
        ++attackCount;
        if (attackCount >= 3)
        {
            EndTurn();
        }
    }

    public void StartTurn()
    {
        board.isComboProcess = false;
    }

    void EndTurn()
    {
        attackCount = 0;
        OnTurnEnd?.Invoke();
    }
    public void RebindBoard()
    {
        board = FindObjectOfType<Board>();
        matchChecking = FindObjectOfType<MatchChekcing>();

        board.OnEndAttack -= HandleEndAttack;
        board.OnEndAttack += HandleEndAttack;

        board.OnTilesMatched -= HandleMatchedTypes;
        board.OnTilesMatched += HandleMatchedTypes;

        Debug.Log("TileController: board reference refreshed");
    }

}
