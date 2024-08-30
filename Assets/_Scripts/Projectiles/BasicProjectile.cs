using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BasicProjectile : MonoBehaviour, IStraightProjectile {

    [SerializeField] private float moveSpeed;

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

    public void Shoot(Vector2 direction, float damage, float knockbackStrength) {
        transform.up = direction;
        this.damage = damage;
        this.knockbackStrength = knockbackStrength;

        transform.localScale = originalScale;

        hit = false;
    }

    private void FixedUpdate() {
        if (hit) {
            return;
        }
        transform.position += transform.up * moveSpeed * Time.fixedDeltaTime;
    }

    [SerializeField] private LayerMask targetLayer;

    protected virtual void OnTriggerEnter2D(Collider2D collision) {

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
