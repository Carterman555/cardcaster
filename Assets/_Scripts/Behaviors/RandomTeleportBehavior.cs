using UnityEngine;

public class RandomTeleportBehavior : UnitBehavior {

    private PolygonCollider2D teleportBounds;

    public RandomTeleportBehavior(GameObject gameObject, IHasStats hasStats, PolygonCollider2D teleportBounds) : base(gameObject, hasStats) {
        this.teleportBounds = teleportBounds;
    }

    public void Teleport() {
        Vector2 newPosition = new RoomPositionHelper().GetRandomSpawnPos();
        gameObject.transform.position = newPosition;
    }

    public void Teleport(Vector2 noTeleportCenter, float noTeleportRadius) {
        Vector2 newPosition = new RoomPositionHelper().GetRandomSpawnPos(noTeleportCenter, noTeleportRadius);
        gameObject.transform.position = newPosition;
    }
}
