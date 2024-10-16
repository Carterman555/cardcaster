using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeBehavior : MonoBehaviour, IAttacker {

    public event Action OnAttack;

    [SerializeField] private float explosionRadius;

    [Header("Visual")]
    [SerializeField] private ParticleSystem explosionParticlesPrefab;

    private IHasStats hasStats;

    private void Awake() {
        hasStats = GetComponent<IHasStats>();
    }

    public void Explode(bool returnToPool = true) {

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, explosionRadius, GameLayers.PlayerLayer);

        foreach (Collider2D col in cols) {
            DamageDealer.TryDealDamage(col.gameObject,
                transform.position,
                hasStats.GetStats().Damage,
                hasStats.GetStats().KnockbackStrength);
        }

        if (explosionParticlesPrefab != null) {
            explosionParticlesPrefab.Spawn(transform.position, Containers.Instance.Effects);
        }

        OnAttack?.Invoke();

        if (returnToPool) {
            gameObject.ReturnToPool();
        }
    }
}
