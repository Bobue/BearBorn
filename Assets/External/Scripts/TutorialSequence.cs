using System.Collections;
using UnityEngine;

public class TutorialSequence : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private float pageDuration = 2f;

    private void OnEnable()
    {
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // 전부 끄고 시작
        foreach (var page in pages)
            page.SetActive(false);

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(true);
            yield return new WaitForSeconds(pageDuration);
            pages[i].SetActive(false);
        }

        // 튜토리얼 끝
        OnSequenceComplete();
    }

    private void OnSequenceComplete()
    {
        // 필요 시 부모 끄기
        gameObject.SetActive(false);
    }
}
