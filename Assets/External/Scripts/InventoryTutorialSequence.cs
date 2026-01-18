using System.Collections;
using UnityEngine;

public class InventoryTutorialSequence : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private GameObject page1; // 인벤토리 버튼 누르세요
    [SerializeField] private GameObject page2; // 인벤토리 닫기 후
    [SerializeField] private GameObject page3; // 2초 후

    private Coroutine page3Coroutine;

    private void OnEnable()
    {
        // 초기 상태
        page1.SetActive(true);
        page2.SetActive(false);
        page3.SetActive(false);
    }

    // 🔹 1단계: 인벤토리 버튼 눌렀을 때
    public void OnInventoryButtonClicked()
    {
        page1.SetActive(false);
        // 인벤토리는 이 시점에 열리면 됨
    }

    // 🔹 2단계: 인벤토리 닫기 버튼 눌렀을 때
    public void OnInventoryClosed()
    {
        page2.SetActive(true);

        // 2초 뒤 Page 3 활성화
        if (page3Coroutine != null)
            StopCoroutine(page3Coroutine);

        page3Coroutine = StartCoroutine(ShowPage3AfterDelay());
    }

    private IEnumerator ShowPage3AfterDelay()
    {
        yield return new WaitForSeconds(2f);
        page3.SetActive(true);
    }
}
