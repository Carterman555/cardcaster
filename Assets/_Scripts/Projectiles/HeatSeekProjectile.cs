using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class HeatSeekProjectile : MonoBehaviour, ITargetProjectile {

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    private Transform target;
    private float damage;
    private float knockbackStrength;

    private Vector2 originalScale;

    private bool hit;

    public GameObject GetObject() {
        return gameObject;
    }

    private void Awake() {
        originalScale = transform.localScale;
    }

    public void Shoot(Transform target, float damage, float knockbackStrength) {
        this.target = target;
        this.damage = damage;
        this.knockbackStrength = knockbackStrength;

        transform.localScale = originalScale;

        Vector2 toTarget = target.position - transform.position;
        transform.up = toTarget;

        hit = false;
    }

    private void FixedUpdate() {

        // move
        transform.position += transform.up * moveSpeed * Time.fixedDeltaTime;

        // rotate
        Vector2 toTarget = target.position - transform.position;
        transform.up = Vector3.MoveTowards(transform.up, toTarget, rotationSpeed * Time.fixedDeltaTime);
    }

    [SerializeField] private LayerMask targetLayer;

    private void OnTriggerEnter2D(Collider2D collision) {

        if (hit) {
            return;
        }

        if (targetLayer.ContainsLayer(collision.gameObject.layer)) {
            if (collision.TryGetComponent(out IDamagable damagable)) {
                damagable.Damage(damage);
            }
            if (collision.TryGetComponent(out Knockback knockback)) {
                Vector2 toTargetDirection = collision.transform.position - transform.position;
                knockback.ApplyKnockback(toTargetDirection, knockbackStrength);
            }

            transform.ShrinkThenDestroy();
            hit = true;
        }

        if (collision.gameObject.layer == GameLayers.WallLayer || collision.gameObject.layer == GameLayers.RoomObjectLayer) {
            transform.ShrinkThenDestroy();
            hit = true;
        }
    }
}
