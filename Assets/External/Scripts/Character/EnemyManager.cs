using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public List<EnemyCharacter> enemies = new List<EnemyCharacter>();
    public event Action OnAllEnemiesDead;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 랜덤 적 선택
    public EnemyCharacter GetRandomEnemy()
    {
        if (enemies.Count == 0) return null;
        return enemies[UnityEngine.Random.Range(0, enemies.Count)];
    }

    // 적 제거
    public void RemoveEnemy(EnemyCharacter enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);

        CheckAllEnemiesDead();
    }

    // 모든 적 사망 시 씬 전환
    private void CheckAllEnemiesDead()
    {
        if (enemies.Count == 0)
        {
            Debug.Log("모든 적 처치 완료! 씬 전합니다.");
            OnAllEnemiesDead?.Invoke();
        }
    }
}
