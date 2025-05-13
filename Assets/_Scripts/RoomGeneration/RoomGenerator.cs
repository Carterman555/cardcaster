using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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

            MMAdditiveSceneLoadingManager.AllowUnload();
            OnCompleteGeneration?.Invoke();
        }
        else {
            GenerateRooms(GameSceneManager.Instance.CurrentEnvironment);
        }
    }

    public void GenerateRooms(EnvironmentType environmentType) {
        currentEnvironmentType = environmentType;
        //currentEnvironmentType = EnvironmentType.BlueStone;
        StartCoroutine(GenerateRoomsCor());
    }

    private IEnumerator GenerateRoomsCor() {
        yield return StartCoroutine(GenerateLayout());

        SpawnRooms();
        RemoveOverlapCheckers();

        yield return null;

        isGeneratingRooms = false;

        OnCompleteGeneration?.Invoke();
    }

    #region Generate Layout

    [Header("Generate Layout")]
    [SerializeField] private RoomOverlapChecker roomOverlapCheckerPrefab;
    [SerializeField] private bool debug;

    private bool failedRoomCreation;
    private Dictionary<RoomType, List<ScriptableRoom>> usedRooms;

    private int attemptNum = 0;

    private IEnumerator GenerateLayout() {
        do {
            attemptNum++;
            if (debug) Debug.Log($"ATTEMPT {attemptNum} TO GENERATE LAYOUT");

            failedRoomCreation = false;
            yield return StartCoroutine(TryGenerateLayout());

        }
        while (failedRoomCreation);
    }

    private IEnumerator TryGenerateLayout() {

        // reset the generation in case this is a retry
        ClearOverlapCheckers();
        SetupUsedRoomsDict();

        ScriptableLevelLayout levelLayout = ResourceSystem.Instance.GetRandomLayout();

        if (debug) Debug.Log("Trying to generate layout with " + levelLayout.name);

        // spawn first room checker
        RoomOverlapChecker newRoomChecker = roomOverlapCheckerPrefab.Spawn(Vector2.zero, Containers.Instance.RoomOverlapCheckers);
        Room entranceRoomPrefab = GetRandomUniqueRoom(levelLayout.RoomConnections[0].RoomType).Prefab;
        newRoomChecker.Setup(entranceRoomPrefab, null);
        roomOverlapCheckers.Add(newRoomChecker);

        SetChildrensChecker(levelLayout, levelLayout.RoomConnections[0], newRoomChecker, entranceRoomPrefab);

        // spawn all other room checkers
        yield return StartCoroutine(SpawnRoomCheckers(levelLayout));

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

    private IEnumerator SpawnRoomCheckers(ScriptableLevelLayout levelLayout) {
        if (debug) Debug.Log($"Starting SpawnRoomCheckers for layout with {levelLayout.RoomConnections.Length} connected rooms");

        foreach (RoomConnection roomConnection in levelLayout.RoomConnections) {

            // already setup first room, so skip
            if (roomConnection == levelLayout.RoomConnections[0]) {
                continue;
            }

            if (debug) Debug.Log($"Processing subLayout of type {roomConnection.RoomType}");
            bool canSpawn = false;
            ScriptableRoom newRoomScriptable = null;
            Room newRoomPrefab = null;

            int tryRoomCounter = 0;

            // Until it spawns a room that can be spawned
            while (!canSpawn) {
                if (debug)
                    Debug.Log($"Attempt #{tryRoomCounter} to spawn room of type {roomConnection.RoomType} to connect to {roomConnection.ParentRoomOverlapChecker.GetRoomPrefab().name}");

                tryRoomCounter++;
                int tryRoomAmount = 5;
                if (tryRoomCounter > tryRoomAmount) {
                    failedRoomCreation = true;
                    if (debug)
                        Debug.Log($"Failed to create room after {tryRoomAmount} attempts");
                    yield break; // exits method
                }

                newRoomScriptable = GetRandomUniqueRoom(roomConnection.RoomType);
                newRoomPrefab = newRoomScriptable.Prefab;
                if (debug)
                    Debug.Log($"Selected room: {newRoomScriptable.name}");

                // Try to spawn this room
                yield return StartCoroutine(TrySpawnRoomChecker(newRoomPrefab, roomConnection.ParentRoomOverlapChecker, (success) => canSpawn = success));
            }

            usedRooms[newRoomScriptable.RoomType].Add(newRoomScriptable);
            if (debug)
                Debug.Log($"Successfully spawned room: {newRoomScriptable.name}");

            RoomOverlapChecker newRoomChecker = roomOverlapCheckers.Last();
            SetChildrensChecker(levelLayout, roomConnection, newRoomChecker, newRoomPrefab);
        }
    }

    // Choose random unique room with matching type
    private ScriptableRoom GetRandomUniqueRoom(RoomType roomType) {
        ScriptableRoom newRoomScriptable;
        List<ScriptableRoom> availableRooms = ResourceSystem.Instance.GetRooms(roomType)
            .Where(room => room.EnvironmentType == currentEnvironmentType)
            .Where(room => !usedRooms[roomType].Contains(room)) // comment to reuse same rooms because not enough yet
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

        if (debug)
            Debug.Log($"Found {availableRooms.Count} available rooms of type {roomType}");
        newRoomScriptable = availableRooms.RandomItem();
        return newRoomScriptable;
    }

    private void SetChildrensChecker(ScriptableLevelLayout levelLayout, RoomConnection roomConnection, RoomOverlapChecker newRoomChecker, Room roomPrefab) {
        foreach (string connectedRoomID in roomConnection.ConnectedRoomIDs) {
            RoomConnection childRoomConnection = levelLayout.RoomConnections.First(c => c.RoomID == connectedRoomID);
            childRoomConnection.ParentRoomOverlapChecker = newRoomChecker;
        }
    }

    /// <summary>
    /// go through all the doorways of the connecting room to see if the new room can connect to any of them. If it can,
    /// then return the position where the connection can be made
    /// </summary>
    private IEnumerator TrySpawnRoomChecker(Room newRoomPrefab, RoomOverlapChecker existingRoomChecker, Action<bool> callback) {
        if (debug)
            Debug.Log($"Attempting to spawn room checker for {newRoomPrefab.name}");

        RoomOverlapChecker newRoomChecker = roomOverlapCheckerPrefab.Spawn(Containers.Instance.RoomOverlapCheckers);
        newRoomChecker.Setup(newRoomPrefab, existingRoomChecker);

        foreach (PossibleDoorway existingDoorway in existingRoomChecker.GetPossibleDoorways()) {
            if (debug)
                Debug.Log($"Checking doorway compatibility for {existingDoorway.GetSide()} side of {existingRoomChecker.GetRoomPrefab().name}");

            if (newRoomChecker.CanConnectToDoorwaySide(existingDoorway.GetSide())) {

                PossibleDoorway[] newRoomDoorways = newRoomChecker.GetConnectingDoorways(existingDoorway.GetSide());
                if (debug)
                    Debug.Log($"Found {newRoomDoorways.Length} doorways in new room to connect to {existingDoorway.GetSide()} side");

                foreach (PossibleDoorway newDoorway in newRoomDoorways) {
                    if (debug)
                        Debug.Log($"Attempting to connect {existingDoorway.name} in {existingRoomChecker.GetRoomPrefab().name} to {newDoorway.name} in {newRoomPrefab.name}");
                    newRoomChecker.MoveToConnectionPos(existingDoorway, newDoorway);

                    // Yield to ensure collider overlap detection works
                    yield return null;

                    if (!newRoomChecker.OverlapsWithRoomChecker(roomOverlapCheckers)) {
                        if (debug)
                            Debug.Log($"Successfully found valid position for {newRoomPrefab.name}");

                        roomOverlapCheckers.Add(newRoomChecker);

                        newRoomChecker.AddCreatedDoorway(newDoorway, existingRoomChecker);
                        existingRoomChecker.AddCreatedDoorway(existingDoorway, newRoomChecker);

                        callback(true);
                        yield break;
                    }

                    if (debug) Debug.Log("Room overlaps with existing rooms, trying next doorway");
                }
            }
        }

        if (debug)
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
