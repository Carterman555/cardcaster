using UnityEngine;

public class RandomTeleportBehavior : MonoBehaviour {

    public void Teleport() {
        Vector2 newPosition = new RoomPositionHelper().GetRandomSpawnPos();
        transform.position = newPosition;
    }

    public void Teleport(Vector2 noTeleportCenter, float noTeleportRadius) {
        Vector2 newPosition = new RoomPositionHelper().GetRandomSpawnPos(noTeleportCenter, noTeleportRadius);
        transform.position = newPosition;
    }
}
