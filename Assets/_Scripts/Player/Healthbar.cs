using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    [SerializeField] private Image fill;

    private void OnEnable() {
        health.OnHealthChanged_HealthProportion += UpdateHealthBar;
    }
    private void OnDisable() {
        health.OnHealthChanged_HealthProportion -= UpdateHealthBar;
    }

    private void UpdateHealthBar(float proportion) {
        fill.fillAmount = proportion;
    }
}
