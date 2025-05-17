using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomConnection {
    public string RoomID; // Unique identifier
    public RoomType RoomType;
    public List<string> ConnectedRoomIDs = new(); // Store IDs instead of direct references

    [HideInInspector] public RoomOverlapChecker ParentRoomOverlapChecker;
}

[CreateAssetMenu(fileName = "LevelLayout", menuName = "Level Layout")]
public class ScriptableLevelLayout : ScriptableObject {
    [SerializeField] private RoomConnection[] roomConnections;
    public RoomConnection[] RoomConnections => roomConnections;
}
