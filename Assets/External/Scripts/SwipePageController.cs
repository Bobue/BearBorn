using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SwipePageController : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Page")]
    [SerializeField] private int pageCount = 3;
    [SerializeField] private RectTransform pageContainer;
    [SerializeField] private float swipeThreshold = 150f;
    [SerializeField] private GameObject TutorialPage;

    [Header("Indicator")]
    [SerializeField] private Transform dotParent;
    [SerializeField] private Image dotPrefab;
    [SerializeField] private float dotSpacing = 100f;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(1, 1, 1, 0.3f);

    private List<Image> dots = new List<Image>();
    private int currentPage = 0;

    private Vector2 dragStartPos;
    private Vector2 containerStartPos;
    private float pageWidth;

    void Start()
    {
        pageWidth = ((RectTransform)transform).rect.width;

        CreateDots();
        SnapToPage(0);
    }

    #region Swipe Input
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = eventData.position;
        containerStartPos = pageContainer.anchoredPosition;
    }

    public void ExitPage()
    {
        TutorialPage.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float deltaX = eventData.position.x - dragStartPos.x;
        pageContainer.anchoredPosition = containerStartPos + new Vector2(deltaX, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float dragDistance = eventData.position.x - dragStartPos.x;

        if (Mathf.Abs(dragDistance) > swipeThreshold)
        {
            currentPage += dragDistance < 0 ? 1 : -1;
        }

        currentPage = Mathf.Clamp(currentPage, 0, pageCount - 1);
        SnapToPage(currentPage);
    }
    #endregion

    #region Page Move
    void SnapToPage(int pageIndex)
    {
        float targetX = -pageIndex * pageWidth;
        pageContainer.anchoredPosition =
            new Vector2(targetX, pageContainer.anchoredPosition.y);

        UpdateDots(pageIndex);
    }
    #endregion

    #region Dot Indicator
    void CreateDots()
    {
        foreach (Transform child in dotParent)
            Destroy(child.gameObject);

        dots.Clear();

        float totalWidth = (pageCount - 1) * dotSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < pageCount; i++)
        {
            Image dot = Instantiate(dotPrefab, dotParent);
            dot.gameObject.SetActive(true);

            RectTransform rt = dot.rectTransform;
            rt.anchoredPosition = new Vector2(startX + i * dotSpacing, 0);

            dots.Add(dot);
        }
    }

    void UpdateDots(int activeIndex)
    {
        for (int i = 0; i < dots.Count; i++)
        {
            dots[i].color = (i == activeIndex)
                ? activeColor
                : inactiveColor;
        }
    }
    #endregion
}
