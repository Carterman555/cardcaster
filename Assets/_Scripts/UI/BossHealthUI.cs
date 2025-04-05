using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private Image healthFill;

    private EnemyHealth bossHealth;

    private LocalizedString bossName;

    public void Setup(LocalizedString bossName, EnemyHealth bossHealth) {
        this.bossName = bossName;
        this.bossHealth = bossHealth;

        bossNameText.text = bossName.GetLocalizedString();

        healthFill.fillAmount = 1f;
    }

    private void OnEnable() {
        bossHealth.OnHealthChanged_HealthProportion += UpdateHealthBar;
        LocalizationSettings.SelectedLocaleChanged += UpdateBossText;
    }

    private void OnDisable() {
        bossHealth.OnHealthChanged_HealthProportion -= UpdateHealthBar;
        LocalizationSettings.SelectedLocaleChanged -= UpdateBossText;
    }

    private void UpdateHealthBar(float healthProportion) {
        healthFill.fillAmount = healthProportion;
    }

    private void UpdateBossText(Locale locale) {
        bossNameText.text = bossName.GetLocalizedString();
    }
}
