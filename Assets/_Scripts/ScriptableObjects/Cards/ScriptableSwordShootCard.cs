using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "SwordShootCard", menuName = "Cards/Sword Shoot Card")]
public class ScriptableSwordShootCard : ScriptableAbilityCardBase {

    [SerializeField] private StraightMovement swordHologramPrefab;
    [SerializeField] private float spawnOffsetValue;

    private List<GameObject> abilityEffectPrefabs = new();

    private Vector3 shootPos;

    protected override void Play(Vector2 position) {
        base.Play(position);

        shootPos = position;

        ShootSword();

        Stop();
    }

    private void ShootSword() {

        // get direction to shoot
        Vector2 toShootDirection = shootPos - PlayerMovement.Instance.transform.position;
        toShootDirection.Normalize();
        Vector2 offset = spawnOffsetValue * toShootDirection;
        Vector2 spawnPos = (Vector2)PlayerMovement.Instance.transform.position + offset;

        // spawn and setup dagger
        StraightMovement straightMovement = swordHologramPrefab.Spawn(spawnPos, Containers.Instance.Projectiles);
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
