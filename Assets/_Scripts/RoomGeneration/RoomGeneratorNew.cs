using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGeneratorNew : StaticInstance<RoomGeneratorNew> {

    public static event Action OnCompleteGeneration;

    [SerializeField] private ScriptableDungeonLayout layoutData;

    [SerializeField] private DoorwayTileDestroyer doorwayTileReplacer;
    [SerializeField] private GameObject cameraConfiner;

    [Header("Prefabs")]
    [SerializeField] private Transform horizontalHallwayPrefab;
    [SerializeField] private Transform verticalHallwayPrefab;

    [SerializeField] private Room startingRoom;
    [SerializeField] private Room[] roomPrefabs;

    private bool isGeneratingRooms;

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

    private IEnumerator GenerateRooms() {

        float overlapDetectDelay = 0f;
        yield return new WaitForSeconds(overlapDetectDelay);

        List<Room> placedRooms = new() {
            startingRoom
        };
        StartCoroutine(SpawnLevelLayout(layoutData.LevelLayout));

        IEnumerator SpawnLevelLayout(RoomConnection layout) {
            foreach (RoomConnection subLayout in layout.connectedRooms) {
                print("Room: " + subLayout.roomType);

                bool roomSpawned = false;

                while (!roomSpawned) {
                    roomSpawned = TrySpawnNewRoom(placedRooms, out Room newRoom);

                    //... wait a frame to give time to colliders, so CheckRoomOverlap will work. I also don't know if you are
                    //... able to instantiate and destroy an object in the same frame.
                    yield return null;
                }

                SetupRoom(newRoom);

                StartCoroutine(SpawnLevelLayout(subLayout));
            }
        }

        isGeneratingRooms = false;

        OnCompleteGeneration?.Invoke();
    }

    private bool TrySpawnNewRoom(List<Room> placedRooms, out Room newRoom) {
        // spawn a new room
        newRoom = Instantiate(roomPrefabs.RandomItem(), Containers.Instance.Rooms);

        // connected the new room to a random possible doorway in a random room
        Room connectingRoom = GetRandomRoomWithDoorway(placedRooms);
        PossibleDoorway connectingDoorway = connectingRoom.GetPossibleDoorways().RandomItem();
        bool canConnect = newRoom.ConnectRoomToDoorway(connectingDoorway, out PossibleDoorway newDoorway);

        return canConnect && !DoesRoomOverlap(placedRooms, newRoom);
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

    private bool DoesRoomOverlap(List<Room> placedRooms, Room room) {
        foreach (Room placedRoom in placedRooms) {
            if (placedRoom.GetComponent<Collider2D>().bounds.Intersects(room.GetComponent<Collider2D>().bounds))
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
