using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Localization.SmartFormat.Utilities;

public class Knockback : MonoBehaviour {

    public event Action<Vector2> OnKnockbacked;

    private IHasPlayerStats hasPlayerStats;
    private IHasEnemyStats hasEnemyStats;

    // requires either
    private Rigidbody2D rb;
    private NavMeshAgent agent;

    private Vector2 knockbackVelocity;

    [SerializeField] private bool overrideKnockback;
    [ConditionalHide("overrideKnockback")][SerializeField] private float overrideKnockbackResistance;

    private float GetKnockbackResistance() {
        if (overrideKnockback) {
            return overrideKnockbackResistance;
        }
        else {
            if (hasPlayerStats != null) {
                return hasPlayerStats.PlayerStats.KnockbackResistance;
            }
            else if (hasEnemyStats != null) {
                return hasEnemyStats.GetEnemyStats().KnockbackResistance;
            }
            else {
                Debug.LogError("Object with Knockback does not override or have stats component!");
                return 1f;
            }
        }
    }

    public bool IsApplyingKnockback() {
        return knockbackVelocity.magnitude > 0f;
    }

    private void Awake() {
        hasPlayerStats = GetComponent<IHasPlayerStats>();
        hasEnemyStats = GetComponent<IHasEnemyStats>();

        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void ApplyKnockback(Vector2 direction, float strength) {

        if (TryGetComponent(out EnemyHealth enemyHealth) && enemyHealth.IsInvincible()) {
            return;
        }

        if (TryGetComponent(out PlayerHealth playerHealth) && playerHealth.IsInvincible()) {
            return;
        }

        if (GetKnockbackResistance() == 0) {
            Debug.LogError(gameObject.name + ": KnockbackResistance Cannot be 0!");
            return;
        }


        float knockbackFactor = 10f;
        float knockbackForce = strength / GetKnockbackResistance();
        knockbackVelocity = knockbackForce * knockbackFactor * direction.normalized;

        if (agent != null) {
            agent.velocity = knockbackVelocity;
        }
        else if (rb != null) {
            rb.velocity = knockbackVelocity;
        }
        else {
            Debug.LogError($"Trying to apply knockback to {name}, but doesn't have agent or rb!");
            return;
        }

        OnKnockbacked?.Invoke(direction.normalized);
    }

    public void FixedUpdate() {
        if (IsApplyingKnockback()) {

            float knockbackDeacceleration = 100f;

            knockbackVelocity = Vector2.MoveTowards(knockbackVelocity, Vector2.zero, knockbackDeacceleration * Time.fixedDeltaTime);
            if (agent != null) {
                agent.velocity = knockbackVelocity;
            }
            else if (rb != null) {
                rb.velocity = knockbackVelocity;
            }
            else {
                Debug.LogError($"Trying to apply knockback to {name}, but doesn't have agent or rb!");
                return;
            }
        }
    }
}
