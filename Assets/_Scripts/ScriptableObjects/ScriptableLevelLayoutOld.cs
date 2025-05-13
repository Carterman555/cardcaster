using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class RoomConnectionOld {
    public RoomType roomType;
    public List<RoomConnectionOld> connectedRooms = new();
    
    //... this is the room overlap checker of the connecting room it was created from and it's used in the
    //... generation process
    [HideInInspector] public RoomOverlapChecker ParentRoomOverlapChecker;
}

[CreateAssetMenu(fileName = "New Dungeon Layout", menuName = "Dungeon/Dungeon Layout")]
public class ScriptableLevelLayoutOld : ScriptableObject {

    [FormerlySerializedAs("entranceRoom")]
    [SerializeField] private RoomConnectionOld roomLayout;
    public RoomConnectionOld LevelLayout => roomLayout;
}