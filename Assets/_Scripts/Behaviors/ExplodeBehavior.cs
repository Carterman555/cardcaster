using System;
using UnityEngine;

public class ExplodeBehavior : MonoBehaviour, ITargetAttacker {

    public event Action OnAttack;
    public event Action<GameObject> OnDamage_Target;

    [SerializeField] private LayerMask targetLayerMask;

    [Header("Stats")]
    [SerializeField] private bool useIHasStats;
    private IHasEnemyStats hasEnemyStats;

    [SerializeField] private bool serializedExplosionRadius;
    [ConditionalHide("serializedExplosionRadius")] [SerializeField] private float explosionRadius;

    private float damage;
    private float knockbackStrength;

    [SerializeField] private bool dealDifferentDamageAmount;
    [ConditionalHide("dealDifferentDamageAmount")] [SerializeField] private LayerMask differentDamageLayerMask;
    [ConditionalHide("dealDifferentDamageAmount")] [SerializeField] private float differentDamageAmount;

    [Header("Visual")]
    [SerializeField] private ParticleSystem explosionParticlesPrefab;
    [SerializeField] private bool shakeCamera = true;

    private void Awake() {
        if (useIHasStats) {
            hasEnemyStats = GetComponent<IHasEnemyStats>();
        }
    }

    public void Explode(bool returnToPool = true) {

        // deal damage
        float damage = useIHasStats ? hasEnemyStats.EnemyStats.Damage : this.damage;
        float knockbackStrength = useIHasStats ? hasEnemyStats.EnemyStats.KnockbackStrength : this.knockbackStrength;
        Collider2D[] damagedColliders = DamageDealer.DealCircleDamage(targetLayerMask,
            transform.position,
            explosionRadius,
            damage,
            knockbackStrength);

        foreach (Collider2D col in damagedColliders) {
            OnDamage_Target?.Invoke(col.gameObject);
        }

        if (dealDifferentDamageAmount) {
            Collider2D[] differentDamagedColliders = DamageDealer.DealCircleDamage(differentDamageLayerMask,
                transform.position,
                explosionRadius,
                differentDamageAmount,
                knockbackStrength);

            foreach (Collider2D col in differentDamagedColliders) {
                OnDamage_Target?.Invoke(col.gameObject);
            }
        }

        // spawn particles
        if (explosionParticlesPrefab != null) {
            explosionParticlesPrefab.Spawn(transform.position, Containers.Instance.Effects);
        }

        if (shakeCamera) {
            CameraShaker.Instance.ShakeCamera(0.3f);
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Explode);

        OnAttack?.Invoke();
            
        // try to destroy this object
        if (returnToPool) {
            gameObject.ReturnToPool();
        }
    }

    public void SetDamage(float damage) {
        useIHasStats = false;
        this.damage = damage;
    }

    public void SetExplosionRadius(float radius) {
        explosionRadius = radius;
    }

    public void SetKnockbackStrength(float knockbackStrength) {
        this.knockbackStrength = knockbackStrength;
    }
}
