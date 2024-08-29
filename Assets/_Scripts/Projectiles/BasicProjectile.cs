using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BasicProjectile : MonoBehaviour, IStraightProjectile {

    [SerializeField] private float moveSpeed;

    private float damage;
    private float knockbackStrength;

    private Vector2 originalScale;

    public GameObject GetObject() {
        return gameObject;
    }

    private void Awake() {
        originalScale = transform.localScale;
    }

    public void Shoot(Vector2 direction, float damage, float knockbackStrength) {
        transform.up = direction;
        this.damage = damage;
        this.knockbackStrength = knockbackStrength;

        transform.localScale = originalScale;
    }

    private void Update() {
        transform.position += transform.up * moveSpeed * Time.fixedDeltaTime;
    }

    [SerializeField] private LayerMask targetLayer;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (targetLayer.ContainsLayer(collision.gameObject.layer)) {
            if (collision.TryGetComponent(out IDamagable damagable)) {
                damagable.Damage(damage);
            }
            if (collision.TryGetComponent(out Knockback knockback)) {
                Vector2 toTargetDirection = collision.transform.position - transform.position;
                knockback.ApplyKnockback(toTargetDirection, knockbackStrength);
            }

            transform.ShrinkThenDestroy();
        }
    }
}
