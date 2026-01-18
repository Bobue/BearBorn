using UnityEngine;

public class VerticalScroller : MonoBehaviour
{
    public Camera cam;

    public float minY = 0f;
    public float maxY = -50f;

    // 드래그 관련 변수
    private Vector3 lastMousePos;
    private bool isDragging = false;
    public float dragThreshold = 1f;

    void Awake()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }

        SetScrollLimitsByWorld();
    }

    void Update()
    {
        HandleDrag();
    }

    private void LateUpdate()
    {
        ClampCameraVertical();
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;

            if (Mathf.Abs(delta.y) > dragThreshold)
            {
                isDragging = true;
            }

            if (isDragging)
            {
                float unitsPerPixel = (cam.orthographicSize * 2f) / Screen.height;

                float moveY = -delta.y * unitsPerPixel;

                Vector3 currentPos = cam.transform.position;

                cam.transform.position = new Vector3(
                    currentPos.x,
                    currentPos.y + moveY,
                    currentPos.z
                );
            }
            lastMousePos = Input.mousePosition;
        }
    }

    void ClampCameraVertical()
    {
        float vertExtent = cam.orthographicSize;

        float clampedBottom = minY + vertExtent;
        float clampedTop = maxY - vertExtent;

        Vector3 pos = cam.transform.position;

        // 세로 스크롤 제한
        if (clampedBottom > clampedTop)
        {
            pos.y = (minY + maxY) / 2f;
        }
        else
        {
            pos.y = Mathf.Clamp(pos.y, clampedBottom, clampedTop);
        }
        cam.transform.position = pos;
    }

    private void SetScrollLimitsByWorld()
    {
        int world = StageDataTransfer.currentWorld;

        switch (world)
        {
            case 1:
                maxY = 5f;   // 최대 Y 고정
                minY = -5f;  // 최소 Y
                break;
            case 2:
                maxY = 5f;   // 최대 Y는 동일
                minY = -15f; // 최소 Y 첫 값
                // 만약 6개 스테이지라면 -15, -25, -35, ... 이런 식으로 처리 가능
                break;
            default:
                maxY = 5f;
                minY = -50f;
                break;
        }

        Debug.Log($"[VerticalScroller] World {world} -> minY: {minY}, maxY: {maxY}");
    }
}
