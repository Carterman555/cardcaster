using AllIn1SpriteShader;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MinimapManager : StaticInstance<MinimapManager> {

    [SerializeField] private RectTransform miniMapIcons;
    [SerializeField] private Image mapIconPrefab;

    [SerializeField] private float minimapScaleFactor;

    private Dictionary<Transform, Image> roomAndHallwayIcons = new();

    private bool spawnedIcons;

    private const float WORLD_TO_MINIMAP_SCALE = 100f;

    private void OnEnable() {
        Room.OnAnyRoomEnter_Room += SpawnRoom;
        RoomGenerator.OnCompleteGeneration += SpawnAllRoomsAndHalls;

        roomAndHallwayIcons.Clear();
        spawnedIcons = false;
    }

    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= SpawnRoom;
        RoomGenerator.OnCompleteGeneration -= SpawnAllRoomsAndHalls;
    }

    private void SpawnAllRoomsAndHalls() {
        StartCoroutine(SpawnAllRoomsAndHallsCor());
    }
    private IEnumerator SpawnAllRoomsAndHallsCor() {

        Room[] rooms = FindObjectsOfType<Room>();
        foreach (Room room in rooms) {
            SpawnRoom(room);
        }

        Hallway[] hallways = FindObjectsOfType<Hallway>();
        foreach (Hallway hallway in hallways) {
            SpawnHallway(hallway);
        }

        miniMapIcons.GetComponent<All1CreateUnifiedOutline>().CreateUnifiedOutline();

        yield return null;

        spawnedIcons = true;
    }

    private void SpawnRoom(Room room) {
        Image roomIcon = mapIconPrefab.Spawn(miniMapIcons);
        roomIcon.sprite = room.GetScriptableRoom().MapIcon;
        RectTransform roomIconTransform = roomIcon.GetComponent<RectTransform>();
        roomIconTransform.anchoredPosition = WorldToIconPos(GetTileMapCenterPos(room.transform));

        roomIcon.SetNativeSize();
        roomIconTransform.sizeDelta = roomIconTransform.sizeDelta * minimapScaleFactor;

        roomIcon.Fade(0f);

        roomAndHallwayIcons.Add(room.transform, roomIcon);
        print("added: " + roomIconTransform.name);
    }

    private void SpawnHallway(Hallway hallway) {
        Image hallIcon = mapIconPrefab.Spawn(miniMapIcons);
        hallIcon.sprite = hallway.MapIcon;
        RectTransform hallIconTransform = hallIcon.GetComponent<RectTransform>();
        hallIconTransform.anchoredPosition = WorldToIconPos(GetTileMapCenterPos(hallway.transform));

        hallIcon.SetNativeSize();
        hallIconTransform.sizeDelta = hallIconTransform.sizeDelta * minimapScaleFactor;

        hallIcon.Fade(0f);

        roomAndHallwayIcons.Add(hallway.transform, hallIcon);
        print("added: " + hallIconTransform.name);
    }

    private void Update() {
        miniMapIcons.anchoredPosition = -WorldToIconPos(PlayerMovement.Instance.CenterPos);
    }

    public IEnumerator ShowMapIcon(Transform roomOrHallway) {

        while (!spawnedIcons) {
            yield return null;
        }

        if (!roomAndHallwayIcons.ContainsKey(roomOrHallway)) {
            Debug.LogError("Tried to show hallway or room that hasn't been spawned! - " + roomOrHallway.name);
            yield break;
        }

        roomAndHallwayIcons[roomOrHallway].DOFade(1f, duration: 0.5f);
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
