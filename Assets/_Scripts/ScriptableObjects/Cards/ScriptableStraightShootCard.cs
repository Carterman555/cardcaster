using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StraightShootCard", menuName = "Cards/Straight Shoot Card")]
public class ScriptableStraightShootCard : ScriptableAbilityCardBase {

    [SerializeField] private StraightMovement projectilePrefab;
    [SerializeField] private float spawnOffsetValue;

    private Vector3 shootPos;

    protected override void Play(Vector2 position) {
        base.Play(position);

        shootPos = position;

        Shoot();

        Stop();
    }

    private void Shoot() {

        // get direction to shoot
        Vector2 toShootDirection = shootPos - PlayerMovement.Instance.CenterPos;
        toShootDirection.Normalize();
        Vector2 offset = spawnOffsetValue * toShootDirection;
        Vector2 spawnPos = (Vector2)PlayerMovement.Instance.CenterPos + offset;

        // spawn and setup projectile
        StraightMovement straightMovement = projectilePrefab.Spawn(spawnPos, Containers.Instance.Projectiles);

        straightMovement.Setup(toShootDirection, Stats.ProjectileSpeed);

        float damage = Stats.Damage * StatsManager.PlayerStats.BaseProjectileDamageMult;
        straightMovement.GetComponent<DamageOnContact>().Setup(damage, Stats.KnockbackStrength);

        // apply effect
        ApplyEffects(straightMovement.transform);
    }

    public override void Stop() {
        base.Stop();
        effectModifiers.Clear();
    }

    #region Effects

    private List<EffectModifier> effectModifiers = new();

    public override void ApplyModifier(ScriptableModifierCardBase modifierCard) {
        base.ApplyModifier(modifierCard);
        if (modifierCard.AppliesEffect) {
            effectModifiers.Add(modifierCard.EffectModifier);
        }
    }

    private void ApplyEffects(Transform attacker) {
        foreach (EffectModifier effectModifier in effectModifiers) {
            effectModifier.EffectLogicPrefab.Spawn(attacker);

            if (effectModifier.HasVisual) {
                Transform visualTransform = attacker.Find("Visual");
                if (visualTransform == null) {
                    Debug.LogError($"StraightShoot projectile {attacker.name} does not have child with name 'Visual'!");
                    return;
                }

                effectModifier.EffectVisualPrefab.Spawn(visualTransform);
            }
        }
    }

    #endregion
}
