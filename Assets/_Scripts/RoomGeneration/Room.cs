using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour {

    public static event Action<Room> OnAnyRoomEnter_Room; // int: new roomNum
    public static event Action<Room> OnAnyRoomExit_Room; // int: new roomNum

    [SerializeField] private List<PossibleDoorway> possibleDoorways;

    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap colliderTilemap;

    [SerializeField] private PolygonCollider2D cameraConfiner;

    #region Get Methods

    public List<PossibleDoorway> GetPossibleDoorways() {
        return possibleDoorways;
    }

    public Tilemap GetGroundTilemap() {
        return groundTilemap;
    }

    public Tilemap GetColliderTilemap() {
        return colliderTilemap;
    }

    public PolygonCollider2D GetCameraConfiner() {
        return cameraConfiner;
    }

    public int GetRoomNum() {
        return roomNum;
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
    }

    #region Connect Room To Doorway

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

    #endregion

    public void CopyColliderToCameraConfiner(GameObject cameraConfiner) {
        PolygonCollider2D sourceCollider = GetComponent<PolygonCollider2D>();
        if (sourceCollider == null) {
            Debug.LogError("Source GameObject does not have a PolygonCollider2D");
            return;
        }

        PolygonCollider2D targetCollider = cameraConfiner.AddComponent<PolygonCollider2D>();

        targetCollider.usedByComposite = true;
        targetCollider.offset = sourceCollider.offset;

        targetCollider.pathCount = sourceCollider.pathCount;
        for (int i = 0; i < sourceCollider.pathCount; i++) {
            Vector2[] path = sourceCollider.GetPath(i);
            targetCollider.SetPath(i, path);
        }

        // Adjust for different positions if needed
        targetCollider.offset += (Vector2)(transform.position - cameraConfiner.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (RoomGenerator.Instance.IsGeneratingRooms()) {
            return;
        }

        if (collision.gameObject.layer == GameLayers.PlayerLayer) {
            OnAnyRoomEnter_Room?.Invoke(this);
            print("Entered Room: " + name);
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (RoomGenerator.Instance.IsGeneratingRooms()) {
            return;
        }

        if (collision.gameObject.layer == GameLayers.PlayerLayer) {
            OnAnyRoomExit_Room?.Invoke(this);
            print("Exited Room: " + name);
        }
    }

}
