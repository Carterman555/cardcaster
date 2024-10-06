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

    [SerializeField] private Room[] roomPrefabs;
    [SerializeField] private RoomOverlapChecker roomOverlapCheckerPrefab;

    private bool isGeneratingRooms;

    private List<RoomOverlapChecker> roomOverlapCheckers = new();

    public bool IsGeneratingRooms() {
        return isGeneratingRooms;
    }

    protected override void Awake() {
        base.Awake();
        isGeneratingRooms = true;
    }

    private void Start() {
        StartCoroutine(GenerateRooms());
    }

    /// <summary>
    /// Recursively go through each room type in layout
	///    While the room is can't be spawned
	///	      Choose random unique room with matching type(doesn't need to be unique if chest room)
    ///       Try to spawn this room
    /// </summary>
    private IEnumerator GenerateRooms() {

        // setup first room checker
        RoomOverlapChecker newRoomChecker = roomOverlapCheckerPrefab.Spawn(Containers.Instance.Rooms);
        newRoomChecker.Setup(roomPrefabs.RandomItem());
        layoutData.LevelLayout.RoomOverlapChecker = newRoomChecker;

        yield return StartCoroutine(SetRoomToSpawn(layoutData.LevelLayout));

        foreach (RoomOverlapChecker roomOverlapChecker in roomOverlapCheckers) {
            print("Room: " + roomOverlapChecker.GetRoomPrefab().name + ", pos: " + roomOverlapChecker.transform.position);
        }

        isGeneratingRooms = false;
        OnCompleteGeneration?.Invoke();
    }

    private IEnumerator SetRoomToSpawn(RoomConnection layout) {

        foreach (RoomConnection subLayout in layout.connectedRooms) {
            print("Room: " + subLayout.roomType);

            bool canSpawn = false;
            Room newRoomPrefab = null;

            int breakOutCounter = 0;

            while (!canSpawn) {

                breakOutCounter++;
                if (breakOutCounter > 30) {
                    Debug.LogError("Breakout Error");
                    break;
                }

                newRoomPrefab = roomPrefabs.RandomItem();
                yield return StartCoroutine(TrySpawnRoomChecker(newRoomPrefab, layout.RoomOverlapChecker, (success) => canSpawn = success));

                print("Tried room: " + newRoomPrefab.name + " spawned: " + canSpawn);

            }

            RoomOverlapChecker newlySpawnedOverlapChecker = roomOverlapCheckers.Last();

            subLayout.RoomOverlapChecker = newlySpawnedOverlapChecker;

            yield return StartCoroutine(SetRoomToSpawn(subLayout)); // Recursive spawn
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
            if (newRoomChecker.CanConnectToDoorwaySide(existingDoorway.GetSide())) {

                PossibleDoorway newRoomDoorway = newRoomChecker.GetRandomConnectingDoorway(existingDoorway.GetSide());
                newRoomChecker.MoveToConnectionPos(existingDoorway, newRoomDoorway);

                // Yield to ensure collider overlap detection works
                yield return null;

                if (!newRoomChecker.OverlapsWithRoomChecker(roomOverlapCheckers)) {
                    roomOverlapCheckers.Add(newRoomChecker);
                    print("Added: " + newRoomChecker.GetRoomPrefab().name);
                    newRoomChecker.RemovePossibleDoorway(newRoomDoorway);
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