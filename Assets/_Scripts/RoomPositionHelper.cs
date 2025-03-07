using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPositionHelper {
    private Vector2 avoidCenter = Vector2.zero;
    private float avoidRadius = 0f;
    private float obstacleAvoidDistance = 0f;
    private float wallAvoidDistance = 0f;
    private bool mustBeOnGroundTile = true;

    public Vector2 GetRandomRoomPos(float obstacleAvoidanceRadius = 1f, float wallAvoidDistance = 1f) {
        Vector2 randomPoint = new RoomPositionHelper()
            .SetObstacleAvoidance(obstacleAvoidanceRadius)
            .SetWallAvoidance(wallAvoidDistance)
            .GetRandomPositionInCollider();
        return randomPoint;
    }

    public Vector2 GetRandomRoomPos(Vector2 avoidCenter, float avoidRadius, float obstacleAvoidDistance = 1f, float wallAvoidDistance = 1f) {
        Vector2 randomPoint = new RoomPositionHelper()
            .SetAvoidArea(avoidCenter, avoidRadius)
            .SetObstacleAvoidance(obstacleAvoidDistance)
            .SetWallAvoidance(wallAvoidDistance)
            .GetRandomPositionInCollider();
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

    public RoomPositionHelper SetWallAvoidance(float distance) {
        this.wallAvoidDistance = distance;
        return this;
    }

    public RoomPositionHelper MustBeOnGroundTile(bool value) {
        this.mustBeOnGroundTile = value;
        return this;
    }

    public Vector2 GetRandomPositionInCollider() {

        int breakoutCounter = 0;

        PolygonCollider2D col = Room.GetCurrentRoom().GetComponent<PolygonCollider2D>();
        Bounds bounds = col.bounds;
        Vector2 randomPoint;

        do {
            randomPoint = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            breakoutCounter++;
            if (breakoutCounter > 1000) {
                Debug.LogError("Could not find room position! Returned invalid Pos!");
                return randomPoint;
            }

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

        if (wallAvoidDistance > 0 && IsNearWall(point, wallAvoidDistance))
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
        bool onColliderTile = OnTile(room.GetTopWallsTilemap(), point) || OnTile(room.GetBotWallsTilemap(), point);
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

    private bool IsNearWall(Vector2 point, float wallAvoidanceRadius = 1f) {
        return Physics2D.OverlapCircle(point, wallAvoidanceRadius, GameLayers.WallLayerMask);
    }

    #endregion
}