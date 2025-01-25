using DG.Tweening;
using QFSW.QC;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour {

    public static event Action<Room> OnAnyRoomEnter_Room;
    public static event Action<Room> OnAnyRoomExit_Room;

    private static Room currentRoom;
    private static int currentRoomNum;

    [SerializeField] private ScriptableRoom scriptableRoom;

    private int roomNum;

    private List<PossibleDoorway> possibleDoorways;
    private List<PossibleDoorway> createdDoorways = new();

    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap botColliderTilemap;
    [SerializeField] private Tilemap colliderTilemap;

    [SerializeField] private PolygonCollider2D cameraConfiner;

    [SerializeField] private TriggerContactTracker enterTrigger;
    [SerializeField] private TriggerContactTracker exitTrigger;
    [SerializeField] private DoorBlocker doorBlockerPrefab;
    [SerializeField] private DoorBlocker sideDoorBlockerPrefab;

    private bool roomCleared;

    [SerializeField] private Light2D roomLight;
    [SerializeField] private SpriteRenderer mapIconToSpawn;
    private SpriteRenderer mapIcon;

    #region Get Methods

    public static Room GetCurrentRoom() {
        return currentRoom;
    }

    public ScriptableRoom GetScriptableRoom() {
        return scriptableRoom;
    }

    public List<PossibleDoorway> GetPossibleDoorways() {

        //... assign it if it's null
        possibleDoorways ??= transform.GetComponentsInChildren<PossibleDoorway>().ToList();

        return possibleDoorways;
    }

    public PossibleDoorway GetPossibleDoorway(string name) {

        if (!GetPossibleDoorways().Any(d => d.name == name)){
            Debug.LogError("Could Not Find Doorway With Name: " + name);
            return null;
        }

        return GetPossibleDoorways().Where(d => d.name == name).FirstOrDefault();
    }

    public Tilemap GetGroundTilemap() {
        return groundTilemap;
    }

    public Tilemap GetBotWallsTilemap() {
        return botColliderTilemap;
    }

    public Tilemap GetTopWallsTilemap() {
        return colliderTilemap;
    }

    public int GetRoomNum() {
        return roomNum;
    }

    public bool IsRoomCleared() {
        return roomCleared;
    }

    #endregion

    public void SetRoomCleared() {
        roomCleared = true;
    }

    public void AddCreatedDoorway(PossibleDoorway possibleDoorway) {
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
            RoomGenerator.OnCompleteGeneration += OnEnterRoom;
        }

        mapIcon.name = "RoomMapIcon" + roomNum;
    }

    private void OnEnable() {
        enterTrigger.OnEnterContact += OnEnterRoom;

        roomCleared = scriptableRoom.NoEnemies;
        roomLight.intensity = 0;

        SetupMapIcon();
        CopyColliderToCameraConfiner();
    }
    private void OnDisable() {
        exitTrigger.OnEnterContact -= OnEnterRoom;

        Destroy(mapIcon.gameObject);
    }

    // spawn the map icon as a child of LevelMapIcons so LevelMapIcons can create a unified outline around all the
    // rooms and hallways
    private void SetupMapIcon() {
        mapIcon = mapIconToSpawn.Spawn(mapIconToSpawn.transform.position, Containers.Instance.RoomMapIcons);
        mapIconToSpawn.enabled = false;
    }

    public void CopyColliderToCameraConfiner() {

        GameObject cameraConfinerComposite = ReferenceSystem.Instance.CameraConfiner;
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

    public void OnEnterRoom() {

        currentRoomNum = roomNum;
        currentRoom = this;

        if (!roomCleared) {
            CreateDoorwayBlockers();
        }

        //... brighten room
        DOTween.To(() => roomLight.intensity, x => roomLight.intensity = x, 1, duration: 1f);

        //... show room on minimap
        LevelMapIcons.Instance.ShowMapIcon(mapIcon);

        exitTrigger.OnEnterContact += OnExitRoom;

        CheckEnemiesCleared.OnEnemiesCleared += SetRoomCleared;
        BossManager.OnBossKilled += SetRoomCleared;

        RoomGenerator.OnCompleteGeneration -= OnEnterRoom;
        enterTrigger.OnEnterContact -= OnEnterRoom;

        OnAnyRoomEnter_Room?.Invoke(this);
    }

    private void OnExitRoom() {
        currentRoomNum = -1;
        currentRoom = null;

        enterTrigger.OnEnterContact += OnEnterRoom;

        exitTrigger.OnEnterContact -= OnExitRoom;

        CheckEnemiesCleared.OnEnemiesCleared -= SetRoomCleared;
        BossManager.OnBossKilled -= SetRoomCleared;

        OnAnyRoomExit_Room?.Invoke(this);
    }

    private void CreateDoorwayBlockers() {

        foreach (PossibleDoorway createdDoorway in createdDoorways) {
            bool sideBlocker = createdDoorway.GetSide() == DoorwaySide.Left || createdDoorway.GetSide() == DoorwaySide.Right;

            if (sideBlocker) {
                DoorBlocker newDoorBlocker = sideDoorBlockerPrefab.Spawn(createdDoorway.transform.position, Containers.Instance.Rooms);
                newDoorBlocker.Setup(createdDoorway.GetSide());
            }
            else {
                DoorBlocker newDoorBlocker = doorBlockerPrefab.Spawn(createdDoorway.transform.position, Containers.Instance.Rooms);
                newDoorBlocker.Setup(createdDoorway.GetSide());
            }
        }
    }

    
}
