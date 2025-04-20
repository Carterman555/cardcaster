using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour {

    [SerializeField] private RectTransform miniMapIcons;
    [SerializeField] private Image mapIconPrefab;

    [SerializeField] private float minimapScaleFactor;

    private const float WORLD_TO_MINIMAP_SCALE = 100f;

    private void OnEnable() {
        Room.OnAnyRoomEnter_Room += OnEnterRoom;

        RoomGenerator.OnCompleteGeneration += SpawnAllRooms;
    }

    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= OnEnterRoom;

        RoomGenerator.OnCompleteGeneration -= SpawnAllRooms;
    }

    private void OnEnterRoom(Room room) {

        Image roomIcon = mapIconPrefab.Spawn(miniMapIcons);
        roomIcon.sprite = room.GetScriptableRoom().MapIcon;
        RectTransform roomIconTransform = roomIcon.GetComponent<RectTransform>();
        roomIconTransform.anchoredPosition = WorldToIconPos(GetRoomTileMapCenterPos(room));

        roomIcon.SetNativeSize();
        roomIconTransform.sizeDelta = roomIconTransform.sizeDelta * minimapScaleFactor;
    }

    private Vector2 WorldToIconPos(Vector2 worldPos) {
        return worldPos * minimapScaleFactor * WORLD_TO_MINIMAP_SCALE;
    }

    // debugging
    private void SpawnAllRooms() {
        Room[] rooms = FindObjectsOfType<Room>();
        foreach (Room room in rooms) {
            OnEnterRoom(room);
        }
    }

    private Vector2 GetRoomTileMapCenterPos(Room room) {

        Tilemap[] tilemaps = new Tilemap[] {
            room.GetBotWallsTilemap(),
            room.GetTopWallsTilemap(),
            room.GetGroundTilemap()
        };

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
