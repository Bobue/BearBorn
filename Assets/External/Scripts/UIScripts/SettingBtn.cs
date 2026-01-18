using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingBtn : MonoBehaviour
{
    public GameObject Setting_Panel;
    public GameObject Sound_Panel;
    public GameObject GameExit_Panel;

    [System.Serializable]
    public class SelectableButton
    {
        public Button button;
        public Sprite normalSprite;
        public Sprite selectedSprite;
    }
    

    public SelectableButton buttonA;
    public SelectableButton buttonB;
    private SelectableButton currentSelected;

    private void Start()
    {
        Setting_Panel.SetActive(false);
        Sound_Panel.SetActive(false);
        GameExit_Panel.SetActive(false);

        buttonA.button.onClick.AddListener(() => Select(buttonA));
        buttonB.button.onClick.AddListener(() => Select(buttonB));
        Select(buttonA);//초기선택
    }

    
    void Select(SelectableButton target)
    {
        if(currentSelected !=null)
        {
            currentSelected.button.image.sprite = currentSelected.normalSprite;
        }

        currentSelected = target;
        currentSelected.button.image.sprite = currentSelected.selectedSprite;
    }


    public void SoundOpen()
    {
        Setting_Panel.SetActive(true);
        Sound_Panel.SetActive(true);
        GameExit_Panel.SetActive(false);
    }
    public void GameExitOpen()
    {
        Setting_Panel.SetActive(true);
        Sound_Panel.SetActive(false);
        GameExit_Panel.SetActive(true);
    }
    public void Cancel()
    {
        Setting_Panel.SetActive(false);
        Sound_Panel.SetActive(true);
        GameExit_Panel.SetActive(false);
    }
    public void GameExit()
    {
        Application.Quit();
    }

}
