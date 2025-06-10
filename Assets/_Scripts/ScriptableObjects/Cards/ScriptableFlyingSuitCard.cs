using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlyingSuitCard", menuName = "Cards/Flying Suit Card")]
public class ScriptableFlyingSuitCard : ScriptablePersistentHandCard {

    [SerializeField] private StraightMovement cardProjectilePrefab;

    [SerializeField, Range(0f, 1f)] private float baseShootChance;
    [SerializeField, Range(0f, 1f)] private float shootChancePerLevel;
    [SerializeField] private float baseDamage;
    [SerializeField] private float baseKnockback;

    public float ShootPercentChance => (baseShootChance + (shootChancePerLevel * CurrentLevel)) * 100f;
    public override string Description => description.GetLocalizedString(this);

    public override void OnInstanceCreated() {
        base.OnInstanceCreated();

        PlayerMeleeAttack.Instance.OnBasicAttack += OnBasicAttack;
    }

    public override void OnRemoved() {
        base.OnRemoved();

        PlayerMeleeAttack.Instance.OnBasicAttack -= OnBasicAttack;
    }

    private void OnBasicAttack() {
        if (inHand) {
            float shootChance = baseShootChance + (shootChancePerLevel * CurrentLevel);
            if (shootChance > UnityEngine.Random.value) {
                StraightMovement cardProjectile = cardProjectilePrefab.Spawn(PlayerMovement.Instance.CenterPos, Containers.Instance.Projectiles);
                cardProjectile.Setup(PlayerMeleeAttack.Instance.GetAttackDirection());

                float damage = baseDamage * PlayerMovement.Instance.PlayerStats.ProjectileDamageMult;
                cardProjectile.GetComponent<DamageOnContact>().Setup(damage, baseKnockback, canCrit: true);
            }
        }
    }
}
