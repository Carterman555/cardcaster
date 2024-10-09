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

    private List<RoomOverlapChecker> roomOverlapCheckers = new();

    public bool IsGeneratingRooms() {
        return isGeneratingRooms;
    }

    protected override void Awake() {
        base.Awake();
        isGeneratingRooms = true;
    }

    private IEnumerator Start() {
        yield return StartCoroutine(GenerateLayout());

        SpawnRooms();

        yield return null;

        isGeneratingRooms = false;
        OnCompleteGeneration?.Invoke();
    }

    #region Generate Layout

    [Header("Generate Layout")]
    [SerializeField] private ScriptableDungeonLayout layoutData;
    [SerializeField] private RoomOverlapChecker roomOverlapCheckerPrefab;

    private bool failedRoomCreation;

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
        newRoomChecker.Setup(entranceRoomPrefab, null);
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
        newRoomChecker.Setup(newRoomPrefab, existingRoomChecker);

        foreach (PossibleDoorway existingDoorway in existingRoomChecker.GetPossibleDoorways()) {

            if (newRoomChecker.CanConnectToDoorwaySide(existingDoorway.GetSide())) {

                PossibleDoorway newRoomDoorway = newRoomChecker.GetRandomConnectingDoorway(existingDoorway.GetSide());
                newRoomChecker.MoveToConnectionPos(existingDoorway, newRoomDoorway);

                // Yield to ensure collider overlap detection works
                yield return null;

                if (!newRoomChecker.OverlapsWithRoomChecker(roomOverlapCheckers)) {
                    roomOverlapCheckers.Add(newRoomChecker);

                    newRoomChecker.AddCreatedDoorway(newRoomDoorway, existingRoomChecker);
                    existingRoomChecker.AddCreatedDoorway(existingDoorway, newRoomChecker);

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

    private Dictionary<RoomOverlapChecker, Room> spawnRoomsDict = new();

    /// <summary>
    /// TODO - write summary
    /// </summary>
    private void SpawnRooms() {

        spawnRoomsDict.Clear();

        // setup first room seperately because the setup is different
        RoomOverlapChecker firstOverlapChecker = roomOverlapCheckers[0];
        Room firstRoom = firstOverlapChecker.GetRoomPrefab().Spawn(firstOverlapChecker.transform.position, Containers.Instance.Rooms);
        spawnRoomsDict.Add(firstOverlapChecker, firstRoom);

        /// go through each room checker
        for (int roomIndex = 1; roomIndex < roomOverlapCheckers.Count; roomIndex++) {

            RoomOverlapChecker roomOverlapChecker = roomOverlapCheckers[roomIndex];

            //... spawn the room of the checker
            Room newRoom = roomOverlapChecker.GetRoomPrefab().Spawn(roomOverlapChecker.transform.position, Containers.Instance.Rooms);

            int roomNumber = roomIndex + 1;
            SetupRoom(newRoom, roomOverlapChecker, roomNumber);
            spawnRoomsDict.Add(roomOverlapChecker, newRoom);
        }
    }

    private void SetupRoom(Room newRoom, RoomOverlapChecker roomOverlapChecker, int roomNumber) {

        newRoom.SetRoomNum(roomNumber);

        //... get the connecting room through the dictionary
        Room connectingRoom = spawnRoomsDict[roomOverlapChecker.GetParentChecker()];

        // get the matching doorways by getting the doorway of the names that match

        // get the new rooms doorway
        string newDoorwayName = roomOverlapChecker.GetDoorwayName(roomOverlapChecker.GetParentChecker());
        newDoorwayName = newDoorwayName[..^7]; // take off '(clone)'
        PossibleDoorway newDoorway = newRoom.GetPossibleDoorway(newDoorwayName);

        // get the connecting rooms doorway
        string existingDoorwayName = roomOverlapChecker.GetParentChecker().GetDoorwayName(roomOverlapChecker);
        existingDoorwayName = existingDoorwayName[..^7]; // take off '(clone)'
        PossibleDoorway existingDoorway = connectingRoom.GetPossibleDoorway(existingDoorwayName);
        connectingRoom.AddCreatedDoorway(existingDoorway);

        // spawn in the hallway to connect the rooms
        SpawnHallway(existingDoorway.GetSide(), existingDoorway.transform.position);
        RemoveTilesForHallway(newRoom, connectingRoom, newDoorway, existingDoorway);

        newRoom.CopyColliderToCameraConfiner(cameraConfiner);

        // create enter and exit triggers
        newRoom.CreateEnterAndExitTriggers(newDoorway);
        connectingRoom.CreateEnterAndExitTriggers(existingDoorway);
    }

    #endregion

    #region Spawn Hallways

    private void RemoveTilesForHallway(Room newRoom, Room existingRoom, PossibleDoorway newDoorway, PossibleDoorway existingDoorway) {
        Tilemap connectingGroundTilemap = existingDoorway.GetSide() == DoorwaySide.Bottom ?
                            existingRoom.GetBotColliderTilemap() : existingRoom.GetGroundTilemap();

        doorwayTileReplacer.DestroyTiles(connectingGroundTilemap,
            existingRoom.GetColliderTilemap(),
            existingDoorway.GetSide(),
            existingDoorway.transform.localPosition);

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
    #endregion
}