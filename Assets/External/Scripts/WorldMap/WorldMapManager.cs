using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WorldMapManager : MonoBehaviour
{
    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip worldSelectSound;

    string[] sceneToLoad =
    {
        "RadgaReactor",   // 1
        "BlackZone",      // 2
        "SnowingZone",    // 3
        "Forrest",        // 4
        "Razvetya"        // 5
    };

    [Header("Map Images (1~5 World)")]
    public Image[] mapImages;

    [Header("Locked World Sprite")]
    public Sprite[] EasySnowing; // 잠긴 월드용 스프라이트

    [SerializeField] private Camera cam;

    [Header("Zoom")]
    public float zoomSpeed = 2f;

    [Header("Map Bounds")]
    public Collider2D mapBoundsCollider;

    private Bounds mapBounds;
    private Vector3 lastMousePos;
    private bool isDragging = false;
    private bool isTransitioning = false;

    public static int selectedContentIndex = -1;

    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    // 하이라이트
    private Image currentSelectedImage;
    private Color originalColor;

    private void Start()
    {
        mapBounds = mapBoundsCollider.bounds;

        Canvas canvas = GetComponentInParent<Canvas>();
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        ApplyWorldSprites();
    }

    void Update()
    {
        HandleZoom();
        HandleDrag();
        HandleClick();
    }

    private void LateUpdate()
    {
        ClampCameraMap();
    }

    // ================= WORLD SPRITE SETUP =================
    void ApplyWorldSprites()
    {
        int currentWorld = StageDataTransfer.currentWorld;

        for (int i = 0; i < mapImages.Length; i++)
        {
            int worldIndex = i + 1;

            // 잠긴 월드 (EasySnowing)
            if (worldIndex > currentWorld)
            {
                mapImages[i].sprite = EasySnowing[i];
                mapImages[i].color = new Color(1f, 1f, 1f, 0.6f);
            }
            // 해금된 월드
            else
            {
                mapImages[i].color = Color.white;
            }
        }
    }

    // ================= ZOOM =================
    void HandleZoom()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if (Mathf.Abs(zoomDelta) > 0.001f)
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomDelta, 1f, 5f);
#endif
    }

    // ================= DRAG =================
    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            if (delta.magnitude > 1f)
            {
                isDragging = true;
                float unitsPerPixel = (cam.orthographicSize * 2f) / Screen.height;
                cam.transform.position += new Vector3(-delta.x, -delta.y, 0f) * unitsPerPixel;
            }

            lastMousePos = Input.mousePosition;
        }
    }

    // ================= CLICK =================
    void HandleClick()
    {
        if (isDragging || isTransitioning) return;
        if (!Input.GetMouseButtonUp(0)) return;

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            for (int i = 0; i < mapImages.Length; i++)
            {
                if (result.gameObject == mapImages[i].gameObject)
                {
                    int worldIndex = i + 1;

                    // ❌ 잠긴 월드
                    if (worldIndex > StageDataTransfer.currentWorld)
                        return;

                    // ✅ 여기서 사운드 재생
                    PlayWorldSelectSound();

                    StartCoroutine(SelectAndMoveScene(i, mapImages[i]));
                    return;
                }
            }
        }
    }
    void PlayWorldSelectSound()
    {
        if (audioSource == null || worldSelectSound == null)
            return;

        audioSource.PlayOneShot(worldSelectSound);
    }


    IEnumerator SelectAndMoveScene(int index, Image img)
    {
        isTransitioning = true;
        selectedContentIndex = index;

        // 이전 강조 해제
        if (currentSelectedImage != null)
            currentSelectedImage.color = originalColor;

        // 현재 강조
        currentSelectedImage = img;
        originalColor = img.color;
        img.color = Color.yellow;

        yield return new WaitForSeconds(0.3f);

        if (FadeTransitionManager.Instance != null)
            FadeTransitionManager.Instance.LoadScene("RadgaReactor");
        else
            SceneManager.LoadScene("RadgaReactor");
    }

    void ClampCameraMap()
    {
        float vert = cam.orthographicSize;
        float horz = vert * cam.aspect;

        float left = mapBounds.min.x + horz;
        float right = mapBounds.max.x - horz;
        float bottom = mapBounds.min.y + vert;
        float top = mapBounds.max.y - vert;

        Vector3 pos = cam.transform.position;
        pos.x = Mathf.Clamp(pos.x, left, right);
        pos.y = Mathf.Clamp(pos.y, bottom, top);

        cam.transform.position = pos;
    }
}
