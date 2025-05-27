using AllIn1SpriteShader;
using DG.Tweening;
using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MinimapManager : StaticInstance<MinimapManager> {

    [SerializeField] private RectTransform miniMapIconContainer;
    [SerializeField] private Transform outlineIconContainer;

    [SerializeField] private Image roomHallMapIconPrefab;
    [SerializeField] private Image objectMapIconPrefab;

    [SerializeField] private float minimapScaleFactor;
    public float MinimapScaleFactor => minimapScaleFactor;

    private Dictionary<Transform, Image> roomAndHallwayIcons = new();
    public Dictionary<Image, Room> RoomIconDict = new();

    private bool spawnedIcons;

    private const float WORLD_TO_MINIMAP_SCALE = 100f;

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += SpawnAllRoomsAndHalls;

        roomAndHallwayIcons.Clear();
        spawnedIcons = false;
    }

    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= SpawnAllRoomsAndHalls;
    }

    private void SpawnAllRoomsAndHalls() {
        StartCoroutine(SpawnAllRoomsAndHallsCor());
    }
    private IEnumerator SpawnAllRoomsAndHallsCor() {
        // spawn hallways first to appear under rooms, so doesn't look weird when selecting room to teleport to
        Hallway[] hallways = FindObjectsOfType<Hallway>();
        foreach (Hallway hallway in hallways) {
            SpawnHallway(hallway);
        }

        Room[] rooms = FindObjectsOfType<Room>();
        foreach (Room room in rooms) {
            SpawnRoom(room);
        }

        miniMapIconContainer.GetComponent<All1CreateUnifiedOutline>().CreateUnifiedOutline();

        foreach (Transform outlineTransform in outlineIconContainer) {
            outlineTransform.GetComponent<Image>().Fade(0);
        }

        yield return null;

        spawnedIcons = true;
    }

    private void SpawnRoom(Room room) {

        Image roomIcon = roomHallMapIconPrefab.Spawn(miniMapIconContainer);
        roomIcon.sprite = room.ScriptableRoom.MapIcon;
        RectTransform roomIconTransform = roomIcon.GetComponent<RectTransform>();
        roomIconTransform.anchoredPosition = WorldToIconPos(GetTileMapCenterPos(room.transform));

        roomIcon.SetNativeSize();
        roomIconTransform.sizeDelta = roomIconTransform.sizeDelta * minimapScaleFactor;

        roomIcon.name = $"RoomIcon{roomAndHallwayIcons.Count + 1}";

        roomIcon.Fade(0f);

        roomAndHallwayIcons.Add(room.transform, roomIcon);
        RoomIconDict.Add(roomIcon, room);
    }

    private void SpawnHallway(Hallway hallway) {
        Image hallIcon = roomHallMapIconPrefab.Spawn(miniMapIconContainer);
        hallIcon.sprite = hallway.MapIcon;
        RectTransform hallIconTransform = hallIcon.GetComponent<RectTransform>();
        hallIconTransform.anchoredPosition = WorldToIconPos(GetTileMapCenterPos(hallway.transform));

        hallIcon.SetNativeSize();
        hallIconTransform.sizeDelta = hallIconTransform.sizeDelta * minimapScaleFactor;

        hallIcon.name = $"HallIcon{roomAndHallwayIcons.Count + 1}";

        hallIcon.Fade(0f);

        roomAndHallwayIcons.Add(hallway.transform, hallIcon);
    }

    private void Update() {
        miniMapIconContainer.anchoredPosition = -WorldToIconPos(PlayerMovement.Instance.CenterPos);
    }

    public IEnumerator ShowRoomOrHallIcon(Transform roomOrHallway) {

        int count = 0;
        int maxCount = 60;
        float tryShowDuration = 1f; // in seconds
        while (!spawnedIcons) {
            yield return new WaitForSeconds(tryShowDuration / maxCount);

            count++;
            if (count > maxCount) {
                Debug.LogError("Tried to show hallway or room but icons haven't been spawned! - " + roomOrHallway.name);
                yield break;
            }
        }

        if (!roomAndHallwayIcons.ContainsKey(roomOrHallway)) {
            Debug.LogError("Tried to show hallway or room that hasn't been spawned! - " + roomOrHallway.name);
            yield break;
        }

        roomAndHallwayIcons[roomOrHallway].DOFade(1f, duration: 0.5f);

        string outlineName = roomAndHallwayIcons[roomOrHallway].name + "Outline";
        Transform outline = outlineIconContainer.Find(outlineName);
        if (outline == null) {
            Debug.LogError("Could not find outline for room or hall icon!");
            yield break;
        }
        outline.GetComponent<Image>().DOFade(1f, duration: 0.5f);
    }

    [Command]
    private void ShowAllMapIcons() {
        foreach (Transform roomOrHallway in roomAndHallwayIcons.Keys) {
            StartCoroutine(ShowRoomOrHallIcon(roomOrHallway));
        }
    }

    public Image SpawnObjectIcon(Sprite sprite, Vector2 worldPos, Vector2 size) {
        Image objectIcon = objectMapIconPrefab.Spawn(miniMapIconContainer);
        objectIcon.sprite = sprite;

        RectTransform objectIconTransform = objectIcon.GetComponent<RectTransform>();
        objectIconTransform.anchoredPosition = WorldToIconPos(worldPos);
        objectIconTransform.sizeDelta = size;

        return objectIcon;
    }

    private Vector2 WorldToIconPos(Vector2 worldPos) {
        return worldPos * minimapScaleFactor * WORLD_TO_MINIMAP_SCALE;
    }

    private Vector2 GetTileMapCenterPos(Transform tilemapParent) {

        Tilemap[] tilemaps = tilemapParent.GetComponentsInChildren<Tilemap>();

        Vector3Int min = Vector3Int.one * int.MaxValue;
        Vector3Int max = Vector3Int.one * int.MinValue;
        bool foundTile = false;

        foreach (var tilemap in tilemaps) {
            BoundsInt bounds = tilemap.cellBounds;
            foreach (var pos in bounds.allPositionsWithin) {
                if (tilemap.HasTile(pos)) {
                    min = Vector3Int.Min(min, pos);
                    max = Vector3Int.Max(max, pos);
                    foundTile = true;
                }
            }
        }

        if (foundTile) {
            //... Cell To world gets the bottom left corner of cell, but need top right of 
            //... max pos cell to get correct avg
            max.x++;
            max.y++;
        
            Vector3Int centerCell = (min + max) / 2;
            return tilemaps[0].CellToWorld(centerCell);
        }
        else {
            Debug.LogError("Tried finding center, but tilemaps don't have any tiles!");
            return Vector2.zero;
        }
    }
}
