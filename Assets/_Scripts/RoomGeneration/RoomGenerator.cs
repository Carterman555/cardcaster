using DG.Tweening;
using MoreMountains.Tools;
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

    private EnvironmentType currentEnvironmentType;

    public bool IsGeneratingRooms() {
        return isGeneratingRooms;
    }

    protected override void Awake() {
        base.Awake();
        isGeneratingRooms = true;
    }

    private void Start() {
        if (GameSceneManager.Instance.Tutorial) {
            SpawnTrainingRoom();
        }
        else {
            GenerateRooms(GameSceneManager.Instance.GetEnvironment());
        }
    }

    public void GenerateRooms(EnvironmentType environmentType) {
        currentEnvironmentType = environmentType;

        currentEnvironmentType = EnvironmentType.BlueStone;

        StartCoroutine(GenerateRoomsCor());
    }

    private IEnumerator GenerateRoomsCor() {
        yield return StartCoroutine(GenerateLayout());

        SpawnRooms();
        RemoveOverlapCheckers();

        yield return null;

        isGeneratingRooms = false;

        MMAdditiveSceneLoadingManager.AllowUnload();

        OnCompleteGeneration?.Invoke();
    }

    #region Generate Layout

    [Header("Generate Layout")]
    [SerializeField] private RoomOverlapChecker roomOverlapCheckerPrefab;

    private bool failedRoomCreation;
    private Dictionary<RoomType, List<ScriptableRoom>> usedRooms;

    private IEnumerator GenerateLayout() {
        do {
            failedRoomCreation = false;
            yield return StartCoroutine(TryGenerateLayout());
        }
        while (failedRoomCreation);
    }

    private IEnumerator TryGenerateLayout() {

        // reset the generation in case this is a retry
        ClearOverlapCheckers();
        SetupUsedRoomsDict();

        ScriptableLevelLayout layoutData = ResourceSystem.Instance.GetRandomLayout();

        // spawn first room checker
        RoomOverlapChecker newRoomChecker = roomOverlapCheckerPrefab.Spawn(Vector2.zero, Containers.Instance.RoomOverlapCheckers);
        Room entranceRoomPrefab = GetRandomUniqueRoom(layoutData.LevelLayout.roomType).Prefab;
        newRoomChecker.Setup(entranceRoomPrefab, null);
        roomOverlapCheckers.Add(newRoomChecker);

        SetChildrensChecker(layoutData.LevelLayout, newRoomChecker, entranceRoomPrefab);

        // spawn all other room checkers
        yield return StartCoroutine(SpawnRoomCheckers(layoutData.LevelLayout));

        isGeneratingRooms = false;
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
        Debug.Log($"Starting SpawnRoomCheckers for layout with {layout.connectedRooms.Count} connected rooms");

        // Recursively go through each room type in layout
        foreach (RoomConnection subLayout in layout.connectedRooms) {
            Debug.Log($"Processing subLayout of type {subLayout.roomType}");
            bool canSpawn = false;
            ScriptableRoom newRoomScriptable = null;
            Room newRoomPrefab = null;

            int tryRoomCounter = 0;

            // Until it spawns a room that can be spawned
            while (!canSpawn) {
                Debug.Log($"Attempt #{tryRoomCounter} to spawn room of type {subLayout.roomType}");

                tryRoomCounter++;
                int tryRoomAmount = 15;
                if (tryRoomCounter > tryRoomAmount) {
                    failedRoomCreation = true;
                    Debug.LogError($"Failed to create room after {tryRoomAmount} attempts");
                    yield break; // exits method
                }

                newRoomScriptable = GetRandomUniqueRoom(subLayout.roomType);
                newRoomPrefab = newRoomScriptable.Prefab;
                Debug.Log($"Selected room: {newRoomScriptable.name}");

                // Try to spawn this room
                yield return StartCoroutine(TrySpawnRoomChecker(newRoomPrefab, subLayout.ParentRoomOverlapChecker, (success) => canSpawn = success));
            }

            usedRooms[newRoomScriptable.RoomType].Add(newRoomScriptable);
            Debug.Log($"Successfully spawned room: {newRoomScriptable.name}");

            RoomOverlapChecker newRoomChecker = roomOverlapCheckers.Last();
            SetChildrensChecker(subLayout, newRoomChecker, newRoomPrefab);

            if (!failedRoomCreation) {
                Debug.Log("Starting recursive spawn for connected rooms");
                yield return StartCoroutine(SpawnRoomCheckers(subLayout)); // Recursive spawn
            }
        }
    }

    // Choose random unique room with matching type
    private ScriptableRoom GetRandomUniqueRoom(RoomType roomType) {
        Debug.Log($"Getting random room of type {roomType}");
        ScriptableRoom newRoomScriptable;
        List<ScriptableRoom> availableRooms = ResourceSystem.Instance.GetRooms(roomType)
            .Where(room => room.EnvironmentType == currentEnvironmentType)
            //.Where(room => !usedRooms[roomType].Contains(room)) // commented to reuse same rooms because not enough yet
            .ToList();

        // doesn't need to be unique if reward room
        if (roomType == RoomType.Reward) {
            availableRooms = ResourceSystem.Instance.GetRooms(roomType)
                .Where(room => room.EnvironmentType == currentEnvironmentType)
                .ToList();
        }

        if (availableRooms.Count == 0) {
            Debug.LogError("There are no rooms of type " + roomType + " available!");
        }

        Debug.Log($"Found {availableRooms.Count} available rooms of type {roomType}");
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
        Debug.Log($"Attempting to spawn room checker for {newRoomPrefab.name}");

        RoomOverlapChecker newRoomChecker = roomOverlapCheckerPrefab.Spawn(Containers.Instance.RoomOverlapCheckers);
        newRoomChecker.Setup(newRoomPrefab, existingRoomChecker);

        foreach (PossibleDoorway existingDoorway in existingRoomChecker.GetPossibleDoorways()) {
            Debug.Log($"Checking doorway compatibility for {existingDoorway.GetSide()}");

            if (newRoomChecker.CanConnectToDoorwaySide(existingDoorway.GetSide())) {

                PossibleDoorway newRoomDoorway = newRoomChecker.GetRandomConnectingDoorway(existingDoorway.GetSide());
                newRoomChecker.MoveToConnectionPos(existingDoorway, newRoomDoorway);

                // Yield to ensure collider overlap detection works
                yield return null;

                if (!newRoomChecker.OverlapsWithRoomChecker(roomOverlapCheckers)) {
                    Debug.Log($"Successfully found valid position for {newRoomPrefab.name}");

                    roomOverlapCheckers.Add(newRoomChecker);

                    newRoomChecker.AddCreatedDoorway(newRoomDoorway, existingRoomChecker);
                    existingRoomChecker.AddCreatedDoorway(existingDoorway, newRoomChecker);

                    callback(true);
                    yield break;
                }
                Debug.Log("Room overlaps with existing rooms, trying next doorway");
            }
        }

        Debug.Log($"Failed to find valid position for {newRoomPrefab.name}");
        newRoomChecker.gameObject.ReturnToPool();
        callback(false);
    }

    #endregion

    #region Spawn Rooms

    [Header("Spawn Rooms")]
    [SerializeField] private DoorwayTileDestroyer doorwayTileReplacer;
    [SerializeField] private Room trainingRoomPrefab;

    private Dictionary<RoomOverlapChecker, Room> spawnRoomsDict = new();

    /// <summary>
    /// TODO - write summary
    /// </summary>
    private void SpawnRooms() {

        spawnRoomsDict.Clear();

        // setup first room seperately because the setup is different
        RoomOverlapChecker firstOverlapChecker = roomOverlapCheckers[0];
        Room firstRoom = firstOverlapChecker.GetRoomPrefab().Spawn(firstOverlapChecker.transform.position, Containers.Instance.Rooms);
        firstRoom.SetRoomNum(1);
        spawnRoomsDict.Add(firstOverlapChecker, firstRoom);

        // go through each room checker
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
        newRoom.AddCreatedDoorway(newDoorway);

        // get the connecting rooms doorway
        string existingDoorwayName = roomOverlapChecker.GetParentChecker().GetDoorwayName(roomOverlapChecker);
        existingDoorwayName = existingDoorwayName[..^7]; // take off '(clone)'
        PossibleDoorway existingDoorway = connectingRoom.GetPossibleDoorway(existingDoorwayName);
        connectingRoom.AddCreatedDoorway(existingDoorway);

        // spawn and setup in the hallway to connect the rooms
        Transform hallway = SpawnHallway(existingDoorway.GetSide(), existingDoorway.transform.position);
        RemoveTilesForHallway(newRoom, connectingRoom, newDoorway, existingDoorway);
        RemoveObjectsForHallway(newDoorway.transform.position, existingDoorway.transform.position);
        AddRoomsToHallwayLight(hallway, connectingRoom.GetRoomNum(), newRoom.GetRoomNum());

        // create enter and exit triggers
        newRoom.CreateEnterAndExitTriggers(newDoorway);
        connectingRoom.CreateEnterAndExitTriggers(existingDoorway);
    }

    private void SpawnTrainingRoom() {
        Room trainingRoom = trainingRoomPrefab.Spawn(Vector2.zero, Containers.Instance.Rooms);
    }

    #endregion

    #region Spawn Hallways

    [Header("Spawn Hallways")]
    [SerializeField] private List<HallwaySet> hallwaySets;

    [Serializable]
    public struct HallwaySet {
        public EnvironmentType EnvironmentType;
        public Transform HorizontalHallwayPrefab;
        public Transform VerticalHallwayPrefab;
    }

    private Transform SpawnHallway(DoorwaySide doorwaySide, Vector2 doorwayPosition) {

        Transform hallwayPrefab = null;
        if (doorwaySide == DoorwaySide.Top || doorwaySide == DoorwaySide.Bottom) {
            hallwayPrefab = hallwaySets.FirstOrDefault(set => set.EnvironmentType == currentEnvironmentType).VerticalHallwayPrefab;
        }
        else if (doorwaySide == DoorwaySide.Left || doorwaySide == DoorwaySide.Right) {
            hallwayPrefab = hallwaySets.FirstOrDefault(set => set.EnvironmentType == currentEnvironmentType).HorizontalHallwayPrefab;
        }

        float hallwayOffset = 4;
        Vector2 hallwayPos = doorwayPosition + SideToDirection(doorwaySide) * hallwayOffset;
        return hallwayPrefab.Spawn(hallwayPos, Quaternion.identity, Containers.Instance.Hallways);
    }

    private void RemoveTilesForHallway(Room newRoom, Room existingRoom, PossibleDoorway newDoorway, PossibleDoorway existingDoorway) {
        Tilemap connectingWallTilemap = existingDoorway.GetSide() == DoorwaySide.Top ?
                            existingRoom.GetTopWallsTilemap() : existingRoom.GetBotWallsTilemap();

        doorwayTileReplacer.DestroyTiles(connectingWallTilemap,
            existingDoorway.GetSide(),
            existingDoorway.transform.localPosition);

        Tilemap newWallTilemap = newDoorway.GetSide() == DoorwaySide.Top ?
            newRoom.GetTopWallsTilemap() : newRoom.GetBotWallsTilemap();

        doorwayTileReplacer.DestroyTiles(newWallTilemap,
            newDoorway.GetSide(),
            newDoorway.transform.localPosition);
    }

    private void RemoveObjectsForHallway(Vector2 newDoorwayPosition, Vector2 existingDoorwayPosition) {

        float centerX = (newDoorwayPosition.x + existingDoorwayPosition.x) / 2f;
        float centerY = (newDoorwayPosition.y + existingDoorwayPosition.y) / 2f;

        Vector2 center = new Vector2(centerX, centerY);

        float range = 2f;

        float xDistance = Mathf.Abs(newDoorwayPosition.x - existingDoorwayPosition.x);
        float yDistance = Mathf.Abs(newDoorwayPosition.y - existingDoorwayPosition.y);

        Vector2 boxSize = new Vector2(xDistance + range, yDistance + range);

        Collider2D[] cols = Physics2D.OverlapBoxAll(center, boxSize, 0, GameLayers.RoomObjectLayerMask);

        foreach (Collider2D col in cols) {
            col.gameObject.ReturnToPool();
        }
    }

    private void AddRoomsToHallwayLight(Transform hallway, int connectingRoomNum, int newRoomNum) {
        Hallway hallwayLight = hallway.GetComponentInChildren<Hallway>();
        hallwayLight.SetConnectingRoomNums(newRoomNum, connectingRoomNum);
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

    private void RemoveOverlapCheckers() {
        foreach (RoomOverlapChecker overlapChecker in roomOverlapCheckers) {
            overlapChecker.gameObject.ReturnToPool();
        }
        roomOverlapCheckers.Clear();
    }

}
