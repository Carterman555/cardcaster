using UnityEngine;
using System.Collections.Generic;

public enum RoomType {
    Normal,
    Hub,
    Reward,
    Boss,
    Shop,
    Entrance,
    Exit
}

[CreateAssetMenu(fileName = "New Room Data", menuName = "Room Generation/Room Data")]
public class ScriptableRoom : ScriptableObject {

    [SerializeField] private RoomType roomType;
    public RoomType RoomType => roomType;

    [SerializeField] private GameObject roomPrefab;
}