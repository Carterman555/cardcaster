using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour {

    public static event Action<Room> OnAnyRoomEnter_Room;
    public static event Action<Room> OnAnyRoomExit_Room;

    private static int enteredRoomNum;

    private int roomNum;

    [SerializeField] private List<PossibleDoorway> possibleDoorways;
    private List<PossibleDoorway> createdDoorways = new();

    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap colliderTilemap;

    [SerializeField] private PolygonCollider2D cameraConfiner;

    [SerializeField] private TriggerContactTracker enterTrigger;
    [SerializeField] private TriggerContactTracker exitTrigger;
    [SerializeField] private DoorBlocker doorBlockerPrefab;

    [SerializeField] private bool noEnemies;
    private bool roomCleared;

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

    public int GetRoomNum() {
        return roomNum;
    }

    public bool IsRoomCleared() {
        return roomCleared;
    }

    #endregion

    public void RemovePossibleDoorway(PossibleDoorway possibleDoorway) {
        possibleDoorways.Remove(possibleDoorway);
        createdDoorways.Add(possibleDoorway);
    }

    public void SetRoomNum(int roomNum) {
        this.roomNum = roomNum;

        IHasRoomNum[] hasRoomNumChildren = GetComponentsInChildren<IHasRoomNum>();
        foreach (IHasRoomNum hasRoomNum in hasRoomNumChildren) {
            hasRoomNum.SetRoomNum(roomNum);
        }

        // starting room
        if (roomNum == 1) {
            EnterRoom(gameObject);
        }
    }

    private void OnEnable() {
        enterTrigger.OnEnterContact += EnterRoom;

        roomCleared = noEnemies;
    }
    private void OnDisable() {
        exitTrigger.OnEnterContact -= ExitRoom;
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

        float hallwayLength = 8;
        Vector2 hallwayOffset = RoomGenerator.Instance.SideToDirection(connectingRoomDoorway.GetSide()) * hallwayLength;
        offset += hallwayOffset;
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

    #endregion

    public void CopyColliderToCameraConfiner(GameObject cameraConfinerComposite) {

        PolygonCollider2D targetCollider = cameraConfinerComposite.AddComponent<PolygonCollider2D>();

        targetCollider.usedByComposite = true;
        targetCollider.offset = cameraConfiner.offset;

        targetCollider.pathCount = cameraConfiner.pathCount;
        for (int i = 0; i < cameraConfiner.pathCount; i++) {
            Vector2[] path = cameraConfiner.GetPath(i);
            targetCollider.SetPath(i, path);
        }

        // Adjust for different positions if needed
        targetCollider.offset += (Vector2)(cameraConfiner.transform.position - cameraConfinerComposite.transform.position);
    }

    public void CreateEnterAndExitTriggers(PossibleDoorway doorway) {

        Vector2 colSize = new Vector2(3f, 3f);
        float offsetValue = 1f + (colSize.x / 2f);

        Vector2 offset = RoomGenerator.Instance.SideToDirection(doorway.GetSide()) * offsetValue;

        BoxCollider2D enterCol = enterTrigger.AddComponent<BoxCollider2D>();
        enterCol.usedByComposite = true;
        enterCol.size = colSize;
        enterCol.offset = (Vector2)doorway.transform.localPosition - offset;

        BoxCollider2D exitCol = exitTrigger.AddComponent<BoxCollider2D>();
        exitCol.usedByComposite = true;
        exitCol.size = colSize;
        exitCol.offset = (Vector2)doorway.transform.localPosition + offset;
    }

    private void EnterRoom(GameObject player) {

        enteredRoomNum = roomNum;

        if (!roomCleared) {
            CreateDoorwayBlockers();
        }

        exitTrigger.OnEnterContact += ExitRoom;
        Enemy.OnEnemiesCleared += SetRoomCleared;

        enterTrigger.OnEnterContact -= EnterRoom;

        OnAnyRoomEnter_Room?.Invoke(this);
    }

    private void ExitRoom(GameObject player) {
        //if (enteredRoomNum != roomNum) {
        //    return;
        //}

        enteredRoomNum = -1;

        enterTrigger.OnEnterContact += EnterRoom;

        exitTrigger.OnEnterContact -= ExitRoom;
        Enemy.OnEnemiesCleared -= SetRoomCleared;

        OnAnyRoomExit_Room?.Invoke(this);
    }

    private void CreateDoorwayBlockers() {
        foreach (PossibleDoorway createdDoorway in createdDoorways) {
            DoorBlocker newDoorBlocker = doorBlockerPrefab.Spawn(createdDoorway.transform.position, Containers.Instance.Rooms);
            newDoorBlocker.Setup(createdDoorway.GetSide());
        }
    }

    private void SetRoomCleared() {
        roomCleared = true;
    }
}
