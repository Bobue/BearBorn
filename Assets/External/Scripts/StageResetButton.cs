using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageResetButton : MonoBehaviour
{
    public void ResetAllStages()
    {
        // 1️⃣ PlayerPrefs 초기화
        PlayerPrefs.DeleteKey("CLEARED_WORLD");
        PlayerPrefs.DeleteKey("CLEARED_STAGE");
        PlayerPrefs.Save();

        // 2️⃣ StageProgress 초기화
        // 내부 기본값 사용: ClearedWorld = 1, ClearedStage = 0
        // 필요하다면 메모리 변수도 초기화
        StageDataTransfer.currentWorld = 1;
        StageDataTransfer.currentStage = 1;
        StageDataTransfer.monsterTypesPerWave.Clear();
        StageDataTransfer.allyTypes = null;
        StageDataTransfer.stageExp = 0;
        StageDataTransfer.C_tiles = null;
        StageDataTransfer.width = 0;
        StageDataTransfer.height = 0;

        Debug.Log("스테이지 초기화 완료");
        Debug.Log($"StageDataTransfer: {StageDataTransfer.currentWorld}-{StageDataTransfer.currentStage}");

        // 3️⃣ 스테이지 선택 씬으로 이동
        SceneManager.LoadScene("RadgaReactor"); // StageSelect 씬 이름에 맞춰 수정
    }
}
