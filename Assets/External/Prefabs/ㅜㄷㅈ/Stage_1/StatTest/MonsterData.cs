using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Monster")]
public class MonsterData : ScriptableObject
{
    public int monsterID;
    public string monsterName;

    public Stats baseStats;
}
