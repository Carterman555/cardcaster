using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPositionHelper {
    private Vector2 avoidCenter = Vector2.zero;
    private float avoidRadius = 0f;
    private float obstacleAvoidDistance = 0f;
    private bool mustBeOnGroundTile = true;

    public Vector2 GetRandomSpawnPos() {
        Vector2 randomPoint = new RoomPositionHelper()
            .SetObstacleAvoidance(0.5f)
            .GetRandomPosition();
        return randomPoint;
    }

    public Vector2 GetRandomSpawnPos(Vector2 avoidCenter, float avoidRadius) {
        Vector2 randomPoint = new RoomPositionHelper()
            .SetAvoidArea(avoidCenter, avoidRadius)
            .SetObstacleAvoidance(0.5f)
            .GetRandomPosition();
        return randomPoint;
    }

    #region Chain Methods

    public RoomPositionHelper SetAvoidArea(Vector2 center, float radius) {
        this.avoidCenter = center;
        this.avoidRadius = radius;
        return this;
    }

    public RoomPositionHelper SetObstacleAvoidance(float distance) {
        this.obstacleAvoidDistance = distance;
        return this;
    }

    public RoomPositionHelper MustBeOnGroundTile(bool value) {
        this.mustBeOnGroundTile = value;
        return this;
    }

    public Vector2 GetRandomPosition() {
        PolygonCollider2D col = Room.GetCurrentRoom().GetComponent<PolygonCollider2D>();
        Bounds bounds = col.bounds;
        Vector2 randomPoint;
        do {
            randomPoint = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
        } while (!IsValidPosition(randomPoint));

        return randomPoint;
    }

    public bool IsValidPosition(Vector2 point) {
        if (!IsPointInPolygon(Room.GetCurrentRoom().GetComponent<PolygonCollider2D>(), point))
            return false;

        if (mustBeOnGroundTile && !OnlyOnGroundTile(Room.GetCurrentRoom(), point))
            return false;

        if (IsPointInNoTeleportZone(point, avoidCenter, avoidRadius))
            return false;

        if (obstacleAvoidDistance > 0 && IsNearObstacle(point, obstacleAvoidDistance))
            return false;

        return true;
    }

    #endregion


    #region Base Methods

    private bool IsPointInPolygon(PolygonCollider2D col, Vector2 point) {
        return col.OverlapPoint(point);
    }

    private bool OnlyOnGroundTile(Room room, Vector2 point) {
        bool onGroundTile = OnTile(room.GetGroundTilemap(), point);
        bool onColliderTile = OnTile(room.GetColliderTilemap(), point) || OnTile(room.GetBotColliderTilemap(), point);
        return onGroundTile && !onColliderTile;
    }

    private bool OnTile(Tilemap tilemap, Vector2 point) {
        TileBase tile = tilemap.GetTile(tilemap.WorldToCell(point));
        return tile != null;
    }

    private bool IsPointInNoTeleportZone(Vector2 point, Vector2 center, float radius) {
        return Vector2.Distance(point, center) <= radius;
    }

    private bool IsNearObstacle(Vector2 point, float obstacleAvoidanceRadius = 1f) {
        return Physics2D.OverlapCircle(point, obstacleAvoidanceRadius, GameLayers.ObstacleLayerMask);
    }

    #endregion
}