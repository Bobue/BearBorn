using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MonsterWaveSetting
{
    public GameObject[] monsterTypes = new GameObject[3];
}

public class StageButton : MonoBehaviour
{
    [Header("Stage Index")]
    public int worldIndex;   // 1, 2, 3
    public int stageIndex;   // 1, 2, 3

    [Header("Sprite")]
    public SpriteRenderer stageRenderer; // SpriteRenderer가 붙어있는 오브젝트
    public Sprite lockedSprite;          // 잠긴 상태 이미지
    public Sprite unlockedSprite;        // 해금 상태 이미지

    [Header("Stage Data")]
    public List<MonsterWaveSetting> wavesSettings = new();
    public GameObject[] stageAllyTypes = new GameObject[3];
    public int stageExp;

    public Tiles[] C_tiles; // test
    public int width, height; // test

    void Start()
    {
        bool unlocked = StageProgress.IsStageUnlocked(worldIndex, stageIndex);

        // SpriteRenderer에 따라 스프라이트 교체
        if (stageRenderer != null)
            stageRenderer.sprite = unlocked ? unlockedSprite : lockedSprite;
    }

    public void LoadStage()
    {
        if (!StageProgress.IsStageUnlocked(worldIndex, stageIndex))
            return;

        if (!EnergyManager.Instance.ConsumeForStage())
        {
            Debug.Log("에너지가 부족합니다!");
            // 나중에 팝업 UI 연결
            return;
        }

        // ===== 스테이지 데이터 저장 =====
        StageDataTransfer.currentWorld = worldIndex;
        StageDataTransfer.currentStage = stageIndex;

        StageDataTransfer.monsterTypesPerWave.Clear();
        foreach (var wave in wavesSettings)
            StageDataTransfer.monsterTypesPerWave.Add(wave.monsterTypes);

        StageDataTransfer.wavesCount = StageDataTransfer.monsterTypesPerWave.Count;
        StageDataTransfer.allyTypes = stageAllyTypes;
        StageDataTransfer.stageExp = stageExp;

        StageDataTransfer.C_tiles = C_tiles;
        StageDataTransfer.height = height;
        StageDataTransfer.width = width;

        bool isFirstEnter = !StageProgress.IsStageCleared(1, 1);

        if (isFirstEnter)
        {
            SceneManager.LoadScene("CutScene");
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
    }

}
