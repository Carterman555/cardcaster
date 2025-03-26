using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DyingRageCard", menuName = "Cards/Dying Rage")]
public class ScriptableDyingRageCard : ScriptableAbilityCardBase {

    [SerializeField] private PlayerStatModifier[] statModifiers;

    private bool applyingDamageModifier;

    private PlayerHealth playerHealth;

    protected override void Play(Vector2 position) {
        base.Play(position);

        applyingDamageModifier = false;

        playerHealth = PlayerMeleeAttack.Instance.GetComponent<PlayerHealth>();

        playerHealth.OnHealthChanged_HealthProportion += UpdateDamage;
        UpdateDamage(playerHealth.HealthProportion);
    }

    public override void Stop() {
        base.Stop();

        playerHealth.OnHealthChanged_HealthProportion -= UpdateDamage;
        
        if (applyingDamageModifier) {
            StatsManager.Instance.RemovePlayerStatModifiers(statModifiers);
            applyingDamageModifier = false;
        }
    }

    private void UpdateDamage(float proportion) {

        float maxHealthProportionForDamage = 0.25f;

        bool shouldApplyDamage = proportion < maxHealthProportionForDamage;

        if (shouldApplyDamage && !applyingDamageModifier) {
            StatsManager.Instance.RemovePlayerStatModifiers(statModifiers);
            applyingDamageModifier = true;
        }
        else if (!shouldApplyDamage && applyingDamageModifier) {
            StatsManager.Instance.RemovePlayerStatModifiers(statModifiers);
            applyingDamageModifier = false;
        }
    }
}
