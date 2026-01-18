using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMove : MonoBehaviour
{
    [Header("설정:")]
    public string sceneToLoad = "NextSceneName";


    private bool isDragging = false;
    private Vector3 initialMousePos;


    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)//플랫폼이 안드로이드일때의 뒤로가기 물리키 인식
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                HandleClickDetection();
            }
        }
    }
    /*
    private void Update()
    {
        HandleClickDetection();
    }*/
    public void HandleClickDetection()
    {
        /*if(FadeTransitionManager.Instance != null)//페이드 패널이 있으면
        {
            FadeTransitionManager.Instance.LoadScene(sceneToLoad);
        } */
        //없으면 그냥 넘어가기
        if(sceneToLoad == "Quit")//원래 이러면 안되는데 급해서 일단..!나중에 고쳐놓자..!
        {
            QuitGame();
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
            
        
        
        /*
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit != null && hit.gameObject == this.gameObject)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }*/
    }

    public void QuitGame()//게임종료 코드
    {
        Application.Quit();
        Debug.Log("게임을 종료합니다");
    }
}
