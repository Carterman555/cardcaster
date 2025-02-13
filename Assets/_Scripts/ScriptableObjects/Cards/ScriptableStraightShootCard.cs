using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

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
        Vector2 toShootDirection = shootPos - PlayerMovement.Instance.transform.position;
        toShootDirection.Normalize();
        Vector2 offset = spawnOffsetValue * toShootDirection;
        Vector2 spawnPos = (Vector2)PlayerMovement.Instance.transform.position + offset;

        // spawn and setup projectile
        StraightMovement straightMovement = projectilePrefab.Spawn(spawnPos, Containers.Instance.Projectiles);
        straightMovement.Setup(toShootDirection, Stats.ProjectileSpeed);
        straightMovement.GetComponent<DamageOnContact>().Setup(Stats.Damage, Stats.KnockbackStrength);

        // apply effect
        ApplyEffects(straightMovement);
    }

    public override void Stop() {
        base.Stop();
        abilityEffectPrefabs.Clear();
    }

    #region Effects

    public override void AddEffect(GameObject abilityEffectPrefab) {
        base.AddEffect(abilityEffectPrefab);
        abilityEffectPrefabs.Add(abilityEffectPrefab);
    }

    // applies the effects set by the modifier
    private void ApplyEffects(StraightMovement straightMovement) {

        foreach (var abilityEffectPrefab in abilityEffectPrefabs) {
            abilityEffectPrefab.Spawn(straightMovement.transform);
        }
    }

    #endregion
}
