using UnityEngine;
using System;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance;

    [SerializeField] private int maxEnergy = 5;
    [SerializeField] private int currentEnergy = 5;

    public int CurrentEnergy => currentEnergy;
    public int MaxEnergy => maxEnergy;

    public event Action OnEnergyChanged;

    private const int RECOVER_MINUTES = 15;
    private DateTime lastRecoverTime;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadEnergy();
        CalculateOfflineRecovery();
    }

    private void Update()
    {
        TryRecoverEnergy();

        if (Input.GetKeyDown(KeyCode.Space))//잠시 테스트용으로 함수 추가
        {
            currentEnergy = 5;
            Debug.Log("스페이스바로 MP가 에너지가 가득 찼습니다!");
        }
    }
    public TimeSpan GetRemainTime()
    {
        if (currentEnergy >= maxEnergy)
            return TimeSpan.Zero;

        DateTime nextRecoverTime = lastRecoverTime.AddMinutes(RECOVER_MINUTES);
        TimeSpan remain = nextRecoverTime - DateTime.Now;

        return remain.TotalSeconds < 0 ? TimeSpan.Zero : remain;
    }


    // 스테이지 진입 시 호출
    public bool ConsumeForStage()
    {
        if (currentEnergy <= 0)
            return false;

        bool wasFull = currentEnergy == maxEnergy;

        currentEnergy--;

        // MAX → 감소 시점에만 회복 타이머 시작
        if (wasFull)
            lastRecoverTime = DateTime.Now;

        SaveEnergy();
        OnEnergyChanged?.Invoke();
        return true;
    }

    private void TryRecoverEnergy()
    {
        if (currentEnergy >= maxEnergy)
            return;

        TimeSpan elapsed = DateTime.Now - lastRecoverTime;
        int recoverCount = (int)(elapsed.TotalMinutes / RECOVER_MINUTES);

        if (recoverCount > 0)
            RecoverEnergy(recoverCount);
    }

    private void RecoverEnergy(int amount)
    {
        int before = currentEnergy;
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);

        int actualRecovered = currentEnergy - before;
        if (actualRecovered <= 0) return;

        if (currentEnergy >= maxEnergy)
        {
            lastRecoverTime = DateTime.Now;
        }
        else
        {
            lastRecoverTime = lastRecoverTime.AddMinutes(actualRecovered * RECOVER_MINUTES);
        }

        SaveEnergy();
        OnEnergyChanged?.Invoke();
    }


    private void SaveEnergy()
    {
        PlayerPrefs.SetInt("ENERGY", currentEnergy);
        PlayerPrefs.SetString("ENERGY_TIME", lastRecoverTime.ToString());
        PlayerPrefs.Save();
    }

    private void LoadEnergy()
    {
        currentEnergy = PlayerPrefs.GetInt("ENERGY", maxEnergy);

        if (PlayerPrefs.HasKey("ENERGY_TIME"))
            lastRecoverTime = DateTime.Parse(PlayerPrefs.GetString("ENERGY_TIME"));
        else
            lastRecoverTime = DateTime.Now;
    }

    private void CalculateOfflineRecovery()
    {
        if (currentEnergy >= maxEnergy)
            return;

        TimeSpan elapsed = DateTime.Now - lastRecoverTime;
        int recoverCount = (int)(elapsed.TotalMinutes / RECOVER_MINUTES);

        if (recoverCount > 0)
            RecoverEnergy(recoverCount);
    }


}
