using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageDataTransfer
{
    // ===== 현재 선택된 스테이지 정보 =====
    public static int currentWorld = 1;
    public static int currentStage = 1;


    // ===== 전투 데이터 =====
    public static int wavesCount = 0; // 웨이브 횟수
    public static List<GameObject[]> monsterTypesPerWave = new();
    public static GameObject[] allyTypes = new GameObject[3];
    public static int stageExp = 0;

    // ===== 보드 테스트 데이터 =====
    public static Tiles[] C_tiles;
    public static int width, height;

    public static void ClearData()
    {
        currentWorld = 1;
        currentStage = 1;

        wavesCount = 0;
        monsterTypesPerWave.Clear();
        allyTypes = new GameObject[3];
        stageExp = 0;

        C_tiles = null;
        width = height = 0;
    }
}

