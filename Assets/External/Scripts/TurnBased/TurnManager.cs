using DG.Tweening;
using System;
using UnityEngine;



public class TurnManager : MonoBehaviour
{
    private EnemyController enemyController;
    public GameObject PlayerNoti;
    public GameObject EnemyNoti;
    private void Start()
    {
        StageManager.Instance.StartPlayerTurn += StartPlayerTurn;
        StageManager.Instance.StartEnemyTurn += StartEnemyTurn;
        // 적 컨트롤러 생성
        enemyController = FindObjectOfType<EnemyController>();
        if (enemyController == null)
            Debug.LogError("EnemyController를 찾을 수 없습니다!");

    }

    public void StartPlayerTurn()
    {
        Noti();
    }

    // 플레이어 턴 종료 → ENEMY
    public void StartEnemyTurn()
    {
        if(StageManager.Instance.currentTurnState == TurnState.ENEMY && enemyController != null)
        {
            Noti();
            enemyController.Act();
        }
    }

    private void Noti()
    {
        if(StageManager.Instance.currentTurnState == TurnState.PLAYER)
        {
            Debug.Log("플레이어 턴 시작!");
            PlayerNoti.SetActive(true);
            MySoundManager.Instance.PlayTurnChange();
            Sequence notiSequence = DOTween.Sequence();
            notiSequence.AppendInterval(1f);
            notiSequence.AppendCallback(() =>
            {
                PlayerNoti.SetActive(false);
            });
        }
        
        else if (StageManager.Instance.currentTurnState == TurnState.ENEMY)
        {
            Debug.Log("적 턴 시작!");
            EnemyNoti.SetActive(true);
            MySoundManager.Instance.PlayTurnChange();
            Sequence notiSequence = DOTween.Sequence();
            notiSequence.AppendInterval(1f);
            notiSequence.AppendCallback(() =>
            {
                EnemyNoti.SetActive(false);
            });
        }
    }

}
