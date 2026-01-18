using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum SceneTransitionPolicy//씬 기본 정책
{
    Direct,//로딩없이 바로 전환
    WithLoading//로딩씬을 거쳐 전환
}

public class FadeTransitionManager : MonoBehaviour
{
    public static FadeTransitionManager Instance { get; private set; }
    
    public Image fadePanel;
    public float fadeDuration = 1.0f;


    private Dictionary<string, SceneTransitionPolicy> scenePolicies =
    new Dictionary<string, SceneTransitionPolicy>()
{
        {"Status", SceneTransitionPolicy.WithLoading },
        {"Upgrade", SceneTransitionPolicy.WithLoading },
        {"WorldMap", SceneTransitionPolicy.WithLoading }

};

    private void Awake()
    {
        Debug.Log($"FadeTM Awake: {GetInstanceID()}");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if(fadePanel != null)
        {
            Color color = fadePanel.color;
            color.a = 0f;
            fadePanel.color = color;
            fadePanel.gameObject.SetActive(true);
        }
    }
    public void LoadScene(string sceneName)
    {
        if(!scenePolicies.TryGetValue(sceneName, out var policy))
        {
            Debug.Log($"{sceneName}씬 기본 정책 없음, 기본 정책 사용(Direct)");
            policy = SceneTransitionPolicy.Direct;
        }


        StartCoroutine(TransitionScene(sceneName, policy));
    }


    public IEnumerator TransitionScene(string sceneName, SceneTransitionPolicy policy)
    {
        yield return StartCoroutine(Fade(1f));//페이드 인

        if(policy ==SceneTransitionPolicy.WithLoading)//로딩 전환
        {
            LoadingData.TargetScene = sceneName;
            SceneManager.LoadScene("LoadingScene");
        }
        else//직접 전환
        {
            SceneManager.LoadScene(sceneName);
        }

        yield return null;
        yield return StartCoroutine(Fade(0f));
    }

    public IEnumerator FadeAndActivateScene(AsyncOperation op)//씬 로딩 후 페이드 아웃하고 씬 활성화(로딩씬에서만 사용)
    {
        yield return StartCoroutine(Fade(1f));
        op.allowSceneActivation = true;
        yield return StartCoroutine(Fade(0f));
    }


    public IEnumerator Fade(float targetAlpha)
    {
        if(fadePanel == null)
        {
            Debug.Log("Fade 패널 없습니다~");
            yield break;
        }

        float startAlpha = fadePanel.color.a;
        float timer = 0f;

        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer/fadeDuration);

            Color color = fadePanel.color;
            color.a = newAlpha;
            fadePanel.color = color;

            yield return null;
        }

        Color finalColor = fadePanel.color;
        finalColor.a = targetAlpha;
        fadePanel.color = finalColor;
    }
}
