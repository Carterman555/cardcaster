using UnityEngine;

public class RandomTeleportBehavior : UnitBehavior {

    private PolygonCollider2D teleportBounds;

    public RandomTeleportBehavior(GameObject gameObject, IHasStats hasStats, PolygonCollider2D teleportBounds) : base(gameObject, hasStats) {
        this.teleportBounds = teleportBounds;
    }

    public void Teleport() {
        Vector2 newPosition = GetRandomPositionInBounds(Vector2.zero, 0f);
        gameObject.transform.position = newPosition;
    }

    public void Teleport(Vector2 noTeleportCenter, float noTeleportRadius) {
        Vector2 newPosition = GetRandomPositionInBounds(noTeleportCenter, noTeleportRadius);
        gameObject.transform.position = newPosition;
    }

    private Vector2 GetRandomPositionInBounds(Vector2 noTeleportCenter, float noTeleportRadius) {
        Bounds bounds = teleportBounds.bounds;

        // Keep trying until we find a valid point inside the polygon and outside the no teleport circle
        Vector2 randomPoint;
        do {
            // Generate a random point within the bounds' rectangle
            randomPoint = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
        } while (!IsPointInPolygon(randomPoint) || IsPointInNoTeleportZone(randomPoint, noTeleportCenter, noTeleportRadius));

        return randomPoint;
    }

    private bool IsPointInPolygon(Vector2 point) {
        return teleportBounds.OverlapPoint(point);
    }

    private bool IsPointInNoTeleportZone(Vector2 point, Vector2 center, float radius) {
        return Vector2.Distance(point, center) < radius;
    }

}
