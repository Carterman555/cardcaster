using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class RoomGeneratorNew : StaticInstance<RoomGeneratorNew> {

    public static event Action OnCompleteGeneration;

    [SerializeField] private ScriptableDungeonLayout layoutData;

    [SerializeField] private DoorwayTileDestroyer doorwayTileReplacer;
    [SerializeField] private GameObject cameraConfiner;

    [Header("Prefabs")]
    [SerializeField] private Transform horizontalHallwayPrefab;
    [SerializeField] private Transform verticalHallwayPrefab;

    [SerializeField] private Room[] roomPrefabs;

    private bool isGeneratingRooms;

    private List<RoomInfo> roomsToSpawn = new();
    private List<Collider2D> roomOverlapTriggers = new();

    public bool IsGeneratingRooms() {
        return isGeneratingRooms;
    }

    protected override void Awake() {
        base.Awake();
        isGeneratingRooms = true;
    }

    private async void Start() {
        await GenerateRoomsAsync();
    }


    /// <summary>
    /// Recursively go through each room type in layout
	///    While the room is can't be spawned
	///	      Choose random unique room with matching type(doesn't need to be unique if chest room)
    ///       Try to spawn this room
    /// </summary>
    private async Task GenerateRoomsAsync() {

        await SetRoomToSpawnAsync(layoutData.LevelLayout);

        async Task SetRoomToSpawnAsync(RoomConnection layout) {
            foreach (RoomConnection subLayout in layout.connectedRooms) {
                print("Room: " + subLayout.roomType);

                bool canSpawn = false;
                Room newRoomPrefab = null;
                Vector2 spawnPosition = Vector2.zero;

                int breakOutCounter = 0;

                while (!canSpawn) {

                    breakOutCounter++;
                    if (breakOutCounter > 500) {
                        Debug.LogError("Breakout Error");
                        break;
                    }

                    newRoomPrefab = roomPrefabs.RandomItem();
                    (bool _canSpawn, Vector2 _spawnPosition) = await CanSpawnRoomAsync(newRoomPrefab, );
                    canSpawn = _canSpawn;
                    spawnPosition = _spawnPosition;
                }

                RoomInfo roomInfo;
                roomInfo.RoomPrefab = newRoomPrefab;
                roomInfo.Position = spawnPosition;
                roomsToSpawn.Add(roomInfo);

                //SetupRoom(newRoom);
                await SetRoomToSpawnAsync(subLayout); // Recursive spawn
            }
        }

        foreach (RoomInfo roomInfo in roomsToSpawn) {
            print("Room: " + roomInfo.RoomPrefab.name + ", pos: " + roomInfo.Position);
        }

        isGeneratingRooms = false;
        OnCompleteGeneration?.Invoke();
    }

    /// <summary>
    /// go through all the doorways of the connecting room to see if the new room can connect to any of them. If it can,
    /// then return the position where the connection can be made
    /// </summary>
    public async Task<(bool canSpawn, Vector2 spawnPosition)> CanSpawnRoomAsync(Room newRoomPrefab, Room existingRoom) {
        
        foreach (PossibleDoorway existingDoorway in existingRoom.GetPossibleDoorways()) {
            if (newRoomPrefab.CanConnectToDoorwaySide(existingDoorway.GetSide())) {
                PolygonCollider2D prefabCollider = newRoomPrefab.GetComponent<PolygonCollider2D>();
                GameObject triggerObject = Instantiate(new GameObject(), Containers.Instance.Rooms);
                PolygonCollider2D trigger = triggerObject.AddComponent<PolygonCollider2D>();

                // Copy the collider data
                CopyColliderToNew(prefabCollider, trigger);
                PossibleDoorway newRoomDoorway = newRoomPrefab.GetRandomConnectingDoorway(existingDoorway.GetSide());
                triggerObject.transform.position = newRoomPrefab.GetConnectionPos(existingDoorway, newRoomDoorway);

                // Yield to ensure collider overlap detection works
                await Task.Yield();

                if (!DoesTriggerOverlap(roomOverlapTriggers, trigger)) {
                    roomOverlapTriggers.Add(trigger);
                    return (true, triggerObject.transform.position);
                }
                else {
                    Destroy(triggerObject);
                }
            }
        }

        return (false, Vector2.zero);
    }


    private static void CopyColliderToNew(PolygonCollider2D prefabCollider, PolygonCollider2D trigger) {
        trigger.pathCount = prefabCollider.pathCount;
        for (int i = 0; i < prefabCollider.pathCount; i++) {
            Vector2[] path = prefabCollider.GetPath(i);
            trigger.SetPath(i, path);
        }
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

    private bool DoesRoomOverlap(List<Collider2D> roomOverlapTriggers, Room room) {
        foreach (Collider2D overlapTrigger in roomOverlapTriggers) {
            if (overlapTrigger.bounds.Intersects(room.GetComponent<Collider2D>().bounds))
                return true;
        }
        return false;
    }

    private bool DoesTriggerOverlap(List<Collider2D> roomOverlapTriggers, Collider2D trigger) {
        foreach (Collider2D overlapTrigger in roomOverlapTriggers) {
            if (overlapTrigger.bounds.Intersects(trigger.bounds))
                return true;
        }
        return false;
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

public struct RoomInfo {
    public Room RoomPrefab;
    public Vector3 Position;
}