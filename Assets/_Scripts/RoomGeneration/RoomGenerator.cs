using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour {

    [SerializeField] private RandomInt roomsPerLevel;

    [SerializeField] private Room startingRoom;
    [SerializeField] private Room[] roomPrefabs;

    private void Start() {
        StartCoroutine(GenerateRooms());
    }

    private IEnumerator GenerateRooms() {

        //choose a random number of rooms for levels
        roomsPerLevel.Randomize();

        int emergencyCounter = 0;

        List<PossibleDoorway> allPossibleDoorwayPoints = new();
        allPossibleDoorwayPoints.AddRange(startingRoom.GetPossibleDoorwayPoints());

        int roomsSpawned = 0;
        //while that number of rooms has not been spawned yet
        while (roomsSpawned < roomsPerLevel.Value) {

            emergencyCounter++;
            if (emergencyCounter > 50) {
                print("Broke out emergency");
                break;
            }

            PossibleDoorway existingRoomDoorwayPoint;
            Room newRoom;
            while (true) {
                // choose a random possible doorway in a random room
                newRoom = Instantiate(roomPrefabs.RandomItem(), Containers.Instance.Rooms);

                existingRoomDoorwayPoint = allPossibleDoorwayPoints.RandomItem();
                newRoom.ConnectRoomToDoorway(existingRoomDoorwayPoint);

                float overlapDetectDelay = 0.1f;
                yield return new WaitForSeconds(overlapDetectDelay);

                if (newRoom.IsOverlappingRoom()) {
                    Destroy(newRoom.gameObject);
                }
                else {
                    break;
                }

                emergencyCounter++;
                if (emergencyCounter > 50) {
                    print("Broke out emergency");
                    break;
                }
            }

            // add the new room's doorway points
            allPossibleDoorwayPoints.AddRange(newRoom.GetPossibleDoorwayPoints());
            allPossibleDoorwayPoints.Remove(existingRoomDoorwayPoint);
            roomsSpawned++;
        }

        //generate shop, boss room entrance, and item chest each on random possible doorways

    }

}
