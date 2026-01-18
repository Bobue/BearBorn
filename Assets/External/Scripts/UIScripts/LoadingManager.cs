using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
   void Start()
    {
        if(string.IsNullOrEmpty(LoadingData.TargetScene))
        {
            Debug.LogError("전환할 씬이 없어요");
            return;
        }

        StartCoroutine(LoadRoutine());
    }

    IEnumerator LoadRoutine()
    {


        AsyncOperation op = SceneManager.LoadSceneAsync(LoadingData.TargetScene);
        op.allowSceneActivation = false;

        while(op.progress <0.9f)
        {
            yield return null;
        }

        LoadingData.Clear();
        //yield return StartCoroutine(FadeTransitionManager.Instance.FadeAndActivateScene(op));
    }

}
