using UnityEngine;

public class BounceProjectile : BasicProjectile {

    [SerializeField] private int maxBounces = 3;
    private int bounces;

    private void OnEnable() {
        bounces = 0;
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {

        bool hitBouncableObstacle = collision.gameObject.layer == GameLayers.WallLayer || collision.gameObject.layer == GameLayers.RoomObjectLayer;
        bool bouncesLeft = bounces < maxBounces;

        if (hitBouncableObstacle && bouncesLeft) {
            Bounce(collision);
        }
        else {
            base.OnTriggerEnter2D(collision);
        }

    }

    private void Bounce(Collider2D collision) {
        // Calculate the reflection vector
        Vector2 normal = collision.ClosestPoint(transform.position) - (Vector2)transform.position;
        normal.Normalize();
        Vector2 reflectDir = Vector2.Reflect(transform.up, normal);

        // Set the new direction
        transform.up = reflectDir;

        bounces++;
    }
}
