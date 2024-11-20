using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Data", order = 1)]
public class ScriptableRoomColliders : ScriptableObject {
    public PolygonCollider2D[] Colliders;
}
