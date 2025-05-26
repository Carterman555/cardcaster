using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Knockback : MonoBehaviour {

    private IHasPlayerStats hasPlayerStats;
    private IHasEnemyStats hasEnemyStats;
    private Rigidbody2D rb;
        
    private bool applyingKnockback;

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
                return hasEnemyStats.EnemyStats.KnockbackResistance;
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

        float knockbackFactor = 12f;
        float knockbackForce = strength / GetKnockbackResistance();
        rb.velocity = knockbackForce * knockbackFactor * direction.normalized;

        applyingKnockback = true;
    }

    public void FixedUpdate() {
        if (applyingKnockback) {

            float knockbackDeacceleration = 100f;
            rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, knockbackDeacceleration * Time.fixedDeltaTime);

            if (rb.velocity == Vector2.zero) {
                applyingKnockback = false;
            }
        }
    }
}
