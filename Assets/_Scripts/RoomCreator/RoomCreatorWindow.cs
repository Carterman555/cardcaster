#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomCreatorWindow : EditorWindow {

    private Tilemap groundTilemap;
    private Tilemap topWallsTilemap;
    private Tilemap botWallsTilemap;

    private RoomTilemapCreator roomTilemapCreator;
    private string tileSetName;

    private PolygonCollider2D roomCollider;
    private PolygonCollider2D camConfinerCollider;

    [MenuItem("Tools/Room Creator")]
    public static void ShowWindow() {
        RoomCreatorWindow window = GetWindow<RoomCreatorWindow>("Room Creator");
        window.Initialize();
    }

    private void Initialize() {
    }

    private void OnGUI() {

        groundTilemap = EditorGUILayout.ObjectField("Ground Tilemap", groundTilemap, typeof(Tilemap), true) as Tilemap;
        topWallsTilemap = EditorGUILayout.ObjectField("Top Walls Tilemap", topWallsTilemap, typeof(Tilemap), true) as Tilemap;
        botWallsTilemap = EditorGUILayout.ObjectField("Bottom Walls Tilemap", botWallsTilemap, typeof(Tilemap), true) as Tilemap;

        GUILayout.Space(5);
        var headerRect = GUILayoutUtility.GetRect(0, height: 30, GUILayout.ExpandWidth(true));
        EditorGUI.LabelField(headerRect, "Tilemap Creator");
        GUILayout.Space(5);

        roomTilemapCreator = EditorGUILayout.ObjectField("Room Tilemap Creator", roomTilemapCreator, typeof(RoomTilemapCreator), true) as RoomTilemapCreator;
        tileSetName = EditorGUILayout.TextField("Tile Set Name", tileSetName);

        if (GUILayout.Button("Create Wall Tiles")) {
            roomTilemapCreator.CreateRoomTiles(tileSetName, groundTilemap, topWallsTilemap, botWallsTilemap);
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Clear Ground Tilemap")) {
            ClearTilemap(groundTilemap);
        }

        if (GUILayout.Button("Clear Wall Tilemaps")) {
            ClearTilemap(topWallsTilemap);
            ClearTilemap(botWallsTilemap);
        }

        GUILayout.Space(5);
        headerRect = GUILayoutUtility.GetRect(0, height: 30, GUILayout.ExpandWidth(true));
        EditorGUI.LabelField(headerRect, "Collider Setup");
        GUILayout.Space(5);

        roomCollider = EditorGUILayout.ObjectField("Room Collider", roomCollider, typeof(PolygonCollider2D), true) as PolygonCollider2D;
        camConfinerCollider = EditorGUILayout.ObjectField("Camera Confiner Collider", camConfinerCollider, typeof(PolygonCollider2D), true) as PolygonCollider2D;

        if (GUILayout.Button("Setup Polygon Colliders")) {
            RoomColliderMatcher roomColliderMatcher = new(roomCollider, camConfinerCollider, topWallsTilemap, botWallsTilemap);
            roomColliderMatcher.SetupRoomCollider();
            roomColliderMatcher.SetupCamConfinerCollider();
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