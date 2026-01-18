using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Unit unit;
    private List<EnemyCharacter> enemies;
    private int currentEnemyIndex = 0;
    private TurnManager turnManager;

    private void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        unit = GetComponent<Unit>();
        enemies = EnemyManager.Instance.enemies;

        Debug.Log($"[EnemyController] 연결됨: 현재 적 수 = {enemies.Count}");
    }

    public IEnumerator EnemyTurnRoutine()
    {
        while (currentEnemyIndex < enemies.Count)
        {
            EnemyCharacter enemy = enemies[currentEnemyIndex];

            if (enemy != null && enemy.CurrentStats.HP > 0)
            {
                // [수정] 공격 시작 전, 적이 지연 상태인지 가장 먼저 확인합니다.
                if (enemy.ShouldSkipTurn)
                {
                    Debug.Log($"[EnemyTurn] {enemy.name}은 지연 상태입니다. 연출과 공격을 스킵합니다.");

                    // Attacking 코루틴을 실행하여 내부에서 지연 스택을 차감하도록 합니다.
                    yield return StartCoroutine(enemy.Attacking());

                    // 지연 상태이므로 아래의 공격 연출/데미지 로직을 실행하지 않고 다음 적으로 넘어갑니다.
                    currentEnemyIndex++;
                    continue;
                }

                // --- 지연 상태가 아닐 때만 아래 로직 실행 ---

                PlayerCharacter[] allPlayers = FindObjectsOfType<PlayerCharacter>();
                List<PlayerCharacter> alivePlayers = new List<PlayerCharacter>();

                foreach (var player in allPlayers)
                    if (player.CurrentStats.HP > 0)
                        alivePlayers.Add(player);

                if (alivePlayers.Count == 0)
                {
                    Debug.Log("[EnemyTurn] 공격할 플레이어가 없음!");
                    break;
                }

                PlayerCharacter targetPlayer = alivePlayers[Random.Range(0, alivePlayers.Count)];
                Debug.Log($"[EnemyTurn] {enemy.name} 공격 시도 -> {targetPlayer.name}");

                // 1. 공격 내부 로직 실행 (출혈 등)
                yield return StartCoroutine(enemy.Attacking());

                // 2. 공격 연출 실행 (돌진)
                AttackMotion attackMotion = enemy.GetComponent<AttackMotion>();
                if (attackMotion != null)
                {
                    attackMotion.PlayAttack(targetPlayer.transform);
                }

                // 3. 데미지 적용 (연출에 맞춰 0.4초 뒤 실행)
                DOVirtual.DelayedCall(0.4f, () => {
                    if (targetPlayer != null)
                    {
                        targetPlayer.TakeDamage(enemy.CurrentStats.ATK);
                        Debug.Log($"[EnemyTurn] {enemy.name}가 {targetPlayer.name}에게 {enemy.CurrentStats.ATK} 데미지를 입혔습니다.");
                    }
                });

                // 4. MP 회복 및 후처리
                enemy.CurrentStats.MP = Mathf.Min(enemy.BaseStats.MP, enemy.CurrentStats.MP + 20f);
                enemy.UpdateUI();

                yield return new WaitForSeconds(1.0f); // 다음 행동 전 대기 시간
            }

            currentEnemyIndex++;
        }

        // 모든 적 행동 종료 후 플레이어 턴으로 전환
        StageManager.Instance.currentTurnState = TurnState.PLAYER;
        currentEnemyIndex = 0;
        turnManager.StartPlayerTurn();
        Debug.Log("[EnemyTurn] 모든 적 공격 종료 -> 플레이어 턴으로 전환");
    }

    public void Act()
    {
        enemies = EnemyManager.Instance.enemies;

        if (enemies == null || enemies.Count == 0)
        {
            Debug.LogWarning("[EnemyController] 적 리스트가 비어 있습니다!");
            return;
        }

        Debug.Log($"[EnemyController] Act() 호출 - 적 {enemies.Count}명 행동 시작");
        StartCoroutine(EnemyTurnRoutine());
    }
}