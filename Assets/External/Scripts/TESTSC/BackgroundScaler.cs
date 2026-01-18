using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ScaleBackground();
    }

    public void ScaleBackground()
    {
        if (spriteRenderer == null) return;

        // 1. 카메라의 높이와 너비를 유니티 단위로 계산
        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        // 2. 스프라이트 원본의 크기 계산
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        // 3. 화면을 꽉 채우기 위한 스케일 계산 (필요시 '확대' 방식)
        Vector3 newScale = transform.localScale;

        // 가로/세로 중 더 큰 비율에 맞춰서 배경이 화면을 '덮도록' 설정 (잘리더라도 빈 곳 없게)
        float scaleX = worldScreenWidth / spriteWidth;
        float scaleY = worldScreenHeight / spriteHeight;

        // 배경이 화면을 완전히 가려야 하므로 Max값을 취함 (여백 방지)
        float finalScale = Mathf.Max(scaleX, scaleY);

        newScale.x = finalScale;
        newScale.y = finalScale;

        transform.localScale = newScale;

        // 4. 배경의 위치를 카메라 중앙에 맞춤 (Z값은 유지)
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0f);
    }
}