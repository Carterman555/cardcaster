using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ExplodeBehavior : MonoBehaviour, ITargetAttacker {

    public event Action OnAttack;
    public event Action<GameObject> OnDamage_Target;

    [SerializeField] private LayerMask targetLayerMask;

    [Header("Stats")]
    [SerializeField] private bool useIHasStats;
    private IHasStats iHasStats;

    [SerializeField] private bool serializedExplosionRadius;
    [ConditionalHide("serializedExplosionRadius")] [SerializeField] private float explosionRadius;

    private float damage;
    private float knockbackStrength;


    [Header("Visual")]
    [SerializeField] private ParticleSystem explosionParticlesPrefab;
    [SerializeField] private bool shakeCamera = true;

    private void Awake() {
        if (useIHasStats) {
            iHasStats = GetComponent<IHasStats>();
        }
    }

    public void Explode(bool returnToPool = true) {

        // deal damage
        float dmg = useIHasStats ? iHasStats.GetStats().Damage : damage;
        Collider2D[] damagedColliders = DamageDealer.DealCircleDamage(targetLayerMask,
            transform.position,
            explosionRadius,
            dmg,
            knockbackStrength);
        
        // spawn particles
        if (explosionParticlesPrefab != null) {
            explosionParticlesPrefab.Spawn(transform.position, Containers.Instance.Effects);
        }

        if (shakeCamera) {
            CameraShaker.Instance.ShakeCamera(0.3f);
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Explode);

        // invoke events
        OnAttack?.Invoke();
        foreach (Collider2D col in damagedColliders) {
            OnDamage_Target?.Invoke(col.gameObject);
        }

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
