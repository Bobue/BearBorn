using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;
    private AudioSource audioSource;
    private string[] keepScenes = { "WorldMap", "RadgaReactor" };

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Loaded scene: {scene.name}");

        // 🔥 BGM 종료 트리거 씬들
        if (scene.name == "Status" || scene.name == "Main")
        {
            Shutdown();
            return;
        }

        // 🎵 유지 대상 씬이면 재생
        if (System.Array.IndexOf(keepScenes, scene.name) >= 0)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }

    private void Shutdown()
    {
        // 중복 호출 방지
        if (Instance != this) return;

        SceneManager.sceneLoaded -= OnSceneLoaded; // ★ 핵심
        audioSource.Stop();
        Instance = null;
        Destroy(gameObject);
    }
}