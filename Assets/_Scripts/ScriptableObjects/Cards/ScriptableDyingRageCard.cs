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
            StatsManager.RemovePlayerStatModifiers(statModifiers);
            applyingDamageModifier = false;
        }
    }

    // this way of doing things will lead to problems if one dying rage card is played multiple times
    // and it can stack. but I have set dying rage to 'reset duration' instead of 'stackable'
    private void UpdateDamage(float proportion) {

        float maxHealthProportionForDamage = 0.25f;

        bool shouldApplyDamage = proportion < maxHealthProportionForDamage;

        if (shouldApplyDamage && !applyingDamageModifier) {
            StatsManager.AddPlayerStatModifiers(statModifiers);
            applyingDamageModifier = true;
        }
        else if (!shouldApplyDamage && applyingDamageModifier) {
            StatsManager.RemovePlayerStatModifiers(statModifiers);
            applyingDamageModifier = false;
        }
    }
}
