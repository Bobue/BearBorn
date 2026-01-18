using UnityEngine;
using UnityEngine.UI;

public class HelpButton : MonoBehaviour
{
    [SerializeField] private GameObject[] Helps;
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject Help;

    private void Start()
    {
        // 버튼에 클릭 이벤트 연결
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // 클로저 방지
            buttons[i].onClick.AddListener(() => ActiveHelp(index));
        }

        // 시작 시 전부 꺼두기
        DeactivateAllHelps();
    }

    void ActiveHelp(int index)
    {
        DeactivateAllHelps();

        if (index >= 0 && index < Helps.Length)
        {
            Helps[index].SetActive(true);
        }
    }

    void DeactivateAllHelps()
    {
        foreach (var help in Helps)
        {
            help.SetActive(false);
        }
    }
    public void ExitHelp()
    {
        Help.SetActive(false);
    }
}
