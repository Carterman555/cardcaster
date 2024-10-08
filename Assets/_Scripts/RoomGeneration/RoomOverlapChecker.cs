using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomOverlapChecker : MonoBehaviour {

    private Room roomPrefab;

    private List<PossibleDoorway> possibleDoorways = new();

    public void Setup(Room roomPrefab) {
        this.roomPrefab = roomPrefab;
        CopyCollider(roomPrefab.GetComponent<PolygonCollider2D>());

        CreatePossibleDoorways(roomPrefab.GetPossibleDoorways());
    }

    private void OnDisable() {
        RemovePossibleDoorways();
    }

    public Room GetRoomPrefab() {
        return roomPrefab;
    }

    public List<PossibleDoorway> GetPossibleDoorways() {
        return possibleDoorways;
    }

    public void RemovePossibleDoorway(PossibleDoorway possibleDoorway) {
        possibleDoorways.Remove(possibleDoorway);
    }

    private void CreatePossibleDoorways(List<PossibleDoorway> possibleDoorwaysToCreate) {
        foreach (var roomPossibleDoorway in possibleDoorwaysToCreate) {
            PossibleDoorway newPossibleDoorway = roomPossibleDoorway.Spawn(roomPossibleDoorway.transform.position, transform);
            newPossibleDoorway.SetSide(roomPossibleDoorway.GetSide());
            possibleDoorways.Add(newPossibleDoorway);
        }
    }

    private void RemovePossibleDoorways() {
        foreach (var possibleDoorway in possibleDoorways) {
            possibleDoorway.gameObject.ReturnToPool();
        }
        possibleDoorways.Clear();
    }

    #region Overlap Collider

    private PolygonCollider2D polygonCollider;

    private void Awake() {
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    private void CopyCollider(PolygonCollider2D prefabCollider) {
        polygonCollider.pathCount = prefabCollider.pathCount;
        for (int i = 0; i < prefabCollider.pathCount; i++) {
            Vector2[] path = prefabCollider.GetPath(i);
            polygonCollider.SetPath(i, path);
        }
    }

    public bool OverlapsWithRoomChecker(List<RoomOverlapChecker> roomOverlapTriggers) {
        foreach (RoomOverlapChecker overlapTrigger in roomOverlapTriggers) {
            if (overlapTrigger.GetComponent<PolygonCollider2D>().bounds.Intersects(polygonCollider.bounds))
                return true;
        }
        return false;
    }

    #endregion

    #region Connect Room To Doorway

    public void MoveToConnectionPos(PossibleDoorway connectingRoomDoorway, PossibleDoorway newDoorway) {
        Vector2 connectionPos = connectingRoomDoorway.transform.position - newDoorway.transform.localPosition;

        float hallwayLength = 8;
        Vector2 hallwayOffset = RoomGenerator.Instance.SideToDirection(connectingRoomDoorway.GetSide()) * hallwayLength;
        connectionPos += hallwayOffset;

        transform.position = connectionPos;
    }

    public bool CanConnectToDoorwaySide(DoorwaySide doorwaySide) {
        bool possibleConnection = possibleDoorways.Any(doorway => GetOppositeSide(doorway.GetSide()) == doorwaySide);
        return possibleConnection;
    }

    public PossibleDoorway GetRandomConnectingDoorway(DoorwaySide connectingDoorwaySide) {
        if (!CanConnectToDoorwaySide(connectingDoorwaySide)) {
            return null;
        }

        List<PossibleDoorway> connectableDoorways = possibleDoorways
            .Where(doorway => GetOppositeSide(doorway.GetSide()) == connectingDoorwaySide)
            .ToList();

        return connectableDoorways.RandomItem();
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

    #endregion
}
