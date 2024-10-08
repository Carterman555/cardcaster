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

    private bool isGeneratingRooms;

    public bool IsGeneratingRooms() {
        return isGeneratingRooms;
    }

    protected override void Awake() {
        base.Awake();
        isGeneratingRooms = true;
    }

    private IEnumerator Start() {
        yield return StartCoroutine(GenerateLayout());

    }

    #region Generate Layout

    [Header("Generate Layout")]
    [SerializeField] private ScriptableDungeonLayout layoutData;
    [SerializeField] private RoomOverlapChecker roomOverlapCheckerPrefab;

    private bool failedRoomCreation;

    private List<RoomOverlapChecker> roomOverlapCheckers = new();
    private Dictionary<RoomType, List<ScriptableRoom>> usedRooms;


    private IEnumerator GenerateLayout() {
        yield return StartCoroutine(TryGenerateLayout());

        while (failedRoomCreation) {
            failedRoomCreation = false;
            yield return StartCoroutine(TryGenerateLayout());
        }
    }

    private IEnumerator TryGenerateLayout() {

        // reset the generation in case this is a retry
        ClearOverlapCheckers();
        SetupUsedRoomsDict();

        // spawn first room checker
        RoomOverlapChecker newRoomChecker = roomOverlapCheckerPrefab.Spawn(Vector2.zero, Containers.Instance.Rooms);
        Room entranceRoomPrefab = GetRandomUniqueRoom(layoutData.LevelLayout.roomType).Prefab;
        newRoomChecker.Setup(entranceRoomPrefab);
        roomOverlapCheckers.Add(newRoomChecker);

        SetChildrensChecker(layoutData.LevelLayout, newRoomChecker, entranceRoomPrefab);

        // spawn all other room checkers
        yield return StartCoroutine(SpawnRoomCheckers(layoutData.LevelLayout));

        isGeneratingRooms = false;
        OnCompleteGeneration?.Invoke();
    }

    private void ClearOverlapCheckers() {
        foreach (RoomOverlapChecker roomOverlapChecker in roomOverlapCheckers) {
            roomOverlapChecker.gameObject.ReturnToPool();
        }
        roomOverlapCheckers.Clear();
    }

    private void SetupUsedRoomsDict() {
        usedRooms = new();
        foreach (RoomType roomType in Enum.GetValues(typeof(RoomType))) {
            usedRooms.Add(roomType, new List<ScriptableRoom>());
        }
    }

    private IEnumerator SpawnRoomCheckers(RoomConnection layout) {

        // Recursively go through each room type in layout
        foreach (RoomConnection subLayout in layout.connectedRooms) {
            bool canSpawn = false;
            ScriptableRoom newRoomScriptable = null;
            Room newRoomPrefab = null;

            int tryRoomCounter = 0;

            // Until it spawns a room that can be spawned
            while (!canSpawn) {

                tryRoomCounter++;
                int tryRoomAmount = 15;
                if (tryRoomCounter > tryRoomAmount) {
                    failedRoomCreation = true;
                    yield break; // exits method
                }

                newRoomScriptable = GetRandomUniqueRoom(subLayout.roomType);
                newRoomPrefab = newRoomScriptable.Prefab;

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

            if (newRoomChecker.CanConnectToDoorwaySide(existingDoorway.GetSide())) {

                PossibleDoorway newRoomDoorway = newRoomChecker.GetRandomConnectingDoorway(existingDoorway.GetSide());
                newRoomChecker.MoveToConnectionPos(existingDoorway, newRoomDoorway);

                // Yield to ensure collider overlap detection works
                yield return null;

                if (!newRoomChecker.OverlapsWithRoomChecker(roomOverlapCheckers)) {
                    roomOverlapCheckers.Add(newRoomChecker);
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

    #endregion


    #region Spawn Rooms

    [Header("Spawn Rooms")]
    [SerializeField] private DoorwayTileDestroyer doorwayTileReplacer;
    [SerializeField] private GameObject cameraConfiner;

    [SerializeField] private Transform horizontalHallwayPrefab;
    [SerializeField] private Transform verticalHallwayPrefab;

    // Solution 1: create a dict of room overlap checkers and there room number. Create a dict of rooms spawned and their room number.
    // then recursively loop through layouts with a counter. This allows you to access the ParentRoomOverlapChecker of the current spawning room
    // through the checker dictionary, you can get access to which room number that is then you can get access to the connecting room through the
    // spawned rooms dictionary

    // Solution 2: the room checker stores the parent checker and...

    /// <summary>
    ///     setup the room
    ///         - spawn hallways (after both rooms have been spawned or all rooms have been spawned)
    /// </summary>
    private void SpawnRooms() {

        int roomNumber = 1;

        /// go through each room checker
        foreach (RoomOverlapChecker roomOverlapChecker in roomOverlapCheckers) {

            roomOverlapChecker.

            /// spawn the room of the checker
            Room newRoom = roomOverlapChecker.GetRoomPrefab().Spawn(roomOverlapChecker.transform.position, Containers.Instance.Rooms);
            SetupRoom(roomNumber, newRoom, )
            roomNumber++;

        }
    }

    private void SetupRoom(int roomNumber, Room newRoom, Room connectingRoom, PossibleDoorway newDoorway, PossibleDoorway connectingDoorway) {
        
        newRoom.SetRoomNum(roomNumber);

        connectingRoom.AddCreatedDoorway(connectingDoorway);

        // spawn in the hallway to connect the rooms
        SpawnHallway(connectingDoorway.GetSide(), connectingDoorway.transform.position);

        //RemoveTilesForHallway(newRoom, )

        newRoom.CopyColliderToCameraConfiner(cameraConfiner);

        // create enter and exit triggers
        newRoom.CreateEnterAndExitTriggers(newDoorway);
        connectingRoom.CreateEnterAndExitTriggers(connectingDoorway);
    }

    #endregion

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