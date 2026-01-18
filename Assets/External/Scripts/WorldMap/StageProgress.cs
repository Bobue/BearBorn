using UnityEngine;

public static class StageProgress
{
    private const string KEY_WORLD = "CLEARED_WORLD";
    private const string KEY_STAGE = "CLEARED_STAGE";

    public static int ClearedWorld
    {
        get => PlayerPrefs.GetInt(KEY_WORLD, 1);
        set => PlayerPrefs.SetInt(KEY_WORLD, value);
    }

    public static int ClearedStage
    {
        get => PlayerPrefs.GetInt(KEY_STAGE, 0);
        set => PlayerPrefs.SetInt(KEY_STAGE, value);
    }

    public static bool IsStageUnlocked(int world, int stage)
    {
        if (world < ClearedWorld) return true;
        if (world == ClearedWorld && stage <= ClearedStage + 1) return true;
        return false;
    }

    public static bool IsStageCleared(int world, int stage)
    {
        if (world < ClearedWorld) return true;
        if (world == ClearedWorld && stage <= ClearedStage) return true;
        return false;
    }

    public static void ClearStage(int world, int stage, int maxStageInWorld)
    {
        if (world > ClearedWorld) return;

        if (stage < maxStageInWorld)
        {
            ClearedWorld = world;
            ClearedStage = stage;
        }
        else
        {
            ClearedWorld = world + 1;
            ClearedStage = 0;
        }
    }
}
