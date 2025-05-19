using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StraightShootCard", menuName = "Cards/Straight Shoot Card")]
public class ScriptableStraightShootCard : ScriptableAbilityCardBase {

    [SerializeField] private StraightMovement projectilePrefab;
    [SerializeField] private float spawnOffsetValue;

    private List<GameObject> abilityEffectPrefabs = new();

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
        straightMovement.GetComponent<DamageOnContact>().Setup(damage, Stats.KnockbackStrength, canCrit: true);

        // apply effect
        ApplyEffects(straightMovement);
    }

    public override void Stop() {
        base.Stop();
        abilityEffectPrefabs.Clear();
    }

    #region Effects

    public override void ApplyModifier(ScriptableModifierCardBase modifierCard) {
        base.ApplyModifier(modifierCard);
        if (modifierCard.AppliesEffect) {
            abilityEffectPrefabs.Add(modifierCard.EffectPrefab);
        }
    }

    // applies the effects set by the modifier
    private void ApplyEffects(StraightMovement straightMovement) {

        foreach (var abilityEffectPrefab in abilityEffectPrefabs) {
            abilityEffectPrefab.Spawn(straightMovement.transform);
        }
    }

    #endregion
}
