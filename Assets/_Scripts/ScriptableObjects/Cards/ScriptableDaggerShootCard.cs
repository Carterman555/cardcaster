
using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "DaggerShootCard", menuName = "Cards/Dagger Shoot Card")]
public class ScriptableDaggerShootCard : ScriptableAbilityCardBase {

    [SerializeField] private StraightMovement daggerPrefab;
    [SerializeField] private float spawnOffsetValue;

    private List<GameObject> abilityEffectPrefabs = new();

    private Coroutine shootCoroutine;

    protected override void Play(Vector2 position) {
        base.Play(position);

        shootCoroutine = DeckManager.Instance.StartCoroutine(ShootDaggers());
    }

    public override void Stop() {
        base.Stop();

        abilityEffectPrefabs.Clear();

        DeckManager.Instance.StopCoroutine(shootCoroutine);
    }

    private IEnumerator ShootDaggers() {
        while (true) {
            yield return new WaitForSeconds(Stats.Cooldown);

            // get direction to shoot
            Vector2 attackDirection = PlayerMeleeAttack.Instance.GetAttackDirection();
            Vector2 offset = spawnOffsetValue * attackDirection;
            Vector2 spawnPos = (Vector2)PlayerMovement.Instance.CenterPos + offset;

            // spawn and setup dagger
            StraightMovement straightMovement = daggerPrefab.Spawn(spawnPos, Containers.Instance.Projectiles);
            straightMovement.Setup(attackDirection, Stats.ProjectileSpeed);
            straightMovement.GetComponent<DamageOnContact>().Setup(Stats.Damage, Stats.KnockbackStrength);

            // apply effect
            ApplyEffects(straightMovement);
        }
    }

    #region Effects

    public override void ApplyModifier(AbilityStats statsModifier, AbilityAttribute abilityAttributesToModify, GameObject effectPrefab) {
        base.ApplyModifier(statsModifier, abilityAttributesToModify, effectPrefab);
        abilityEffectPrefabs.Add(effectPrefab);
    }

    // applies the effects set by the modifier
    private void ApplyEffects(StraightMovement straightMovement) {

        foreach (var abilityEffectPrefab in abilityEffectPrefabs) {
            abilityEffectPrefab.Spawn(straightMovement.transform);
        }
    }

    #endregion
}
