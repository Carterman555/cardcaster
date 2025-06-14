using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DaggerShootCard", menuName = "Cards/Dagger Shoot Card")]
public class ScriptableDaggerShootCard : ScriptableAbilityCardBase {

    [SerializeField] private StraightMovement daggerPrefab;
    [SerializeField] private float spawnOffsetValue;

    private Coroutine shootCoroutine;

    protected override void Play(Vector2 position) {
        base.Play(position);

        shootCoroutine = DeckManager.Instance.StartCoroutine(ShootDaggers());
    }

    public override void Stop() {
        base.Stop();

        effectModifiers.Clear();

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

            float damage = Stats.Damage * StatsManager.PlayerStats.BaseProjectileDamageMult;
            straightMovement.GetComponent<DamageOnContact>().Setup(damage, Stats.KnockbackStrength);

            // apply effect
            ApplyEffects(straightMovement);
        }
    }

    #region Effects

    private List<EffectModifier> effectModifiers = new();

    public override void ApplyModifier(ScriptableModifierCardBase modifierCard) {
        base.ApplyModifier(modifierCard);
        if (modifierCard.AppliesEffect) {
            effectModifiers.Add(modifierCard.EffectModifier);
        }
    }

    // applies the effects set by the modifier
    private void ApplyEffects(StraightMovement straightMovement) {
        foreach (EffectModifier effectModifier in effectModifiers) {
            effectModifier.EffectLogicPrefab.Spawn(straightMovement.transform);

            if (effectModifier.HasVisual) {
                Transform visualTransform = straightMovement.transform.Find("Visual");
                if (visualTransform == null) {
                    Debug.LogError($"StraightShoot projectile {straightMovement.name} does not have child with name 'Visual'!");
                    return;
                }

                effectModifier.EffectVisualPrefab.Spawn(visualTransform);
            }
        }
    }

    #endregion
}
