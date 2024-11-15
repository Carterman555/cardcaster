#if UNITY_EDITOR
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomCreatorWindow : EditorWindow {

    private Grid tilemapGrid;

    private Tilemap groundTilemap;
    private Tilemap topWallsTilemap;
    private Tilemap botWallsTilemap;

    private RoomTilemapCreator roomTilemapCreator;
    private EnvironmentType environmentType;

    private PolygonCollider2D roomCollider;
    private PolygonCollider2D camConfinerCollider;

    [MenuItem("Tools/Room Creator")]
    public static void ShowWindow() {
        GetWindow<RoomCreatorWindow>("Room Creator");
    }

    private void OnGUI() {

        tilemapGrid = EditorGUILayout.ObjectField("Tilemap Grid", tilemapGrid, typeof(Grid), true) as Grid;

        GUILayout.Space(5);
        var headerRect = GUILayoutUtility.GetRect(0, height: 30, GUILayout.ExpandWidth(true));
        EditorGUI.LabelField(headerRect, "Tilemap Creator");
        GUILayout.Space(5);

        environmentType = (EnvironmentType)EditorGUILayout.EnumPopup("Environment Type", environmentType);

        if (GUILayout.Button("Create Wall Tiles")) {

            SetTilemaps();

            Undo.RecordObjects(new Object[] { groundTilemap, topWallsTilemap, botWallsTilemap }, "Create Wall Tiles");

            roomTilemapCreator = Resources.Load<RoomTilemapCreator>("RoomTilemapCreator");
            roomTilemapCreator.CreateRoomTiles(environmentType, groundTilemap, topWallsTilemap, botWallsTilemap);
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Clear Ground Tilemap")) {
            SetTilemaps();

            Undo.RecordObject(groundTilemap, "Clear Ground Tilemap");

            ClearTilemap(groundTilemap);
        }

        if (GUILayout.Button("Clear Wall Tilemaps")) {
            SetTilemaps();

            Undo.RecordObject(topWallsTilemap, "Clear Top Wall Tilemaps");
            Undo.RecordObject(botWallsTilemap, "Clear Bot Wall Tilemaps");

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
            SetTilemaps();

            // Start undo recording for this object
            Undo.RecordObject(roomCollider, "Setup Room Collider");
            Undo.RecordObject(camConfinerCollider, "Setup Cam Confiner Collider");

            RoomColliderMatcher roomColliderMatcher = new(roomCollider, camConfinerCollider, topWallsTilemap, botWallsTilemap);
            roomColliderMatcher.SetupRoomCollider();
            roomColliderMatcher.SetupCamConfinerCollider();
        }

        
    }

    private void SetTilemaps() {

        string groundTilemapName = "GroundTilemap";
        string topWallsTilemapName = "TopWallsTilemap";
        string botWallsTilemapName = "BotWallsTilemap";

        groundTilemap = tilemapGrid.transform.Find(groundTilemapName).GetComponent<Tilemap>();
        if (groundTilemap == null) Debug.LogError("Could not find ground tilemap by name!");

        topWallsTilemap = tilemapGrid.transform.Find(topWallsTilemapName).GetComponent<Tilemap>();
        if (topWallsTilemap == null) Debug.LogError("Could not find top walls tilemap by name!");

        botWallsTilemap = tilemapGrid.transform.Find(botWallsTilemapName).GetComponent<Tilemap>();
        if (botWallsTilemap == null) Debug.LogError("Could not find bottom walls tilemap by name!");
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