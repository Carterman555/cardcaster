using System;
using System.Collections;
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

    private Vector2 startKnockbackVelocity;
    private Vector2 knockbackVelocity;

    [SerializeField] private bool overrideKnockback;
    [ConditionalHide("overrideKnockback")][SerializeField] private float overrideKnockbackResistance;

    private bool applyingKnockback;

    private Vector2 startKnockbackPos;

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
        return applyingKnockback;
    }

    private void Awake() {
        hasPlayerStats = GetComponent<IHasPlayerStats>();
        hasEnemyStats = GetComponent<IHasEnemyStats>();

        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable() {
        applyingKnockback = false;
        knockbackVelocity = Vector2.zero;
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

        applyingKnockback = true;

        float knockbackFactor = 10f;
        float knockbackForce = strength / GetKnockbackResistance();
        knockbackVelocity = knockbackForce * knockbackFactor * direction.normalized;
        startKnockbackVelocity = knockbackVelocity;

        SetVelocity(knockbackVelocity);

        startKnockbackPos = transform.position;

        OnKnockbacked?.Invoke(direction.normalized);

        StartCoroutine(ApplyKnockbackCor());
    }

    private IEnumerator ApplyKnockbackCor() {

        float knockbackTime = 0.15f;
        float knockbackTimer = 0;

        while (knockbackTimer < knockbackTime) {
            yield return null;

            knockbackTimer += Time.deltaTime;

            float knockbackDecelerationRate = 1f / knockbackTime;
            knockbackVelocity -= startKnockbackVelocity * knockbackDecelerationRate * Time.deltaTime;
            SetVelocity(knockbackVelocity);
        }

        SetVelocity(Vector2.zero);

        applyingKnockback = false;
    }

    private void SetVelocity(Vector2 velocity) {
        if (agent != null) {
            agent.velocity = velocity;
        }
        else if (rb != null) {
            rb.velocity = velocity;
        }
        else {
            Debug.LogError($"Trying to apply knockback to {name}, but doesn't have agent or rb!");
        }
    }
}
