using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : StaticInstance<RoomGenerator> {

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
                bool canConnect = newRoom.ConnectRoomToDoorway(connectingDoorway, out PossibleDoorway newDoorway);

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

                // set room num
                int roomNum = roomsSpawned + 2;
                newRoom.SetRoomNum(roomNum);

                // so another room doesn't try to connect with the same doorway
                connectingRoom.RemovePossibleDoorway(connectingDoorway);

                // spawn in the hallway to connect the rooms
                SpawnHallway(connectingDoorway.GetSide(), connectingDoorway.transform.position);
                doorwayTileReplacer.DestroyTiles(connectingRoom.GetGroundTilemap(),
                    connectingRoom.GetColliderTilemap(),
                    connectingDoorway.GetSide(),
                    connectingDoorway.transform.localPosition);

                doorwayTileReplacer.DestroyTiles(newRoom.GetGroundTilemap(),
                    newRoom.GetColliderTilemap(),
                    newDoorway.GetSide(),
                    newDoorway.transform.localPosition);

                newRoom.CopyColliderToCameraConfiner(cameraConfiner);

                roomSpawned = true;

            }

            placedRooms.Add(newRoom);
        }

        // generate shop, boss room entrance, and item chest each on random possible doorways


        isGeneratingRooms = false;
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

        Vector2 hallwayPos = Vector2.zero;
        Transform hallwayPrefab = null;
        if (doorwaySide == DoorwaySide.Top) {
            hallwayPos = new Vector2(doorwayPosition.x, doorwayPosition.y + 4);
            hallwayPrefab = verticalHallwayPrefab;
        }
        else if (doorwaySide == DoorwaySide.Bottom) {
            hallwayPos = new Vector2(doorwayPosition.x, doorwayPosition.y - 4);
            hallwayPrefab = verticalHallwayPrefab;
        }
        else if (doorwaySide == DoorwaySide.Left) {
            hallwayPos = new Vector2(doorwayPosition.x - 4, doorwayPosition.y);
            hallwayPrefab = horizontalHallwayPrefab;
        }
        else if (doorwaySide == DoorwaySide.Right) {
            hallwayPos = new Vector2(doorwayPosition.x + 4, doorwayPosition.y);
            hallwayPrefab = horizontalHallwayPrefab;
        }

        Instantiate(hallwayPrefab, hallwayPos, Quaternion.identity, Containers.Instance.Rooms);
    }

    private bool CheckRoomOverlap(List<Room> placedRooms, Room room) {
        foreach (Room placedRoom in placedRooms) {
            if (placedRoom.GetComponent<Collider2D>().bounds.Intersects(room.GetComponent<Collider2D>().bounds))
                return true;
        }
        return false;
    }

}
