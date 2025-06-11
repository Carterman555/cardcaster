using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour, IHasEnemyStats {

    [SerializeField] private EnemyStats enemyStats;
    public EnemyStats GetEnemyStats() {
        return enemyStats;
    }

    private BounceMoveBehaviour bounceMovement;
    private Knockback knockback;
    private CircleStraightShootBehavior shootBehavior;
    private EnemyHealth health;

    [SerializeField] private float activateDelay;
    private Invincibility startingInvincibility;

    [SerializeField] private ParticleSystem bounceParticles;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private Transform centerPoint;

    private void Awake() {
        bounceMovement = GetComponent<BounceMoveBehaviour>();
        knockback = GetComponent<Knockback>();
        shootBehavior = GetComponent<CircleStraightShootBehavior>();
        health = GetComponent<EnemyHealth>();
    }

    private void OnEnable() {
        knockback.OnKnockbacked += Bounce;

        bounceMovement.OnBounce += BounceParticles;
        health.DeathEventTrigger.AddListener(DeathParticles);

        startingInvincibility = gameObject.AddComponent<Invincibility>();
        bounceMovement.enabled = false;
        shootBehavior.enabled = false;

        StartCoroutine(ActivateAfterDelay());
    }

    private void OnDisable() {
        knockback.OnKnockbacked -= Bounce;

        bounceMovement.OnBounce -= BounceParticles;
        health.DeathEventTrigger.RemoveListener(DeathParticles);
    }

    private IEnumerator ActivateAfterDelay() {

        yield return new WaitForSeconds(activateDelay);

        Destroy(startingInvincibility);
        bounceMovement.enabled = true;
        shootBehavior.enabled = true;

        float startingAttackCooldown = UnityEngine.Random.Range(0f, enemyStats.AttackCooldown);
        shootBehavior.SetActionTimer(startingAttackCooldown);
    }

    private void Bounce(Vector2 direction) {
        bounceMovement.ForceBounce(-direction);
    }

    private void BounceParticles() {
        ParticleSystem newBounceParticles = bounceParticles.Spawn(centerPoint.position, Containers.Instance.Effects);
        newBounceParticles.Play();
    }

    private void DeathParticles() {
        ParticleSystem newDeathParticles = deathParticles.Spawn(centerPoint.position, Containers.Instance.Effects);
        newDeathParticles.Play();
    }
}
