using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CharactorAnim : MonoBehaviour
{
    [Header("Button Sound")]
    public AudioSource buttonAudioSource; // Inspector에서 연결
    public AudioClip buttonClickClip;     // Inspector에서 연결
    public AudioSource failbuttonAudioSource; // Inspector에서 연결
    public AudioClip failbuttonClickClip;     // Inspector에서 연결
    public GameObject[] charactersAnim;
    public PlayerCharacter[] characters;
    [SerializeField] private Sprite[] skillSprites;
    [SerializeField] private Sprite[] semiBackground1;
    [SerializeField] private Sprite[] semiBackground2;
    [SerializeField] private Sprite[] buttonImage;
    [SerializeField] private Sprite[] characterImage;
    [SerializeField] private GameObject Helpbtn;

    [SerializeField] private GameObject[] toggleObjects;
    private bool isObjectActive = false;

    private int currentIndex = 0;

    public GameObject statusUI;
    public TMP_Text txt_HP; 
    public TMP_Text txt_ATK;
    public TMP_Text txt_DEF;
    public TMP_Text txt_MP;
    public TMP_Text Name;

    public Image SkillImage;
    public Image Characters;
    public Image ImgsemiBackground1;
    public Image ImgsemiBackground2;
    [SerializeField] private Button[] UpGradeButton;
    public Image SeperateImg;

    private Animator currentAnimator;
    private Coroutine animCoroutine;
    public float idleTime = 5f;
    private int randomAnimCount = 2;

    public GameObject InventoryPanel;

    void Start()
    {
        if(Helpbtn!=null)
            Helpbtn.SetActive(false);
        // 처음엔 UI 꺼두기
        if (statusUI != null)
            statusUI.SetActive(false);
        characters = new PlayerCharacter[charactersAnim.Length];

        for (int i = 0; i < charactersAnim.Length; i++) // 0: verka 1: marshika 2: chirr
        {
            characters[i] =
                charactersAnim[i].GetComponentInChildren<PlayerCharacter>();

            if (characters[i] == null)
                Debug.LogError($"{charactersAnim[i].name}에 PlayerCharacter 없음");
        }


        // 기본 캐릭터만 켜기
        for (int i = 0; i < charactersAnim.Length; i++)
            charactersAnim[i].SetActive(i == currentIndex);

        currentAnimator = charactersAnim[currentIndex].GetComponent<Animator>();
        animCoroutine = StartCoroutine(RandomAnimationRoutine());
    }

    void Update()
    {
        CheckClickCharacter();
    }
    public void ToggleCharacterObject()
    {
        if (toggleObjects == null || toggleObjects.Length < 3)
        {
            Debug.LogWarning("toggleObjects 배열 설정 안됨");
            return;
        }

        GameObject target = toggleObjects[currentIndex];

        if (target == null) return;

        isObjectActive = !isObjectActive;
        target.SetActive(isObjectActive);
    }

    public int getCurrentIndex()
    {
        return currentIndex;
    }
    void CheckClickCharacter()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        if (InventoryPanel != null && InventoryPanel.activeSelf)
            return; // 인벤토리가 열려있으면 클릭 이벤트 무시

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == charactersAnim[currentIndex])
            {
                ShowStatusUI(characters[currentIndex]);
            }
        }
    }

    // 클릭된 캐릭터의 스탯을 UI에 표시
    void ShowStatusUI(PlayerCharacter pc) // 0: verka 1: marshika 2: chirr
    {
        if (pc != null)
        {
           
            txt_HP.text = pc.BaseStats.HP.ToString();
            txt_MP.text = pc.BaseStats.MP.ToString();
            txt_ATK.text = pc.BaseStats.ATK.ToString();
            txt_DEF.text = pc.BaseStats.DEF.ToString();
            Name.text = pc.name;
            SkillImage.sprite = skillSprites[currentIndex];
            ImgsemiBackground1.sprite = semiBackground1[currentIndex];
            ImgsemiBackground2.sprite = semiBackground2[currentIndex];
            UpGradeButton[0].image.sprite = buttonImage[currentIndex];
            UpGradeButton[1].image.sprite = buttonImage[currentIndex];
            Characters.sprite = characterImage[currentIndex];
            UpdateSeperateColor();
        }
        else
            print("널임ㅅㄱ");
            statusUI.SetActive(true);
    }

    IEnumerator RandomAnimationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(idleTime);

            int rand = Random.Range(0, randomAnimCount);
            currentAnimator.SetInteger("RandomIndex", rand);
            currentAnimator.SetTrigger("PlayRandom");

            float animLength = currentAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength);
        }
    }
    public void PlayButtonClickSound()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip);
        }
    }
    public void PlayFailButtonClickSound()
    {
        if (failbuttonAudioSource != null && failbuttonClickClip != null)
        {
            failbuttonAudioSource.PlayOneShot(failbuttonClickClip);
        }
    }
    public void ExitStatus()
    {
        statusUI.SetActive(false);
    }
    public void Go1_1()
    {
        SceneManager.LoadScene("RadgaReactor");
    }
    Color HexToColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#" + hex, out color);
        return color;
    }

    void UpdateSeperateColor()
    {
        if (currentIndex == 0)
            SeperateImg.color = HexToColor("4AFFFF");
        else if (currentIndex == 1)
            SeperateImg.color = HexToColor("8ED973");
        else if (currentIndex == 2)
            SeperateImg.color = HexToColor("C00000");
    }

    public void OpenHelp()
    {
        Helpbtn.SetActive(true);
    }
    public void SwitchCharacter()
    {
        charactersAnim[currentIndex].SetActive(false);

        currentIndex = (currentIndex + 1) % charactersAnim.Length;

        charactersAnim[currentIndex].SetActive(true);
        currentAnimator = charactersAnim[currentIndex].GetComponent<Animator>();

        // UI 끄기
        statusUI.SetActive(false);

        // 애니메이션 루틴 재시작
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        animCoroutine = StartCoroutine(RandomAnimationRoutine());
    }
}
