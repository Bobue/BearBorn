using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/StageData")]
public class StageData : ScriptableObject
{
    [Header("Stage ID")]
    public int worldIndex;
    public int stageIndex;

    [Header("Battle Into")]
    public List<MonsterWaveSetting> wavesSettings = new();
    public GameObject[] allyTypes = new GameObject[3];
    public int stageExp;

    [Header("Map")]
    public Tiles[] tiles;
    public int width;
    public int height;
}
