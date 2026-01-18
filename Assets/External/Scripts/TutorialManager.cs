using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    private bool statusTutorialOnce = false;
    private bool statTutorialOnce = false;


    private GameObject staminerTutorial;
    private GameObject invenTutorial;
    private GameObject stageTutorial;
    private GameObject StatusTutorial;
    private GameObject StatTutorial;
    private bool lvUpTutorialStarted = false;
    private GameObject lvUpTutorial;

    private bool quikPlayTutorialStarted = false;
    private GameObject quikPlayTutorial;



    private bool statusTutorialStarted = false;
    private bool worldMapTutorialStarted = false;
    private bool radgaReactorTutorialStarted = false;

    private bool invenStarted = false;



    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Status")
            HandleStatusScene();
        else if (scene.name == "WorldMap")
            HandleWorldMapScene();
        else if (scene.name == "RadgaReactor")
            HandleRadgaReactorScene();
        else if (scene.name == "Upgrade")
            HandleUpgradeScene();
    }

    private void HandleUpgradeScene()
    {
        if (lvUpTutorialStarted) return;  // 이미 켰으면 리턴

        GameObject root = GetTutorialRoot();
        if (root == null) return;

        lvUpTutorial = root.transform.Find("LVUpTutorial")?.gameObject;
        if (lvUpTutorial == null)
        {
            Debug.LogError("LVUpTutorial을 찾을 수 없습니다.");
            return;
        }

        lvUpTutorial.SetActive(true);
        lvUpTutorialStarted = true; // 다시 안 켜도록 플래그 설정
    }

    private GameObject GetTutorialRoot()
    {
        GameObject root = GameObject.Find("TutorialRoot");
        if (root == null)
        {
            Debug.LogError("TutorialRoot를 찾을 수 없습니다.");
        }
        return root;
    }

    private void HandleStatusScene()
    {
        GameObject root = GetTutorialRoot();
        if (root == null) return;

        staminerTutorial = root.transform.Find("StaminerTutorial")?.gameObject;
        invenTutorial = root.transform.Find("InvenTutorial")?.gameObject;
        StatusTutorial = root.transform.Find("StatusTutorial")?.gameObject;

        // TutorialRoot2
        GameObject root2 = GameObject.Find("TutorialRoot2");
        if (root2 != null)
            StatTutorial = root2.transform.Find("StatTutorial")?.gameObject;

        // ===== Staminer =====
        if (!statusTutorialStarted)
        {
            staminerTutorial?.SetActive(true);
            statusTutorialStarted = true;
        }
        else
        {
            staminerTutorial?.SetActive(false);
        }

        invenTutorial?.SetActive(false);

        // ===== StatusTutorial & StatTutorial (각각 한 번만) =====
        bool isCondition =
            StageDataTransfer.currentWorld == 1 &&
            StageDataTransfer.currentStage == 2;

        if (isCondition)
        {
            // StatusTutorial
            if (!statusTutorialOnce && StatusTutorial != null)
            {
                StatusTutorial.SetActive(true);
                statusTutorialOnce = true;
            }
            else
            {
                StatusTutorial?.SetActive(false);
            }

            // StatTutorial
            GameObject StatusObj = GameObject.Find("Status");
            if (!statTutorialOnce &&
                StatusObj != null &&
                StatusObj.activeInHierarchy &&
                StatTutorial != null)
            {
                StatTutorial.SetActive(true);
                statTutorialOnce = true;
            }
            else
            {
                StatTutorial?.SetActive(false);
            }
        }
        else
        {
            StatusTutorial?.SetActive(false);
            StatTutorial?.SetActive(false);
        }

        // ===== QuickPlay =====
        if (StageDataTransfer.currentWorld == 1 &&
            StageDataTransfer.currentStage == 3 &&
            !quikPlayTutorialStarted)
        {
            quikPlayTutorial = root.transform.Find("QuikPlayTutorial")?.gameObject;
            if (quikPlayTutorial != null)
            {
                quikPlayTutorial.SetActive(true);
                quikPlayTutorialStarted = true;
            }
        }
    }



    private void HandleWorldMapScene()
    {
        if (worldMapTutorialStarted) return;

        worldMapTutorialStarted = true;

        GameObject root = GetTutorialRoot();
        if (root == null) return;

        invenTutorial = root.transform.Find("WorldMapTutorial")?.gameObject;
        if (invenTutorial == null)
        {
            Debug.LogError("WorldMap InvenTutorial을 찾을 수 없습니다.");
            return;
        }

        invenTutorial.SetActive(true);
    }

    private void HandleRadgaReactorScene()
    {
        if (radgaReactorTutorialStarted) return;

        radgaReactorTutorialStarted = true;

        GameObject root = GetTutorialRoot();
        if (root == null) return;

        stageTutorial = root.transform.Find("StageTutorial")?.gameObject;
        if (stageTutorial == null)
        {
            Debug.LogError("StageTutorial을 찾을 수 없습니다.");
            return;
        }

        stageTutorial.SetActive(true);
    }

    private void Update()
    {
        // Status 씬 전용 연결 로직
        if (staminerTutorial != null &&
            !staminerTutorial.activeSelf &&
            !invenStarted)
        {
            invenStarted = true;
            invenTutorial.SetActive(true);
        }
    }
}
