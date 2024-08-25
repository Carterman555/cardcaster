using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour {

    public static event Action<int> OnAnyRoomChange; // int: new roomNum

    [SerializeField] private TriggerContactTracker roomOverlapTracker;

    [SerializeField] private List<PossibleDoorway> possibleDoorways;

    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap colliderTilemap;

    #region Get Methods

    public bool IsOverlappingRoom() {
        return roomOverlapTracker.HasContact();
    }

    public List<PossibleDoorway> GetPossibleDoorways() {
        return possibleDoorways;
    }

    public Tilemap GetGroundTilemap() {
        return groundTilemap;
    }

    public Tilemap GetColliderTilemap() {
        return colliderTilemap;
    }

    #endregion

    public void RemovePossibleDoorway(PossibleDoorway possibleDoorway) {
        possibleDoorways.Remove(possibleDoorway);
    }

    private int roomNum;
    public void SetRoomNum(int roomNum) {
        this.roomNum = roomNum;

        IHasRoomNum[] hasRoomNumChildren = GetComponentsInChildren<IHasRoomNum>();
        foreach (IHasRoomNum hasRoomNum in hasRoomNumChildren) {
            hasRoomNum.SetRoomNum(roomNum);
        }

        // when starts, the starting room invokes room change event
        if (roomNum == 1) {
            OnAnyRoomChange?.Invoke(roomNum);
        }
    }

    public bool ConnectRoomToDoorway(PossibleDoorway connectingRoomDoorway, out PossibleDoorway newDoorway) {

        List<PossibleDoorway> connectableDoorways = possibleDoorways
            .Where(doorway => GetOppositeSide(doorway.GetSide()) == connectingRoomDoorway.GetSide())
            .ToList();

        if (connectableDoorways.Count == 0) {
            newDoorway = null;
            return false;
        }

        newDoorway = connectableDoorways.RandomItem();
        RemovePossibleDoorway(newDoorway);

        // Position the new room so the doorways align
        Vector2 offset = connectingRoomDoorway.transform.position - newDoorway.transform.position;
        offset += GetHallwayOffset(connectingRoomDoorway.GetSide());
        transform.position += (Vector3)offset;

        return true;
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

    private Vector2 GetHallwayOffset(DoorwaySide currentSide) {

        float hallwayLength = 8;
        if (currentSide == DoorwaySide.Top) {
            return new Vector2(0, hallwayLength);
        }
        else if (currentSide == DoorwaySide.Bottom) {
            return new Vector2(0, -hallwayLength);
        }
        else if (currentSide == DoorwaySide.Left) {
            return new Vector2(-hallwayLength, 0);
        }
        else if (currentSide == DoorwaySide.Right) {
            return new Vector2(hallwayLength, 0);
        }
        else {
            return Vector2.zero;
        }
    }

}
