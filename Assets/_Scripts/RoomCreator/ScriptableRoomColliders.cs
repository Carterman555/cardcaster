using UnityEngine;

[CreateAssetMenu(fileName = "RoomColliders", menuName = "RoomCreator/RoomColliders", order = 1)]
public class ScriptableRoomColliders : ScriptableObject {
    public PolygonCollider2D[] Colliders;
}
