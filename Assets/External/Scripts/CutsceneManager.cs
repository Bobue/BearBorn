using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    void OnEnable()
    {
        CutSceneProgress.OnCutsceneFinished2 += GoToMain;
    }

    void OnDisable()
    {
        CutSceneProgress.OnCutsceneFinished2 -= GoToMain;
    }

    void GoToMain()
    {
        SceneManager.LoadScene("Main");
    }
}
