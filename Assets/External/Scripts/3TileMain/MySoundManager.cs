using UnityEngine;

public class MySoundManager : MonoBehaviour
{
    public static MySoundManager Instance;

    [Header("Tile Weapon")]
    public AudioClip ArrowSheildTileSelectClip;   // 석궁 타일 선택

    public AudioClip ArrowFireClip;          // 석궁 발사
    public AudioClip ShieldBreakClip;        // 방패 파괴

    [Header("Turn / Battle")]
    public AudioClip TurnChangeClip;
    public AudioClip NormalAttackClip;
    public AudioClip StageClearClip;
    public AudioClip OneHundredManaClip;

    [Header("Skill")]
    public AudioClip VerkaSkillClip;
    public AudioClip MarshikaSkillClip;
    public AudioClip ChirrSkillClip;

    [Header("Drop")]
    public AudioClip DropClip;
    public AudioClip DropMismatchClip;
    public AudioClip DropMatchClip;

    [Header("Card")]
    public AudioClip CardSelectClip;
    public AudioClip CardDecideClip;

    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 🔊 효과음용 AudioSource 자동 생성
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================
    // 공통 재생 메서드
    // =========================
    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // =========================
    // 외부 호출용 메서드
    // =========================
    public void PlayTurnChange() => PlaySFX(TurnChangeClip);
    public void PlayNormalAttack() => PlaySFX(NormalAttackClip);
    public void PlayStageClear() => PlaySFX(StageClearClip);
    public void PlayOneHundredMana() => PlaySFX(OneHundredManaClip);
    public void PlayArrowTileSelect() => PlaySFX(ArrowSheildTileSelectClip);

    public void PlayArrowFire() => PlaySFX(ArrowFireClip);
    public void PlayShieldBreak() => PlaySFX(ShieldBreakClip);

    public void PlayVerkaSkill() => PlaySFX(VerkaSkillClip);
    public void PlayMarshikaSkill() => PlaySFX(MarshikaSkillClip);
    public void PlayChirrSkill() => PlaySFX(ChirrSkillClip);

    public void PlayDrop() => PlaySFX(DropClip);
    public void PlayDropMismatch() => PlaySFX(DropMismatchClip);
    public void PlayDropMatch() => PlaySFX(DropMatchClip);

    public void PlayCardSelect() => PlaySFX(CardSelectClip);
    public void PlayCardDecide() => PlaySFX(CardDecideClip);
}
