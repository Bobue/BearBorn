using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;

    // 이동
    private Vector3 dragOrigin;

    // 줌
    public float zoomSpeed = 2f;
    public float minZoom = 3f;
    public float maxZoom = 10f;

    void Update()
    {
        HandleZoom();
        HandleDrag();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }

    void HandleDrag()
    {
        // 마우스 왼쪽 버튼 클릭 시작 위치 저장
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        // 마우스 이동에 따라 카메라 이동
        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position += difference;
        }
    }
}
