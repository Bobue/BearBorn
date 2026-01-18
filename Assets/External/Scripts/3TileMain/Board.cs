using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject Tile; //백그라운드에 깔 배경타일
    private Transform characterTilesTransform;//타일을 둘 위치(씬내에서 '이름으로' 찾아서 위치 받아옴)

   [SerializeField] private Tiles[] C_tiles; // 캐릭터 타일 프리팹 배열

    //근데 인스펙터에서 설정하는게 빠를거임
    public int width = 7;//타일 가로 몇개
    public int height = 5;//타일 세로 몇개

    public GameObject arrowTilePrefab;
    public GameObject shieldTilePrefab;


    public Tiles[,] C_tilesMap;

    [SerializeField] private bool _isComboProcess = false;
    [SerializeField] private int _comboCount = 0;

    public bool isComboProcess
    {
        get => _isComboProcess;
        set => _isComboProcess = value;
    }

    public int comboCount
    {
        get => _comboCount;
        set => _comboCount = value;
    }


    public event System.Action OnEndAttack;
    public event System.Action<List<C_TileType>> OnTilesMatched; // 새 이벤트 추가

    private bool isBoardInitialized = false;


    void Start()
    {
        characterTilesTransform = transform.Find("CharacterTiles");//CharacterTiles의 위치데이터를 받아 저장
        if (!characterTilesTransform)//그런 이름의 오브젝트가 없으면 직접 생성해서 부여함.
        {
            GameObject newObj = new GameObject("CharacterTiles");
            newObj.transform.SetParent(transform, false);
            characterTilesTransform = newObj.transform;
        }

        if (StageDataTransfer.width > 0 && StageDataTransfer.height > 0)//만약 이전 스테이지에서 지정해놓은 변수가 있다면 그걸로 보드 생성
        {
            width = StageDataTransfer.width;
            height = StageDataTransfer.height;
        }
        if (StageDataTransfer.C_tiles != null && StageDataTransfer.C_tiles.Length > 1)
        {
            C_tiles = StageDataTransfer.C_tiles;
        }
    }

    public void AddCTile(Tiles tilePrefab)//타일을 추가, 현재는 특수타일의 경우 사용
    {
        if (tilePrefab == null)
        {
            Debug.LogWarning("tilePrefab이 null입니다!");
            return;
        }

        List<Tiles> tempList = new List<Tiles>(C_tiles);
        tempList.Add(tilePrefab);
        C_tiles = tempList.ToArray();

        Debug.Log($"{tilePrefab.TileType} 타일이 C_tiles 배열에 추가되었습니다!");
    }

    public void GenerateBoard()//보드배경생성
    {
        if (isBoardInitialized) return;
        if (C_tiles == null || C_tiles.Length < 3)//tile의 프리팹이 없거나 3개이하면 생성중단
        {
            Debug.LogError("타일 프리팹이 부족하여 생성중단");
            return;
        }
        Transform bgTilesTransform = transform.Find("BackgroundTiles");//아까 캐릭터타일과 마찬가지의 플로우
        if (!bgTilesTransform)
        {
            GameObject newObj = new GameObject("BackgroundTiles");
            newObj.transform.SetParent(transform, false);
            bgTilesTransform = newObj.transform;
        }

        C_tilesMap = new Tiles[width, height];//2차원 배열에 타일들의 사이즈를 지정
        Vector2 centerPos = GetCenterPos();//0,0기준으로 가운데로 보드의 중앙값을 맞추려고 넣은 코드

        for (int x = 0; x < width; x++)//여기서 타일을 다 생성함
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x - centerPos.x, y - centerPos.y);//0~도달점까지 centerPos를 각각 빼기
                Instantiate(Tile, pos, Quaternion.identity, bgTilesTransform);//bgTilesTransform을 부모로 오브젝트를 생성
                CreateRandomC_tile(x, y, pos, true);//타일생성 메서드
            }
        }
        
        Debug.Log("보드 생성 완료!");
    }

    private Tiles CreateRandomC_tile(int x, int y, Vector2 pos, bool checkMatch)//타일 단일 생성
    {
        int index = Random.Range(0, C_tiles.Length);//넣을 랜덤 타일을 지정

        if (checkMatch)//언제나 true임
        {
            while (MatchHorizontally(x, y, index) || MatchVertically(x, y, index))//현재 지정된 초기좌표가 가로세로로 매치가 발생하는지 확인
            {
                index = Random.Range(0, C_tiles.Length);//매치가 발생하면 다시 랜덤으로 좌표를 재지정함(매치가 발생안될때까지 반복)
            }
        }

        Tiles tiles = Instantiate(C_tiles[index], pos, Quaternion.identity, characterTilesTransform);//최종 좌표에 타일오브젝트 생성
        tiles.Init(x, y);//객체에 좌표저장
        C_tilesMap[x, y] = tiles;//2차원배열에도 타일배치를 저장
        return tiles;//생성 객체 반환
    }

    private bool MatchHorizontally(int x, int y, int index)//x좌표 매치타일 여부
    {
        if (x > 1)
        {
            if (C_tilesMap[x - 1, y].TileType == C_tiles[index].TileType &&
                C_tilesMap[x - 2, y].TileType == C_tiles[index].TileType)
                return true;
        }
        return false;
    }

    private bool MatchVertically(int x, int y, int index)//y좌표 매치타일 여부
    {
        if (y > 1)
        {
            if (C_tilesMap[x, y - 1].TileType == C_tiles[index].TileType &&
                C_tilesMap[x, y - 2].TileType == C_tiles[index].TileType)
                return true;
        }
        return false;
    }

    private Vector2 GetCenterPos()
    {
        //return new Vector2((width - 4), (height - 2.4f));
        return new Vector2((width / 2f) - 0.5f, (height / 2f) - 0.5f);
    }

    public void StartRemoveTilesRoutine()
    {
        StartCoroutine(RemoveTilesRoutine());
    }


    IEnumerator RemoveTilesRoutine()
    {
        MatchChekcing matchChekcing = FindObjectOfType<MatchChekcing>();

        for (int i = 0; i < 200; i++)
        {
            RemoveMatchedTiles(matchChekcing);
            yield return new WaitForSeconds(0.2f);

            DropTiles();
            yield return new WaitForSeconds(0.35f);

            FillTiles();
            yield return new WaitForSeconds(0.35f);

            var matchedTypes = matchChekcing.CheckAllMatches();

            if (matchedTypes.Count > 0)
            {
                Debug.Log($"{comboCount}콤보째 새로운 매치 발생!");
                foreach (var type in matchedTypes)
                    Debug.Log($"매치된 타입: {type}");

                // TileController로 매치 결과 전달
                OnTilesMatched?.Invoke(matchedTypes);
            }
            else
            {
                EndAttack();
                break;
            }
        }
    }

    private void DropTiles()
    {
        int emptySpace;
        Vector2 centerPos = GetCenterPos();
        float moveDuration = 0.2f;

        for (int x = 0; x < width; x++)
        {
            emptySpace = 0;
            for (int y = 0; y < height; y++)
            {
                if (C_tilesMap[x, y] == null)
                    emptySpace++;
                else if (emptySpace > 0)
                {
                    Tiles tiles = C_tilesMap[x, y];
                    C_tilesMap[x, y] = null;
                    C_tilesMap[x, y - emptySpace] = tiles;
                    tiles.y -= emptySpace;

                    Vector2 pos = new Vector2(x - centerPos.x, y - centerPos.y - emptySpace);
                    tiles.transform.DOMove(pos, moveDuration);
                }
            }
        }
    }

    private void FillTiles()
    {
        float moveDuration = 0.25f;
        Vector2 centerPos = GetCenterPos();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (C_tilesMap[x, y] == null)
                {
                    Vector2 initPos = new Vector2(x - centerPos.x, y - centerPos.y + height);
                    Tiles tile = CreateRandomC_tile(x, y, initPos, false);

                    Vector2 pos = new Vector2(x - centerPos.x, y - centerPos.y);
                    tile.transform.DOMove(pos, moveDuration);
                }
            }
        }
    }
    public void ClearBoard()
    {
        // 1) BackgroundTiles 오브젝트들 삭제
        Transform bgTilesTransform = transform.Find("BackgroundTiles");
        if (bgTilesTransform != null)
        {
            for (int i = bgTilesTransform.childCount - 1; i >= 0; i--)
                Destroy(bgTilesTransform.GetChild(i).gameObject);
        }

        // 2) CharacterTiles 오브젝트들 삭제
        if (characterTilesTransform != null)
        {
            for (int i = characterTilesTransform.childCount - 1; i >= 0; i--)
                Destroy(characterTilesTransform.GetChild(i).gameObject);
        }

        // 3) 내부 데이터 초기화
        C_tilesMap = new Tiles[width, height];
        isBoardInitialized = false;

        // 추가
        _isComboProcess = false;
        _comboCount = 0;

        Debug.Log("Board cleared!");
    }



    private void RemoveMatchedTiles(MatchChekcing matchChekcing)
    {
        foreach (Tiles tiles in matchChekcing.tileList)
        {
            C_tilesMap[tiles.x, tiles.y] = null;
            tiles.Remove();
        }

        _comboCount++;
        matchChekcing.tileList.Clear();
    }

    void EndAttack()
    {
        isComboProcess = false;
        OnEndAttack?.Invoke();
    }
}
