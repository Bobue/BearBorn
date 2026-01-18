using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuSlider : MonoBehaviour
{
    public RectTransform pageContainer;
    public RectTransform buttonContainer;
    public Image selectedImage;
    public float pageWidth = 339f;  
    public float slideDuration = 0.3f;
    private Coroutine currentCoroutine;
    private Coroutine moveImageCoroutine;

    // 씬 이동 전 여기 값을 설정해서 시작 페이지 지정
    public static int StartPageIndex = 0;

    void Start()
    {
        // 씬 열리자마자 즉시 페이지 위치와 선택 이미지 위치 세팅
        JumpToPage(StartPageIndex);
    }

    // 즉시 페이지 위치 이동
    public void JumpToPage(int index)
    {
        float targetX = -index * pageWidth;
        pageContainer.anchoredPosition = new Vector2(targetX, 0);

        MoveSelectedImageImmediate(index);
    }
    public void MoveToPage(int index)
    {
        float targetX = -index * pageWidth;
        Vector2 targetPos = new Vector2(targetX, 0);

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(SlideTo(targetPos));

        MoveSelectedImage(index);
    }

    IEnumerator SlideTo(Vector2 target)
    {
        Vector2 start = pageContainer.anchoredPosition;
        float time = 0f;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = time / slideDuration;
            pageContainer.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        pageContainer.anchoredPosition = target;
    }
    void MoveSelectedImageImmediate(int index)
    {
        if (index < 0 || index >= buttonContainer.childCount)
            return;

        RectTransform targetButton = buttonContainer.GetChild(index) as RectTransform;
        float targetX = targetButton.anchoredPosition.x;

        selectedImage.rectTransform.anchoredPosition = new Vector2(targetX, selectedImage.rectTransform.anchoredPosition.y);
    }

    void MoveSelectedImage(int index)
    {
        if (index < 0 || index >= buttonContainer.childCount)
            return;

        RectTransform targetButton = buttonContainer.GetChild(index) as RectTransform;
        float targetX = targetButton.anchoredPosition.x;

        if (moveImageCoroutine != null)
            StopCoroutine(moveImageCoroutine);
        moveImageCoroutine = StartCoroutine(SmoothMoveImage(targetX));
    }

    IEnumerator SmoothMoveImage(float targetX)
    {
        float time = 0f;
        float duration = 0.2f;
        float startX = selectedImage.rectTransform.anchoredPosition.x;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float newX = Mathf.Lerp(startX, targetX, t);
            selectedImage.rectTransform.anchoredPosition = new Vector2(newX, selectedImage.rectTransform.anchoredPosition.y);
            yield return null;
        }

        selectedImage.rectTransform.anchoredPosition = new Vector2(targetX, selectedImage.rectTransform.anchoredPosition.y);
    }
}
