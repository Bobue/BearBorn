using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum TurnState
{
    TILESELECT,
    POKER,
    PLAYER,
    ENEMY,
    CLEAR,
    DEFEAT
}
public class StageManager : Singleton<StageManager>
{
    public event System.Action StartPokerTurn;
    public event System.Action StartPlayerTurn;
    public event System.Action StartEnemyTurn;

    public TMP_Text WaveIndicator;
    public TMP_Text StageIndicator;
    public TMP_Text Indicator;
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    private bool isPaused = false;
    public GameObject pausePanel;

    private int currentCutsceneIndex = -1;
    private bool waitClearAfterCutscene = false;
    private bool waitEntryCutscene = false;

    private bool waitPokerAfterCutscene = false;

    public GameObject[] CutScene;
    public GameObject[] Tutorials;

    private float multiplier = 0;
    private int stageExp = 0;

    private Coroutine tutorialCoroutine;

    PokerGameManager pokerGameManager;
    TileController tileController;
    TileselectManager tileselectManager;
    PlayerManager playerManager;

    private int maxWaveCount = 0;        // 전체 웨이브 수
    private int currentWaveCount = 0;    // 현재 웨이브 인덱스
    private List<GameObject[]> monsterWaves;
    private GameObject[] allyPrefabs;
    private int currentWeapon; // 0이라면 arrow 1이라면 shield

    private C_TileType selectedTile;
    public TurnState _currentTurnState = TurnState.TILESELECT;
    public TurnState currentTurnState
    {
        get => _currentTurnState;
        set => _currentTurnState = value;
    }
    private void OnEnable()
    {
        Test.OnCutsceneFinished += OnCutsceneFinished;
    }

    private void OnDisable()
    {
        Test.OnCutsceneFinished -= OnCutsceneFinished;
    }

    private void Awake()
    {
        allyPrefabs = StageDataTransfer.allyTypes;
        InitializeAlly();

    }

    private void Start()
    {
        foreach (GameObject tutorial in Tutorials)
        {
            if (tutorial != null)
                tutorial.SetActive(false);
        }
        Time.timeScale = 1f;

        playerManager = PlayerManager.Instance; 
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        pausePanel.SetActive(false);

        maxWaveCount = StageDataTransfer.wavesCount;
        monsterWaves = new List<GameObject[]>(StageDataTransfer.monsterTypesPerWave);
        stageExp = StageDataTransfer.stageExp;

        pokerGameManager = FindObjectOfType<PokerGameManager>();
        tileController = FindAnyObjectByType<TileController>();
        tileselectManager = FindObjectOfType<TileselectManager>();

        pokerGameManager.gameObject.SetActive(false);
        tileController.gameObject.SetActive(false);

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.OnAllEnemiesDead += HandleAllEnemiesDead;

        if (tileselectManager != null)
            tileselectManager.OnTileSelectEnd += HandleTileSelectEnd;
        if (pokerGameManager != null)
            pokerGameManager.OnGameEnd += PokerEnd;
        if (tileController != null)
            tileController.OnTurnEnd += EndPlayerTurn;

        if (playerManager != null) // 안전 체크
            playerManager.OnAllAllysDead += HandleAllAllysDead;

        if (StageDataTransfer.currentWorld == 1 &&
        StageDataTransfer.currentStage == 1)
        {
            ShowTutorialWithDelay(0, 1f);
        }
        Indicator.text = $"{StageDataTransfer.currentWorld}-{StageDataTransfer.currentStage}";

        // =========================
        // 1-2 진입 컷신
        // =========================
        if (StageDataTransfer.currentWorld == 1 &&
            StageDataTransfer.currentStage == 2)
        {
            waitEntryCutscene = true;
            currentCutsceneIndex = 4;

            // 게임 시작 UI 전부 끔
            tileselectManager.gameObject.SetActive(false);
            pokerGameManager.gameObject.SetActive(false);
            tileController.gameObject.SetActive(false);

            CutScene[currentCutsceneIndex].SetActive(true);
            return;
        }

        ForceInitialStartApply();//강제 스탯 및 마나 초기화
        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();

        if (players.Length != 0) Debug.Log("플레이어들 null 아님");
        foreach (var player in players)
        {
            Debug.Log($"[Before Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
            Debug.Log($"[After Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
        }
    }

    private void HandleTileSelectEnd(C_TileType selectedTile)
    {
        this.selectedTile = selectedTile;
        currentTurnState = TurnState.POKER;
        currentWeapon = selectedTile == C_TileType.Arrow ? 0 : 1;
        tileController.WhatWeapon(currentWeapon);
        tileselectManager.gameObject.SetActive(false);
        pokerGameManager.gameObject.SetActive(true);
        StartPokerTurn?.Invoke();

        if (StageDataTransfer.currentWorld == 1 &&
        StageDataTransfer.currentStage == 1)
        {
            ShowTutorialWithDelay(1, 1f);
        }



        //
        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();

        if (players.Length != 0) Debug.Log("플레이어들 null 아님");
        foreach (var player in players)
        {
            Debug.Log($"[Before Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
            Debug.Log($"[After Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
        }
    }
    float CalculateMpPlus(float level)
    {
        return 20f + Mathf.Floor(level / 5f);
    }

    void ApplyMpPlusToPlayers()
    {

        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();

        foreach (var player in players)
        {
            Debug.Log($"player = {player}, stats = {player.CurrentStats}");
            Debug.Log($"[Before Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
            int index = GetPlayerIndex(player);
            if (index == -1) continue;

            float lv = player.CurrentStats.LV;
            tileController.MpPlus[index] = CalculateMpPlus(lv);
            Debug.Log($"[After Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
        }

        
    }


    int GetPlayerIndex(PlayerCharacter player)
    {
        Debug.Log($"[GetPlayerIndex] player = {player}, type = {player.GetType()}");

        if (player is Verka) return 0;
        if (player is Marshika) return 1;
        if (player is Chirr) return 2;

        Debug.LogWarning("[GetPlayerIndex] 매칭 실패!");
        return -1;
    }



    private void PokerEnd()
    {
        Debug.Log($"[PokerEnd] 호출됨");
        pokerGameManager.gameObject.SetActive(false);
        tileController?.gameObject.SetActive(true);
        this.multiplier = pokerGameManager.EvaluateAndShowRank();
        Debug.Log($"[PokerEnd] multiplier = {multiplier}");


        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();
        
        if (players.Length != 0) Debug.Log("플레이어들 null 아님");
        foreach (var player in players)
        {
            Debug.Log(
        $"[Before Multiplier] {player.name} ATK = {player.CurrentStats.ATK}"
    );
            CharacterPlayerHolder.Instance.ApplyToCharacter(player);
            player.ApplyATKMultiplier(multiplier);
            Debug.Log($"[After Multiplier] {player.name} ATK = {player.CurrentStats.ATK}" );
        }
        InitializeEnemyWave(currentWaveCount);
        WaveIndicator.text = $"WAVE {currentWaveCount + 1}";
        if (currentWaveCount == 0)
        {
            Board board = FindObjectOfType<Board>();
            GameObject prefab = selectedTile == C_TileType.Arrow
                ? board.arrowTilePrefab
                : board.shieldTilePrefab;

            Tiles tile = prefab.GetComponent<Tiles>();
            board.AddCTile(tile);//플레이어가 처음 지정한 특수타일을 보드에 추가합니다.
            board.GenerateBoard();
            FindObjectOfType<TileController>().RebindBoard();
            if (StageDataTransfer.currentWorld == 1 && StageDataTransfer.currentStage == 1)
            {
                ShowTutorialWithDelay(2, 1f);
            }
        }
        if (currentWaveCount > 0)
        {
            Board board = FindObjectOfType<Board>();
            board.ClearBoard();

            GameObject prefab = selectedTile == C_TileType.Arrow
                ? board.arrowTilePrefab
                : board.shieldTilePrefab;
            Tiles tile = prefab.GetComponent<Tiles>();
            board.AddCTile(tile);//플레이어가 처음 지정한 특수타일을 보드에 추가합니다.

            board.GenerateBoard();
            FindObjectOfType<TileController>().RebindBoard();

            //
            foreach (var player in players)
            {
                Debug.Log($"[Before Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
                Debug.Log($"[After Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
            }
        }
        currentTurnState = TurnState.PLAYER;
        ApplyMpPlusToPlayers();

        foreach (var player in players)
        {
            //player.RecalulateStats();
        }


        StartPlayerTurn?.Invoke();
    }


    private void EndPlayerTurn()
    {
        currentTurnState = TurnState.ENEMY;
        StartEnemyTurn?.Invoke();

        //
        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();

        foreach (var player in players)
        {
            Debug.Log($"[Before Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
            Debug.Log($"[After Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
        }
    }

    // 모든 적이 사망했을 때 호출되는 메서드
    private void HandleAllEnemiesDead()
    {
        currentWaveCount++;

        // =========================
        // 2웨이브 클리어 컷신 처리
        // =========================
        if (currentWaveCount == 2)
        {
            // 1-1 컷신
            if (StageDataTransfer.currentWorld == 1 &&
                StageDataTransfer.currentStage == 1)
            {
                PlayCutsceneBeforePoker(0);
                return;
            }

        }
        if (currentWaveCount == 3)
        {

            // 1-3 컷신
            if (StageDataTransfer.currentWorld == 1 &&
                StageDataTransfer.currentStage == 3)
            {
                PlayCutsceneBeforePoker(1);
                return;
            }
        }
        // =========================
        // 일반 웨이브 처리
        // =========================
        if (currentWaveCount < maxWaveCount)
        {
            StartPokerTurnNormally();
        }
        else
        {
            StageClear();
        }
    }


    private void OnCutsceneFinished()
    {
        if (waitEntryCutscene)
        {
            waitEntryCutscene = false;

            CutScene[currentCutsceneIndex].SetActive(false);
            currentCutsceneIndex = -1;

            // 타일 선택부터 시작
            tileselectManager.gameObject.SetActive(true);
            currentTurnState = TurnState.TILESELECT;
            return;
        }

        // -------------------------
        // 포커 턴 전 컷신
        // -------------------------
        if (waitPokerAfterCutscene)
        {
            waitPokerAfterCutscene = false;

            CutScene[currentCutsceneIndex].SetActive(false);
            currentCutsceneIndex = -1;

            pokerGameManager.gameObject.SetActive(true);
            StartPokerTurn?.Invoke();
            return;
        }

        // -------------------------
        // 스테이지 클리어 컷신
        // -------------------------
        if (waitClearAfterCutscene)
        {
            waitClearAfterCutscene = false;

            CutScene[currentCutsceneIndex].SetActive(false);
            currentCutsceneIndex = -1;

            // 컷신 끝나고 진짜 클리어 처리
            DoStageClear();
        }
    }


    private void PlayCutsceneBeforePoker(int cutsceneIndex)
    {
        waitPokerAfterCutscene = true;
        currentCutsceneIndex = cutsceneIndex;

        tileController.resetAttackCount();
        currentTurnState = TurnState.POKER;

        tileselectManager.gameObject.SetActive(false);
        tileController.gameObject.SetActive(false);
        pokerGameManager.gameObject.SetActive(false);

        CutScene[cutsceneIndex].SetActive(true);
    }

    private void StartPokerTurnNormally()
    {
        tileController.resetAttackCount();
        currentTurnState = TurnState.POKER;

        tileselectManager.gameObject.SetActive(false);
        tileController.gameObject.SetActive(false);
        pokerGameManager.gameObject.SetActive(true);

        StartPokerTurn?.Invoke();


        //
        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();

        foreach (var player in players)
        {
            Debug.Log($"[Before Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
            Debug.Log($"[After Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
        }
    }

    private void HandleAllAllysDead()
    {
        Debug.Log("모든 아군이 사망했습니다. 게임 오버!");
        currentTurnState = TurnState.DEFEAT;
        Board board = FindObjectOfType<Board>();
        if (board != null)
        {
            board.StopAllCoroutines();
            board.ClearBoard();
        }

        // Stop tile logic
        TileController tc = FindObjectOfType<TileController>();
        if (tc != null)
            tc.gameObject.SetActive(false);

        // Stop poker UI & tile select
        if (pokerGameManager != null)
            pokerGameManager.gameObject.SetActive(false);

        if (tileselectManager != null)
            tileselectManager.gameObject.SetActive(false);

        // Stop enemy
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.StopAllCoroutines();
            EnemyManager.Instance.enemies.Clear();
        }
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ClearAllPlayers();
            PlayerManager.Instance.StopAllCoroutines();
        }
        // Stop animations
        DG.Tweening.DOTween.KillAll();
        defeatPanel.SetActive(true);

    }

    private void InitializeAlly()
    {
        Transform uiParent = GameObject.Find("TileCanvas").transform;

        Vector2[] uiPositions =
        {
            new Vector2(0, -760),
            new Vector2(-300, -760),
            new Vector2(300, -760)
        };

        Vector2[] uiPositions_2 =
        {
            new Vector2(-200, -760),
            new Vector2(200, -760)
        };

        for (int i = 0; i < allyPrefabs.Length; i++)
        {
            if (allyPrefabs[i] == null)
                continue;

            // UI 오브젝트 생성
            GameObject allyUI = Instantiate(allyPrefabs[i], uiParent, false);
            allyUI.name = allyPrefabs[i].name; // 이름 설정(clone 제거)

            RectTransform rect = allyUI.GetComponent<RectTransform>();
            if (rect == null) rect = allyUI.AddComponent<RectTransform>();
            if(allyPrefabs.Length==1 || allyPrefabs.Length == 3)
                rect.anchoredPosition = uiPositions[i];
            else
                rect.anchoredPosition = uiPositions_2[i];
            Debug.Log(allyPrefabs.Length);


            rect.localScale = allyPrefabs[i].transform.localScale;

            PlayerCharacter pc = allyUI.GetComponent<PlayerCharacter>();
            if (pc != null)
            {
                PlayerManager.Instance.AddPlayer(pc);
            }
        }

        Debug.Log($"아군 {allyPrefabs.Length}명 생성 + 플레이어 등록 완료");

        //
        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();

        foreach (var player in players)
        {
            Debug.Log($"[Before Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
            Debug.Log($"[After Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
        }
    }

    // 현재 웨이브의 적들 생성
    private void InitializeEnemyWave(int waveIndex)
    {
        EnemyManager enemyManager = EnemyManager.Instance;
        Transform uiParent = GameObject.Find("TileCanvas").transform;

        Vector2[] uiPositions =
        {
            new Vector2(-320f, 510f),
            new Vector2(0f, 650f),
            new Vector2(320f, 510f)
        };

        enemyManager.enemies.Clear();

        if (waveIndex >= monsterWaves.Count)
        {
            Debug.LogWarning($"잘못된 웨이브 인덱스: {waveIndex}");
            return;
        }

        GameObject[] currentWave = monsterWaves[waveIndex];
        for (int i = 0; i < currentWave.Length && i < uiPositions.Length; i++)
        {
            if (currentWave[i] == null) continue;

            GameObject enemyUI = Instantiate(currentWave[i], uiParent, false);
            RectTransform rect = enemyUI.GetComponent<RectTransform>() ?? enemyUI.AddComponent<RectTransform>();

            rect.anchoredPosition = uiPositions[i];
            rect.localScale = currentWave[i].transform.localScale;

            EnemyCharacter enemyChar = enemyUI.GetComponent<EnemyCharacter>();
            if (enemyChar != null)
                enemyManager.enemies.Add(enemyChar);
        }

        Debug.Log($"웨이브 {waveIndex + 1} 생성 완료 — 적 {enemyManager.enemies.Count}명");
    }
    void StageClear()
    {
        // 1-1 클리어 컷신
        if (StageDataTransfer.currentWorld == 1 &&
            StageDataTransfer.currentStage == 1 &&
            !waitClearAfterCutscene)
        {
            waitClearAfterCutscene = true;
            currentCutsceneIndex = 2;

            StopAllBattleUI();
            CutScene[currentCutsceneIndex].SetActive(true);
            return;
        }

        // 1-3 클리어 컷신
        if (StageDataTransfer.currentWorld == 1 &&
            StageDataTransfer.currentStage == 3 &&
            !waitClearAfterCutscene)
        {
            waitClearAfterCutscene = true;
            currentCutsceneIndex = 3;

            StopAllBattleUI();
            CutScene[currentCutsceneIndex].SetActive(true);
            return;
        }

        // 컷신 없는 경우
        DoStageClear();
    }

    private void StopAllBattleUI()
    {
        Board board = FindObjectOfType<Board>();
        if (board != null)
        {
            board.StopAllCoroutines();
            board.ClearBoard();
        }

        if (tileController != null)
            tileController.gameObject.SetActive(false);

        if (pokerGameManager != null)
            pokerGameManager.gameObject.SetActive(false);

        if (tileselectManager != null)
            tileselectManager.gameObject.SetActive(false);

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.StopAllCoroutines();
            EnemyManager.Instance.enemies.Clear();
        }
        /*
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ClearAllPlayers();
            PlayerManager.Instance.StopAllCoroutines();
        }
        */
        DG.Tweening.DOTween.KillAll();


        //
        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();

        foreach (var player in players)
        {
            Debug.Log($"[Before Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
            Debug.Log($"[After Multiplier] {player.name} ATK = {player.CurrentStats.ATK}");
        }
    }


    public void GamePaused()
    {
        isPaused = !isPaused; // 토글

        if (isPaused)
        {
            Time.timeScale = 0f; // 게임 정지
            if (pausePanel != null)
                pausePanel.SetActive(true); // UI 켜기
        }
        else
        {
            Time.timeScale = 1f; // 게임 재개
            if (pausePanel != null)
                pausePanel.SetActive(false); // UI 끄기
        }
    }
    private void GiveExpToPlayers(int exp)
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogWarning("PlayerManager 인스턴스가 없습니다. EXP를 지급할 수 없습니다.");
            return;
        }

        foreach (PlayerCharacter player in PlayerManager.Instance.playerCharacters)
        {
            if (player == null) continue;
            player.PlusExp(exp);
            Debug.Log($"{player.name} EXP +{exp}");
        }
    }

    private void ShowTutorialWithDelay(int index, float delay)
    {
        if (Tutorials == null || Tutorials.Length <= index)
        {
            Debug.LogWarning($"튜토리얼 인덱스 {index}가 유효하지 않습니다.");
            return;
        }

        if (tutorialCoroutine != null)
            StopCoroutine(tutorialCoroutine);
        tutorialCoroutine = StartCoroutine(
            TutorialDelayRoutine(index, delay)
        );
    }

    private IEnumerator TutorialDelayRoutine(int index, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        // 전부 끄고
        for (int i = 0; i < Tutorials.Length; i++)
        {
            if (Tutorials[i] != null)
                Tutorials[i].SetActive(false);
        }

        // 특정 인덱스만 켜기
        if (Tutorials[index] != null)
            Tutorials[index].SetActive(true);
    }

    private void DoStageClear()
    {
        currentTurnState = TurnState.CLEAR;
        MySoundManager.Instance.PlayStageClear();
        GiveExpToPlayers(stageExp);

        StageIndicator.text =
            $"STAGE {StageDataTransfer.currentWorld}-{StageDataTransfer.currentStage} CLEAR";

        DG.Tweening.DOTween.KillAll();
        victoryPanel.SetActive(true);

        int clearedWorld = StageDataTransfer.currentWorld;
        int clearedStage = StageDataTransfer.currentStage;

        StageProgress.ClearStage(clearedWorld, clearedStage, 6);
        StageDataTransfer.currentStage++;

        Debug.Log($"현재 진행중: {StageDataTransfer.currentWorld}-{StageDataTransfer.currentStage}");
    }
    public void RestartCurrentStage()
    {
        // 시간 정지 해제
        Time.timeScale = 1f;

        // Tween, 코루틴 정리
        DG.Tweening.DOTween.KillAll();

        // 전투 중 생성된 오브젝트 정리 (있다면)
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.StopAllCoroutines();
            EnemyManager.Instance.enemies.Clear();
        }

        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.StopAllCoroutines();
            PlayerManager.Instance.ClearAllPlayers();
        }

        // ⚠️ StageDataTransfer.ClearData() 호출 ❌ 하면 안 됨

        // Main 씬 재로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToStatusScene()
    {
        // 일시정지 상태 해제
        Time.timeScale = 1f;

        // Tween 정리 (선택)
        DG.Tweening.DOTween.KillAll();

        // Status 씬 로드
        SceneManager.LoadScene("Status");
    }

    private void ForceInitialStartApply()
    {
        PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();

        ApplyMpPlusToPlayers();

        foreach(var player in players)
        {
            //player.RecalulateStats();
        }
        Debug.Log("초기 스탯 강제 적용 완료");
    }


}
