using UnityEngine;

[RequireComponent(typeof(IHasStats), typeof(Rigidbody2D))]
public class Knockback : MonoBehaviour {

    private IHasStats hasStats;
    private Rigidbody2D rb;
    private Health health;
        
    private bool applyingKnockback;

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

        float knockbackForce = strength / hasStats.GetStats().KnockbackResistance;
        rb.velocity = knockbackForce * knockbackFactor * direction.normalized;

        applyingKnockback = true;
    }

    [SerializeField] private float knockbackFactor = 6f;
    [SerializeField] private float knockbackDeacceleration = 40f;

    public void FixedUpdate() {
        if (applyingKnockback) {

            rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, knockbackDeacceleration * Time.fixedDeltaTime);

            if (rb.velocity == Vector2.zero) {
                applyingKnockback = false;
            }
        }
    }
}
