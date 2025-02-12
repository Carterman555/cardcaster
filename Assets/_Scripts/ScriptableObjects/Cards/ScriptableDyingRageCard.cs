using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DyingRageCard", menuName = "Cards/Dying Rage")]
public class ScriptableDyingRageCard : ScriptableAbilityCardBase {

    [SerializeField] private PlayerStatsModifier statsModifier;

    private bool applyingDamageModifier;

    private Health playerHealth;

    protected override void Play(Vector2 position) {
        base.Play(position);

        applyingDamageModifier = false;

        playerHealth = PlayerMeleeAttack.Instance.GetComponent<Health>();

        playerHealth.OnHealthChanged_HealthProportion += UpdateDamage;
        UpdateDamage(playerHealth.GetHealthProportion());
    }

    public override void Stop() {
        base.Stop();

        playerHealth.OnHealthChanged_HealthProportion -= UpdateDamage;
        
        if (applyingDamageModifier) {
            StatsManager.Instance.RemovePlayerStatsModifier(statsModifier);
            applyingDamageModifier = false;
        }
    }

    private void UpdateDamage(float proportion) {

        float maxHealthProportionForDamage = 0.25f;

        bool shouldApplyDamage = proportion < maxHealthProportionForDamage;

        if (shouldApplyDamage && !applyingDamageModifier) {
            StatsManager.Instance.AddPlayerStatsModifier(statsModifier);
            applyingDamageModifier = true;
        }
        else if (!shouldApplyDamage && applyingDamageModifier) {
            StatsManager.Instance.RemovePlayerStatsModifier(statsModifier);
            applyingDamageModifier = false;
        }
    }
}
