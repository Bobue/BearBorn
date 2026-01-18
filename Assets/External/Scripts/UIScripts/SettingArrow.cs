using TMPro; // 반드시 추가
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class SettingArrow : MonoBehaviour
{
    public Slider Slider_Setting, Slider_Stamina;
    public TMP_Text StaminaPoint; 
    public Button Btn_Ex1, Down_Arrow, Up_Arrow;
    [SerializeField] private CharactorAnim charactorAnim;


    private Coroutine slideCoroutine;

    public void UpdateStamina(float value)
    {
        int displayedValue = Mathf.RoundToInt(value); // 소수점 반올림해서 정수로 변환
        StaminaPoint.text = displayedValue.ToString();
    }


    public void DownArrow()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        Down_Arrow.gameObject.SetActive(false);
        slideCoroutine = StartCoroutine(SmoothSliderValue(1f, 0.2f, () =>
        {
            Btn_Ex1.gameObject.SetActive(true);
            Up_Arrow.gameObject.SetActive(true);
        }));
    }

    public void UpArrow()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);

        Btn_Ex1.gameObject.SetActive(false);
        Up_Arrow.gameObject.SetActive(false);

        slideCoroutine = StartCoroutine(SmoothSliderValue(0f, 0.2f, () =>
        {
            Down_Arrow.gameObject.SetActive(true);
        }));
    }

    private IEnumerator SmoothSliderValue(float targetValue, float duration, System.Action onComplete)
    {
        float startValue = Slider_Setting.value;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            Slider_Setting.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }

        Slider_Setting.value = targetValue;
        slideCoroutine = null;

        onComplete?.Invoke();
    }

    public void MoveUpgrade(int index)
    {
        MenuSlider.StartPageIndex = index;
        CharacterSelectData.SelectedCharacterIndex = charactorAnim.getCurrentIndex();
        SceneManager.LoadScene("Upgrade");
    }

}
