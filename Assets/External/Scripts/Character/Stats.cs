using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public float HP;//체력
    public float MP;//마나
    public float ATK;//공격력
    public float DEF;//방어력
    public float LUK;//행운
    public float SHD;//쉴드
    public float LV;//레벨
    public float LVPoint;//레벨 스탯 포인트
    public float Exp;//경험치
    public float ExpToNextLevel;//다음 레벨업을 위한 경험치양


    public Stats(float hp, float mp, float atk, float def, float luk, float shd, float lV, float lVPoint, float exp, float expToNextLevel)
    {
        HP = hp;
        MP = mp;
        ATK = atk;
        DEF = def;
        LUK = luk;
        SHD = shd;
        LV = lV;
        LVPoint = lVPoint;
        Exp = exp;
        ExpToNextLevel = expToNextLevel;
    }

    public Stats Clone()
    {
        return new Stats(HP, MP, ATK, DEF, LUK, SHD, LV, LVPoint, Exp, ExpToNextLevel);
    }
}

