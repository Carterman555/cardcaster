using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ExplosionTarget {
    public LayerMask LayerMask;
    public float ExplosionRadius;
    public float Damage;
    public float KnockbackStrength;
}

public class ExplodeBehavior : MonoBehaviour, ITargetAttacker {

    public event Action OnAttack;
    public event Action<GameObject> OnDamage_Target;

    // so can do different damage to different targets and add target through inspector or scripts
    [SerializeField] private List<ExplosionTarget> serializedExplosionTargets;
    public List<ExplosionTarget> AddedExplosionTargets { get; set; } = new();

    [Header("Visual")]
    [SerializeField] private ParticleSystem explosionParticlesPrefab;
    [SerializeField] private bool shakeCamera = true;

    public void Explode() {

        List<ExplosionTarget> allExplosionTargets = new(serializedExplosionTargets);
        allExplosionTargets.AddRange(AddedExplosionTargets);

        foreach (ExplosionTarget target in allExplosionTargets) {

            Collider2D[] damagedColliders = DamageDealer.DealCircleDamage(
                target.LayerMask,
                transform.position,
                transform.position,
                target.ExplosionRadius,
                target.Damage,
                target.KnockbackStrength
            );

            foreach (Collider2D col in damagedColliders) {
                OnDamage_Target?.Invoke(col.gameObject);
            }
        }

        AddedExplosionTargets.Clear();

        // spawn particles
        if (explosionParticlesPrefab != null) {
            explosionParticlesPrefab.Spawn(transform.position, Containers.Instance.Effects);
        }

        if (shakeCamera) {
            CameraShaker.Instance.ShakeCamera(0.3f);
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Explode);

        OnAttack?.Invoke();
    }
}
