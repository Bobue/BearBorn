using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    /*
    public GameObject[] characterObjects;
    private PlayerCharacter[] characters;
    private PlayerCharacter currentCharacter;
    */
    [SerializeField] private GameObject[] levelUpEffects;
    private string currentCharacterName;// 테스트용
    private Stats tempStats;

    [SerializeField] private Sprite[] characterImage;
    public Image[] characterImageUI;
    public Image[] LVGage;

    public Button[] PlusButtons;

    public TMP_Text txt_HP;
    public TMP_Text txt_MP;
    public TMP_Text txt_ATK;
    public TMP_Text txt_DEF;
    public TMP_Text txt_Name;
    public TMP_Text txt_LV1;
    public TMP_Text txt_LV2;
    public TMP_Text txt_LV3;

    public int Cindex;
    void Start()
    {
        int index = CharacterSelectData.SelectedCharacterIndex;
        Cindex = index;
        /*
        characters = new PlayerCharacter[characterObjects.Length];

        for (int i = 0; i < characterObjects.Length; i++)
        {
            characters[i] =
                characterObjects[i].GetComponentInChildren<PlayerCharacter>();

            if (characters[i] == null)
                Debug.LogError($"{characterObjects[i].name}에 PlayerCharacter 없음");
        }

        
        if (index < 0 || index >= characters.Length)
        {
            Debug.LogError("잘못된 캐릭터 인덱스");
            return;
        }
        
        currentCharacter = characters[index];

        // 임시 스탯 초기화
        tempStats = currentCharacter.BaseStats.Clone();
        */

        currentCharacterName = CharacterIdMapper.GetName(index);

        tempStats = CharacterPlayerHolder.Instance.GetStats(currentCharacterName).Clone();

        RefreshUI();
    }

    public void OnClickUpgradeStat(int index)
    {
        if (tempStats.LVPoint <= 0)
            return;

        switch (index)
        {
            case 0: tempStats.HP += 5; break;
            case 1: tempStats.MP += 5; break;
            case 2: tempStats.ATK += 5; break;
            case 3: tempStats.DEF += 5; break;
            default: return;
        }

        tempStats.LVPoint--;
        RefreshUI();
    }

    // 확인 버튼
    // 확인 버튼
    public void OnClickConfirm()
    {
        // 스탯 저장
        CharacterPlayerHolder.Instance.Setstats(currentCharacterName, tempStats);

        // 레벨업 이펙트 재생
        if (levelUpEffects[Cindex] != null)
        {
            levelUpEffects[Cindex].SetActive(false); // 먼저 끄고
            levelUpEffects[Cindex].SetActive(true);  // 켜서 재생
            StartCoroutine(DisableEffectAfterSeconds(levelUpEffects[Cindex], 1.5f)); // 1.5초 후 끄기
        }

        RefreshUI();
    }

    // 코루틴: 일정 시간 후 이펙트 끄기
    private IEnumerator DisableEffectAfterSeconds(GameObject effect, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (effect != null)
            effect.SetActive(false);
    }


    // 취소 버튼
    public void OnClickCancel()
    {
        tempStats = CharacterPlayerHolder.Instance.GetStats(currentCharacterName).Clone();
        RefreshUI();
    }

    // 플러스 버튼
    void UpdatePlusButtons()
    {
        bool canUpgrade = tempStats.LVPoint > 0;

        foreach (Button btn in PlusButtons)
            btn.interactable = canUpgrade;
    }

    void RefreshUI()
    {
        txt_HP.text = tempStats.HP.ToString();
        txt_MP.text = tempStats.MP.ToString();
        txt_ATK.text = tempStats.ATK.ToString();
        txt_DEF.text = tempStats.DEF.ToString();
        /*
        txt_Name.text = currentCharacter.name;
        txt_LV1.text = currentCharacter.BaseStats.LV.ToString();
        txt_LV2.text = "LV. " + currentCharacter.BaseStats.LV;
        */
        txt_Name.text = currentCharacterName;
        txt_LV1.text = tempStats.LV.ToString();
        txt_LV2.text = "LV. " + tempStats.LV;
        txt_LV3.text = "잔여 레벨 포인트 : " + tempStats.LVPoint;

        characterImageUI[0].sprite =
            characterImage[CharacterSelectData.SelectedCharacterIndex];
        characterImageUI[1].sprite =
            characterImage[CharacterSelectData.SelectedCharacterIndex];

        UpdateLVGage();
        UpdatePlusButtons();
    }

    void UpdateLVGage()
    {
        float currentExp = tempStats.Exp;
        float maxExp = tempStats.LV * 100f;

        float expPercent = Mathf.Clamp01(currentExp / maxExp);
        float totalGauge = expPercent * 4f;

        for (int i = 0; i < LVGage.Length; i++)
        {
            float value = totalGauge - i;

            if (value >= 1f)
                LVGage[i].fillAmount = 1f;
            else if (value > 0f)
                LVGage[i].fillAmount = value;
            else
                LVGage[i].fillAmount = 0f;
        }
    }
}
