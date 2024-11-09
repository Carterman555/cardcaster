#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomCreatorWindow : EditorWindow {

    private RoomTilemapCreator roomTilemapCreator;

    private Tilemap groundTilemap;
    private Tilemap topWallsTilemap;
    private Tilemap botWallsTilemap;

    private string tileSetName;

    [MenuItem("Tools/Room Creator")]
    public static void ShowWindow() {
        RoomCreatorWindow window = GetWindow<RoomCreatorWindow>("Room Creator");
        window.Initialize();
    }

    private void Initialize() {
    }

    private void OnGUI() {

        roomTilemapCreator = EditorGUILayout.ObjectField("Room Tilemap Creator", roomTilemapCreator, typeof(RoomTilemapCreator), true) as RoomTilemapCreator;

        groundTilemap = EditorGUILayout.ObjectField("Ground Tilemap", groundTilemap, typeof(Tilemap), true) as Tilemap;
        topWallsTilemap = EditorGUILayout.ObjectField("Top Walls Tilemap", topWallsTilemap, typeof(Tilemap), true) as Tilemap;
        botWallsTilemap = EditorGUILayout.ObjectField("Bottom Walls Tilemap", botWallsTilemap, typeof(Tilemap), true) as Tilemap;

        tileSetName = EditorGUILayout.TextField("Tile Set Name", tileSetName);

        if (GUILayout.Button("Create Wall Tiles")) {
            roomTilemapCreator.CreateRoomTiles(tileSetName, groundTilemap, topWallsTilemap, botWallsTilemap);

            RoomColliderMatcher roomColliderMatcher = new();
            roomColliderMatcher.SetupColliders();
        }

        if (GUILayout.Button("Clear Ground Tilemap")) {
            ClearTilemap(groundTilemap);
        }

        if (GUILayout.Button("Clear Wall Tilemaps")) {
            ClearTilemap(topWallsTilemap);
            ClearTilemap(botWallsTilemap);
        }
    }

    private void ClearTilemap(Tilemap tilemap) {
        for (int x = tilemap.cellBounds.min.x; x <= tilemap.cellBounds.max.x; x++) {
            for (int y = tilemap.cellBounds.min.y; y <= tilemap.cellBounds.max.y; y++) {
                Vector3Int position = new Vector3Int(x, y, 0);
                tilemap.SetTile(position, null);
            }
        }
    }
}
#endif