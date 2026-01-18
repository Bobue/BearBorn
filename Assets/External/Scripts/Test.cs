using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Dialogue
{
    [TextArea]
    public string dialogue;
    public Sprite cg;
}

public class Test : MonoBehaviour
{
    public static event Action OnCutsceneFinished; // ⭐ 컷신 종료 신호

    [SerializeField] private Image img_StandingCG;
    [SerializeField] private Image img_DialogueBox;
    [SerializeField] private TextMeshProUGUI txt_Dialogue;

    [SerializeField] private Dialogue[] dialogue;

    private bool isDialoguePlaying = false;
    private int count = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        ShowDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isDialoguePlaying)
            {
                ShowDialogue();
            }
            else
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    txt_Dialogue.text = dialogue[count - 1].dialogue;
                    typingCoroutine = null;
                }
                else
                {
                    if (count < dialogue.Length)
                    {
                        NextDialogue();
                    }
                    else
                    {
                        EndCutscene(); // ⭐ 여기
                    }
                }
            }
        }
    }

    private void EndCutscene()
    {
        OnOff(false);
        OnCutsceneFinished?.Invoke(); // ⭐ 컷신 끝났다고 알림
    }

    private void OnOff(bool flag)
    {
        img_DialogueBox.gameObject.SetActive(flag);
        img_StandingCG.gameObject.SetActive(flag);
        txt_Dialogue.gameObject.SetActive(flag);
        isDialoguePlaying = flag;
    }

    public void ShowDialogue()
    {
        OnOff(true);
        count = 0;
        NextDialogue();
    }

    private void NextDialogue()
    {
        img_StandingCG.sprite = dialogue[count].cg;
        typingCoroutine = StartCoroutine(TypeText(dialogue[count].dialogue));
        count++;
    }

    private IEnumerator TypeText(string text)
    {
        txt_Dialogue.text = "";
        foreach (char c in text)
        {
            txt_Dialogue.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        typingCoroutine = null;
    }
}
