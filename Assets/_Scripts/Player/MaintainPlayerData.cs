using UnityEngine;

public class MaintainPlayerData : MonoBehaviour {

    [SerializeField] private PlayerHealth health;
    private static float healthAmount;
    private static bool healthSet;

    private void OnEnable() {
        GameSceneManager.OnLevelComplete += UpdateHealth;

        if (healthSet) {
            health.CurrentHealth = healthAmount;
        }
    }
    private void OnDisable() {
        GameSceneManager.OnLevelComplete -= UpdateHealth;
    }

    private void UpdateHealth(int level) {
        healthAmount = health.CurrentHealth;
        healthSet = true;
    }
}
