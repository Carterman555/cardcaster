using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public enum RoomType {
    Normal,
    Hub,
    Reward,
    Boss,
    Shop,
    Entrance,
    Exit
}

public enum EnvironmentType {
    Stone = 0,
    SmoothStone = 1,
    BlueStone = 2
}

[CreateAssetMenu(fileName = "New Room Data", menuName = "Room Generation/Room Data")]
public class ScriptableRoom : ScriptableObject {

    [SerializeField] private RoomType roomType;
    public RoomType RoomType => roomType;

    [SerializeField] private EnvironmentType environmentType;
    public EnvironmentType EnvironmentType => environmentType;

    [SerializeField] private Room roomPrefab;
    public Room Prefab => roomPrefab;

    [SerializeField] private bool noEnemies;
    public bool NoEnemies => noEnemies;

    [ConditionalHideReversed("noEnemies")]
    [SerializeField] private ScriptableEnemyComposition scriptableEnemyComposition;
    public ScriptableEnemyComposition ScriptableEnemyComposition => scriptableEnemyComposition;
}