using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterSelectData
{
    public static int SelectedCharacterIndex;
}

public static class  CharacterIdMapper
{
    private static readonly string[] indexToName =
    {
        "Verka",
        "Marshika",
        "Chirr"
    };

    public static string GetName(int index)
    {
        if (index < 0 || index >= indexToName.Length)
        {
            Debug.LogError("잘못된 인덱스임");
            return null;
        }
        return indexToName[index];
    }
}