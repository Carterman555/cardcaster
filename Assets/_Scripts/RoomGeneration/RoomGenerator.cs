using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class RoomGenerator : StaticInstance<RoomGenerator> {

    public static event Action OnCompleteGeneration;

    [SerializeField] private ScriptableDungeonLayout layoutData;

    [SerializeField] private DoorwayTileDestroyer doorwayTileReplacer;
    [SerializeField] private GameObject cameraConfiner;

    [Header("Prefabs")]
    [SerializeField] private Transform horizontalHallwayPrefab;
    [SerializeField] private Transform verticalHallwayPrefab;

    [SerializeField] private RoomOverlapChecker roomOverlapCheckerPrefab;

    private bool isGeneratingRooms;

    private List<RoomOverlapChecker> roomOverlapCheckers = new();

    private Dictionary<RoomType, List<ScriptableRoom>> usedRooms;

    public bool IsGeneratingRooms() {
        return isGeneratingRooms;
    }

    protected override void Awake() {
        base.Awake();
        isGeneratingRooms = true;

        SetupUsedRoomsDict();
    }

    private void Start() {
        StartCoroutine(GenerateRooms());
    }

    private void SetupUsedRoomsDict() {
        usedRooms = new();
        foreach (RoomType roomType in Enum.GetValues(typeof(RoomType))) {
            usedRooms.Add(roomType, new List<ScriptableRoom>());
        }
    }
    
    private IEnumerator GenerateRooms() {

        // spawn first room checker
        RoomOverlapChecker newRoomChecker = roomOverlapCheckerPrefab.Spawn(Containers.Instance.Rooms);
        Room entranceRoomPrefab = GetRandomUniqueRoom(layoutData.LevelLayout.roomType).Prefab;
        newRoomChecker.Setup(entranceRoomPrefab);
        SetChildrensChecker(layoutData.LevelLayout, newRoomChecker, entranceRoomPrefab);

        // spawn all other room checkers
        yield return StartCoroutine(SpawnRoomCheckers(layoutData.LevelLayout));

        isGeneratingRooms = false;
        OnCompleteGeneration?.Invoke();
    }

    private bool failedRoomCreation;

    private IEnumerator SpawnRoomCheckers(RoomConnection layout) {

        // Recursively go through each room type in layout
        foreach (RoomConnection subLayout in layout.connectedRooms) {
            bool canSpawn = false;
            ScriptableRoom newRoomScriptable = null;
            Room newRoomPrefab = null;

            int breakOutCounter = 0;

            print("\n");
            print("Attempting to make connection to " + subLayout.ParentRoomOverlapChecker.GetRoomPrefab().name);

            // Until it spawns a room that can be spawned
            while (!canSpawn) {

                breakOutCounter++;
                if (breakOutCounter > 30) {
                    Debug.LogError("Breakout Error");
                    failedRoomCreation = true;
                    yield break; // exits method
                }

                newRoomScriptable = GetRandomUniqueRoom(subLayout.roomType);
                newRoomPrefab = newRoomScriptable.Prefab;

                print("Trying to connect new room: " + newRoomPrefab.name);

                // Try to spawn this room
                yield return StartCoroutine(TrySpawnRoomChecker(newRoomPrefab, subLayout.ParentRoomOverlapChecker, (success) => canSpawn = success));
            }

            usedRooms[newRoomScriptable.RoomType].Add(newRoomScriptable);

            RoomOverlapChecker newRoomChecker = roomOverlapCheckers.Last();
            SetChildrensChecker(subLayout, newRoomChecker, newRoomPrefab);

            if (!failedRoomCreation) {
                yield return StartCoroutine(SpawnRoomCheckers(subLayout)); // Recursive spawn
            }
        }
    }

    // Choose random unique room with matching type
    private ScriptableRoom GetRandomUniqueRoom(RoomType roomType) {
        ScriptableRoom newRoomScriptable;
        List<ScriptableRoom> availableRooms = ResourceSystem.Instance.GetRooms(roomType)
            .Where(room => !usedRooms[roomType].Contains(room)).ToList();

        // doesn't need to be unique if reward room
        if (roomType == RoomType.Reward) {
            availableRooms = ResourceSystem.Instance.GetRooms(roomType).ToList();
        }

        if (availableRooms.Count == 0) {
            Debug.LogError("There are no rooms of type " + roomType + " available!");
        }

        newRoomScriptable = availableRooms.RandomItem();
        return newRoomScriptable;
    }

    private void SetChildrensChecker(RoomConnection layout, RoomOverlapChecker newRoomChecker, Room roomPrefab) {
        foreach (RoomConnection subLayout in layout.connectedRooms) {
            subLayout.ParentRoomOverlapChecker = newRoomChecker;
        }
    }

    /// <summary>
    /// go through all the doorways of the connecting room to see if the new room can connect to any of them. If it can,
    /// then return the position where the connection can be made
    /// </summary>
    private IEnumerator TrySpawnRoomChecker(Room newRoomPrefab, RoomOverlapChecker existingRoomChecker, Action<bool> callback) {

        RoomOverlapChecker newRoomChecker = roomOverlapCheckerPrefab.Spawn(Containers.Instance.Rooms);
        newRoomChecker.Setup(newRoomPrefab);

        foreach (PossibleDoorway existingDoorway in existingRoomChecker.GetPossibleDoorways()) {

            print(" - Can make connection from " + existingDoorway.GetSide().ToString() + " side of existing room: " + newRoomChecker.CanConnectToDoorwaySide(existingDoorway.GetSide()));

            if (newRoomChecker.CanConnectToDoorwaySide(existingDoorway.GetSide())) {

                PossibleDoorway newRoomDoorway = newRoomChecker.GetRandomConnectingDoorway(existingDoorway.GetSide());
                newRoomChecker.MoveToConnectionPos(existingDoorway, newRoomDoorway);

                // Yield to ensure collider overlap detection works
                yield return null;

                print("  - Overlaps with another room: " + newRoomChecker.OverlapsWithRoomChecker(roomOverlapCheckers));
                
                if (!newRoomChecker.OverlapsWithRoomChecker(roomOverlapCheckers)) {
                    roomOverlapCheckers.Add(newRoomChecker);
                    print("   - Successfully made connection: " + newRoomChecker.GetRoomPrefab().name);
                    newRoomChecker.RemovePossibleDoorway(newRoomDoorway);
                    existingRoomChecker.RemovePossibleDoorway(existingDoorway);
                    callback(true);
                    yield break;
                }
            }
        }

        newRoomChecker.gameObject.ReturnToPool();
        callback(false);
    }

    private void SetupRoom(Room newRoom) {
        throw new NotImplementedException();
    }

    private void RemoveTilesForHallway(Room newRoom, Room connectingRoom, PossibleDoorway connectingDoorway, PossibleDoorway newDoorway) {
        Tilemap connectingGroundTilemap = connectingDoorway.GetSide() == DoorwaySide.Bottom ?
                            connectingRoom.GetBotColliderTilemap() : connectingRoom.GetGroundTilemap();

        doorwayTileReplacer.DestroyTiles(connectingGroundTilemap,
            connectingRoom.GetColliderTilemap(),
            connectingDoorway.GetSide(),
            connectingDoorway.transform.localPosition);

        Tilemap newGroundTilemap = newDoorway.GetSide() == DoorwaySide.Bottom ?
            newRoom.GetBotColliderTilemap() : newRoom.GetGroundTilemap();

        doorwayTileReplacer.DestroyTiles(newRoom.GetGroundTilemap(),
            newRoom.GetColliderTilemap(),
            newDoorway.GetSide(),
            newDoorway.transform.localPosition);
    }

    private void SpawnHallway(DoorwaySide doorwaySide, Vector2 doorwayPosition) {

        Transform hallwayPrefab = null;
        if (doorwaySide == DoorwaySide.Top) {
            hallwayPrefab = verticalHallwayPrefab;
        }
        else if (doorwaySide == DoorwaySide.Bottom) {
            hallwayPrefab = verticalHallwayPrefab;
        }
        else if (doorwaySide == DoorwaySide.Left) {
            hallwayPrefab = horizontalHallwayPrefab;
        }
        else if (doorwaySide == DoorwaySide.Right) {
            hallwayPrefab = horizontalHallwayPrefab;
        }

        float hallwayOffset = 4;
        Vector2 hallwayPos = doorwayPosition + SideToDirection(doorwaySide) * hallwayOffset;
        Instantiate(hallwayPrefab, hallwayPos, Quaternion.identity, Containers.Instance.Rooms);
    }

    public Vector2 SideToDirection(DoorwaySide side) {
        if (side == DoorwaySide.Top) {
            return new Vector2(0, 1);
        }
        else if (side == DoorwaySide.Bottom) {
            return new Vector2(0, -1);
        }
        else if (side == DoorwaySide.Left) {
            return new Vector2(-1, 0);
        }
        else if (side == DoorwaySide.Right) {
            return new Vector2(1, 0);
        }
        else {
            return Vector2.zero;
        }
    }

}