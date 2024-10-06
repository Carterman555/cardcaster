using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : StaticInstance<RoomGenerator> {

    public static event Action OnCompleteGeneration;

    [SerializeField] private RandomInt roomsPerLevel;

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

        //choose a random number of rooms for levels
        roomsPerLevel.Randomize();

        float overlapDetectDelay = 0f;
        yield return new WaitForSeconds(overlapDetectDelay);

        int emergencyCounter = 0;

        List<Room> placedRooms = new();
        placedRooms.Add(startingRoom);

        startingRoom.SetRoomNum(1);

        startingRoom.CopyColliderToCameraConfiner(cameraConfiner);

        for (int roomsSpawned = 0; roomsSpawned < roomsPerLevel.Value; roomsSpawned++) {

            emergencyCounter++;
            if (emergencyCounter > 500) {
                Debug.LogError("Broke out emergency");
                break;
            }

            Room newRoom = null;
            bool roomSpawned = false;

            while (!roomSpawned) {

                emergencyCounter++;
                if (emergencyCounter > 500) {
                    Debug.LogError("Broke out emergency");
                    break;
                }

                // spawn a new room
                newRoom = Instantiate(roomPrefabs.RandomItem(), Containers.Instance.Rooms);

                // connected the new room to a random possible doorway in a random room
                Room connectingRoom = GetRandomRoomWithDoorway(placedRooms);
                PossibleDoorway connectingDoorway = connectingRoom.GetPossibleDoorways().RandomItem();
                bool canConnect = newRoom.TryConnectRoomToDoorway(connectingDoorway, out PossibleDoorway newDoorway);

                //... wait a frame to give time to colliders, so CheckRoomOverlap will work. I also don't know if you are
                //... able to instantiate and destroy an object in the same frame.
                yield return null;

                if (!canConnect) {
                    Destroy(newRoom.gameObject);
                    continue;
                }

                if (CheckRoomOverlap(placedRooms, newRoom)) {
                    Destroy(newRoom.gameObject);
                    continue;
                }

                SetupRoom(roomsSpawned, newRoom, connectingRoom, connectingDoorway, newDoorway);

                roomSpawned = true;

            }

            placedRooms.Add(newRoom);
        }

        // generate shop, boss room entrance, and item chest each on random possible doorways


        isGeneratingRooms = false;

        OnCompleteGeneration?.Invoke();
    }

    private void SetupRoom(int roomsSpawned, Room newRoom, Room connectingRoom, PossibleDoorway connectingDoorway, PossibleDoorway newDoorway) {
        // set room num
        int roomNum = roomsSpawned + 2;
        newRoom.SetRoomNum(roomNum);

        // so another room doesn't try to connect with the same doorway
        connectingRoom.RemovePossibleDoorway(connectingDoorway);

        // spawn in the hallway to connect the rooms
        SpawnHallway(connectingDoorway.GetSide(), connectingDoorway.transform.position);

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


        newRoom.CopyColliderToCameraConfiner(cameraConfiner);

        // create enter and exit triggers
        newRoom.CreateEnterAndExitTriggers(newDoorway);
        connectingRoom.CreateEnterAndExitTriggers(connectingDoorway);
    }

    private Room GetRandomRoomWithDoorway(List<Room> rooms) {

        int emergencyCounter = 0;
        while (emergencyCounter < 500) {
            emergencyCounter++;

            Room chosenRoom = rooms.RandomItem();
            if (chosenRoom.GetPossibleDoorways().Count > 0) {
                return chosenRoom;
            }
        }

        Debug.LogError("Broke out emergency");

        return null;
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

    private bool CheckRoomOverlap(List<Room> placedRooms, Room room) {
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
