using UnityEngine;

public class HeatSeekProjectile : MonoBehaviour, ITargetProjectile {


    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    private Transform target;
    private float damage;


    public GameObject GetObject() {
        return gameObject;
    }

    public void Shoot(Transform target, float damage) {
        this.target = target;
        this.damage = damage;

        Vector2 toTarget = target.position - transform.position;
        transform.up = toTarget;
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
        if (targetLayer.ContainsLayer(collision.gameObject.layer)) {
            if (collision.TryGetComponent(out Health health)) {
                health.Damage(damage);
            }

            transform.ShrinkThenDestroy();
        }
    }

}
