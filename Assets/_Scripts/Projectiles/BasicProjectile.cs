using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour, IStraightProjectile {

    [SerializeField] private float moveSpeed;

    private float damage;

    public GameObject GetObject() {
        return gameObject;
    }

    public void Shoot(Vector2 direction, float damage) {
        transform.up = direction;
        this.damage = damage;
    }

    private void Update() {
        transform.position += transform.up * moveSpeed * Time.fixedDeltaTime;
    }

    [SerializeField] private LayerMask targetLayer;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (targetLayer.ContainsLayer(collision.gameObject.layer)) {
            if (collision.TryGetComponent(out Health health)) {
                health.Damage(damage);
            }

            transform.ShrinkThenDestroy();
        }
    }
}
