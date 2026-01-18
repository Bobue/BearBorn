using System.Collections.Generic;
using UnityEngine;

public static class StageLoader
{
    public static void Load(
        int worldIndex,
        int stageIndex,
        List<MonsterWaveSetting> wavesSettings,
        GameObject[] stageAllyTypes,
        int stageExp,
        Tiles[] cTiles,
        int width,
        int height
    )
    {
        StageDataTransfer.currentWorld = worldIndex;
        StageDataTransfer.currentStage = stageIndex;

        StageDataTransfer.monsterTypesPerWave.Clear();
        foreach (var wave in wavesSettings)
            StageDataTransfer.monsterTypesPerWave.Add(wave.monsterTypes);

        StageDataTransfer.wavesCount =
            StageDataTransfer.monsterTypesPerWave.Count;

        StageDataTransfer.allyTypes = stageAllyTypes;
        StageDataTransfer.stageExp = stageExp;

        StageDataTransfer.C_tiles = cTiles;
        StageDataTransfer.width = width;
        StageDataTransfer.height = height;
    }
}
