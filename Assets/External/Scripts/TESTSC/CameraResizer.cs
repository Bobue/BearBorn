using UnityEngine;

public class CameraResizer : MonoBehaviour
{
    public Board board; // 유니티 인스펙터에서 Board 오브젝트를 드래그 앤 드롭
    public float padding = 1.0f; // 보드 주변의 여백

    void Start()
    {
        AdjustCamera();
    }

    public void AdjustCamera()
    {
        if (board == null) return;

        // 1. 카메라를 보드의 중앙으로 이동
        // Board의 GetCenterPos 로직에 맞춰 카메라 위치 조정 (Z축은 유지)
        transform.position = new Vector3(0, 0, -10f);

        // 2. 보드의 전체 크기 계산
        float boardWidth = board.width;
        float boardHeight = board.height;

        // 3. 현재 화면의 종횡비 (가로 / 세로)
        float screenAspect = (float)Screen.width / (float)Screen.height;
        // 보드의 종횡비
        float boardAspect = boardWidth / boardHeight;

        Camera cam = GetComponent<Camera>();

        if (screenAspect >= boardAspect)
        {
            // 화면이 보드보다 더 넓은 경우 (세로 기준 맞춤)
            cam.orthographicSize = (boardHeight / 2) + padding;
        }
        else
        {
            // 화면이 보드보다 더 좁은 경우 (가로 기준 맞춤)
            // 가로 폭에 맞추기 위해 세로 크기(Size)를 역산함
            float differenceInSize = boardAspect / screenAspect;
            cam.orthographicSize = (boardHeight / 2 * differenceInSize) + padding;
        }
    }
}