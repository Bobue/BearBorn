using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlayerHolder : MonoBehaviour
{
    public static CharacterPlayerHolder Instance { get; private set; }

    private Dictionary<string, Stats> characterStatsDict = new Dictionary<string, Stats>();

    private void Awake()
    {
        if(Instance !=  null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeCharacters();
    }

    //최초 캐릭터 데이터 세팅
    private void InitializeCharacters()
    {
        //베르카
        characterStatsDict["Verka"] = new Stats(100, 200, 20, 10, 10, 0, 1, 0, 0, 100);
        //마르쉬카
        characterStatsDict["Marshika"] = new Stats(150, 200, 5, 20, 10, 0, 1, 0, 0, 100);
        //치르
        characterStatsDict["Chirr"] = new Stats(75, 200, 30, 10, 10, 0, 1, 0, 0, 100);
    }

    public Stats GetStats(string characterName)//원본스탯 클론 반환
    {
        if(!characterStatsDict.ContainsKey(characterName))
        {
            Debug.LogError($"{characterName} 캐릭터의 스탯 정보를 찾을 수 없습니다.");
            return null;
        }

        return characterStatsDict[characterName].Clone();
    }

    public void Setstats(string characterName, Stats newStats)
    {
        if(!characterStatsDict.ContainsKey(characterName))
        {
            Debug.LogError($"{characterName} 라는 넘은 없습니다.");
            return;
        }

        characterStatsDict[characterName] = newStats.Clone();
    }

    public void SaveFromCharacter(CharacterBase character)
    {
        if (character == null) return;

        string name = character.name;

        if (!characterStatsDict.ContainsKey(name))
        {
            Debug.LogError($"{name} 라는 넘은 없어서 저장 못 했어요!");
            return;
        }
        characterStatsDict[name] = character.CurrentStats.Clone();//커렌트가 맞으려나
    }

    public void ApplyToCharacter(CharacterBase character)
    {
        if (character == null) return;
        string name = character.name;

        if(!characterStatsDict.ContainsKey(name))
        {
            Debug.LogError($"{name} 라는 넘은 없어서 스탯적용을 못 했어요!");
            return;
        }

        character.ApplyBaseStats(characterStatsDict[name]);
    }

}
