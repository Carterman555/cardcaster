using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class BossHealthUI : StaticInstance<BossHealthUI>, IInitializable {

    public void Initialize() {
        Instance = this;
    }

    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private Image healthFill;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color invincibleColor;

    private EnemyHealth bossHealth;

    private LocalizedString bossName;

    public bool RemainAtSliver { get; set; }

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

        if (RemainAtSliver) {
            float sliverHealthProportion = 0.005f;
            healthFill.fillAmount = Mathf.Max(healthProportion, sliverHealthProportion);
        }
    }

    private void UpdateBossText(Locale locale) {
        bossNameText.text = bossName.GetLocalizedString();
    }

    public void SetInvincible(bool invincible) {
        Color newColor = invincible ? invincibleColor : normalColor;

        healthFill.DOKill();
        healthFill.DOColor(newColor, duration: 0.3f);
    }
}
