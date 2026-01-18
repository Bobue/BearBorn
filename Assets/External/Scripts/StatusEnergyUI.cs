using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEnergyUI : MonoBehaviour
{
    [SerializeField] private Image[] energySlots;
    [SerializeField] private Sprite fullSprite;
    [SerializeField] private Sprite emptySprite;

    [Header("Timer Text")]
    [SerializeField] private TMP_Text timerText;

    private void OnEnable()
    {
        if (EnergyManager.Instance == null) return;

        EnergyManager.Instance.OnEnergyChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDisable()
    {
        if (EnergyManager.Instance != null)
            EnergyManager.Instance.OnEnergyChanged -= UpdateUI;
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateUI()
    {
        int current = EnergyManager.Instance.CurrentEnergy;

        for (int i = 0; i < energySlots.Length; i++)
            energySlots[i].sprite = i < current ? fullSprite : emptySprite;

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (EnergyManager.Instance.CurrentEnergy >= EnergyManager.Instance.MaxEnergy)
        {
            timerText.text = "FULL";
            return;
        }

        TimeSpan remain = EnergyManager.Instance.GetRemainTime();

        timerText.text = string.Format(
            "{0:D2}:{1:D2}",
            remain.Minutes,
            remain.Seconds
        );
    }
}
