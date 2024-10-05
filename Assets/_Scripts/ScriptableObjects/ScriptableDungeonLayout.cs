using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class RoomConnection {
    public RoomType roomType;
    public List<RoomConnection> connectedRooms = new List<RoomConnection>();
}

[CreateAssetMenu(fileName = "New Dungeon Layout", menuName = "Dungeon/Dungeon Layout")]
public class ScriptableDungeonLayout : ScriptableObject {

    [FormerlySerializedAs("entranceRoom")]
    [SerializeField] private RoomConnection roomLayout;
    public RoomConnection LevelLayout => roomLayout;
}