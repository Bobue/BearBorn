using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager1 : MonoBehaviour
{
    private int maxAction = 3;
    private int currentAction = 0;

    public bool IsPlayerTurn { get; private set; } = true;
    public void StartTurn()
    {
        currentAction = 0;
        IsPlayerTurn = true;
    }

    public bool TryAction()
    {
        if (!IsPlayerTurn) return false;

        currentAction++;

        if (currentAction >= maxAction)
        {
            EndTurn();
            return true;
        }
        return false;
    }

    void EndTurn()
    {
        IsPlayerTurn = false;
    }
}
