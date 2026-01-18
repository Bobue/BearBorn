using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MonsterDataLoader : MonoBehaviour
{
    public static MonsterDataLoader Instance;

    [Header("Monster Database")]
    public List<MonsterData> monsterDatabase;

    [Header("Google Sheet CSV URL")]
    public string monsterSheetUrl;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(LoadMonsterData());
    }

    IEnumerator LoadMonsterData()
    {
        UnityWebRequest req = UnityWebRequest.Get(monsterSheetUrl);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("몬스터 시트 로드 실패");
            yield break;
        }

        ParseCSV(req.downloadHandler.text);
    }

    void ParseCSV(string csv)
    {
        string[] lines = csv.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 0은 헤더
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] cols = lines[i].Split(',');

            int monsterId = int.Parse(cols[0]);
            int hp = int.Parse(cols[2]);
            int atk = int.Parse(cols[3]);
            int def = int.Parse(cols[4]);
            int mp = int.Parse(cols[5]);

            MonsterData data = monsterDatabase.Find(m => m.monsterID == monsterId);
            if (data == null)
            {
                Debug.LogWarning($"MonsterData 없음: ID {monsterId}");
                continue;
            }

            // 🔥 여기서 덮어쓴다
            data.baseStats.HP = hp;
            data.baseStats.ATK = atk;
            data.baseStats.DEF = def;
            data.baseStats.MP = mp;
        }

        Debug.Log("몬스터 데이터 로드 완료");
    }
}
