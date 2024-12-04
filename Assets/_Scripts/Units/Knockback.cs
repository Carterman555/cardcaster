using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Knockback : MonoBehaviour {

    private IHasStats hasStats;
    private Rigidbody2D rb;
    private Health health;
        
    private bool applyingKnockback;

    [SerializeField] private bool overrideKnockback;
    [ConditionalHide("overrideKnockback")][SerializeField] private float overrideKnockbackResistance;

    public bool IsApplyingKnockback() {
        return applyingKnockback;
    }

    private void Awake() {
        hasStats = GetComponent<IHasStats>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
    }

    public void ApplyKnockback(Vector2 direction, float strength) {

        if (health.IsInvincible()) {
            return;
        }

        if (hasStats.GetStats().KnockbackResistance == 0) {
            Debug.LogError(gameObject.name + ": KnockbackResistance Cannot be 0!");
        }

        float knockbackResistance;
        if (overrideKnockback) {
            knockbackResistance = overrideKnockbackResistance;
        }
        else {
            knockbackResistance = hasStats.GetStats().KnockbackResistance;
        }

        float knockbackFactor = 12f;
        float knockbackForce = strength / knockbackResistance;
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
