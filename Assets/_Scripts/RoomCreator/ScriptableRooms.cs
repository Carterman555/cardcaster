using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableRooms", menuName = "RoomCreator/Rooms")]
public class ScriptableRooms : ScriptableObject {
    public GameObject[] Rooms;
}
