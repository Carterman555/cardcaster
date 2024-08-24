using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour {

    [SerializeField] private TriggerContactTracker roomOverlapTracker;

    public bool IsOverlappingRoom() {
        return roomOverlapTracker.HasContact();
    }

    [SerializeField] private List<PossibleDoorway> possibleDoorways;

    public List<PossibleDoorway> GetPossibleDoorwayPoints() {
        return possibleDoorways;
    }

    public void ConnectRoomToDoorway(PossibleDoorway existingRoomDoorway) {

        List<PossibleDoorway> connectableDoorways = possibleDoorways
            .Where(doorway => GetOppositeSide(doorway.GetSide()) == existingRoomDoorway.GetSide())
            .ToList();
        PossibleDoorway newRoomDoorway = connectableDoorways.RandomItem();

        possibleDoorways.Remove(newRoomDoorway);

        // Position the new room so the doorways align
        Vector3 offset = existingRoomDoorway.transform.position - newRoomDoorway.transform.position;
        transform.position += offset;
    }

    private DoorwaySide GetOppositeSide(DoorwaySide currentSide) {
        if (currentSide == DoorwaySide.Top) {
            return DoorwaySide.Bottom;
        }
        else if (currentSide == DoorwaySide.Bottom) {
            return DoorwaySide.Top;
        }
        else if (currentSide == DoorwaySide.Left) {
            return DoorwaySide.Right;
        }
        else if (currentSide == DoorwaySide.Right) {
            return DoorwaySide.Left;
        }
        else {
            return default;
        }
    }

}
