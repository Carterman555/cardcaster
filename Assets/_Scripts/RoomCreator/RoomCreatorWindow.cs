#if UNITY_EDITOR
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using System;

public class RoomCreatorWindow : EditorWindow {

    private GameObject mainRoom;

    private Tilemap groundTilemap;
    private Tilemap topWallsTilemap;
    private Tilemap botWallsTilemap;

    private RoomTilemapCreator roomTilemapCreator;
    private EnvironmentType environmentType;

    private PolygonCollider2D roomCollider;
    private PolygonCollider2D camConfinerCollider;

    private Light2D roomLight;

    // setup multiple lights
    private ScriptableRoomColliders scriptableRoomColliders;

    // setup multiple minimap sprites
    private ScriptableRooms scriptableRooms;

    [MenuItem("Tools/Room Creator")]
    public static void ShowWindow() {
        GetWindow<RoomCreatorWindow>("Room Creator");
    }

    private void OnGUI() {

        mainRoom = EditorGUILayout.ObjectField("Room", mainRoom, typeof(GameObject), true) as GameObject;

        GUILayout.Space(5);
        var headerRect = GUILayoutUtility.GetRect(0, height: 30, GUILayout.ExpandWidth(true));
        EditorGUI.LabelField(headerRect, "Tilemap Creator");
        GUILayout.Space(5);

        environmentType = (EnvironmentType)EditorGUILayout.EnumPopup("Environment Type", environmentType);

        if (ConditionalButton("Create Wall Tiles", mainRoom != null)) {

            SetTilemaps();

            Undo.RecordObjects(new UnityEngine.Object[] { groundTilemap, topWallsTilemap, botWallsTilemap }, "Create Wall Tiles");

            roomTilemapCreator = Resources.Load<RoomTilemapCreator>("RoomTilemapCreator");
            roomTilemapCreator.CreateRoomTiles(environmentType, groundTilemap, topWallsTilemap, botWallsTilemap);
        }

        GUILayout.Space(5);

        if (ConditionalButton("Clear Ground Tilemap", mainRoom != null)) {
            SetTilemaps();

            Undo.RecordObject(groundTilemap, "Clear Ground Tilemap");

            ClearTilemap(groundTilemap);
        }

        if (ConditionalButton("Clear Wall Tilemaps", mainRoom != null)) {
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

        if (ConditionalButton("Setup Polygon Colliders", mainRoom != null && roomCollider != null && camConfinerCollider != null)) {
            SetTilemaps();

            // Start undo recording for this object
            Undo.RecordObject(roomCollider, "Setup Room Collider");
            Undo.RecordObject(camConfinerCollider, "Setup Cam Confiner Collider");

            RoomColliderMatcher roomColliderMatcher = new(roomCollider, camConfinerCollider, topWallsTilemap, botWallsTilemap);
            roomColliderMatcher.SetupRoomCollider();
            roomColliderMatcher.SetupCamConfinerCollider();
        }

        roomLight = EditorGUILayout.ObjectField("Room Light", roomLight, typeof(Light2D), true) as Light2D;

        if (ConditionalButton("Setup Light Shape", roomCollider != null && roomLight != null)) {
            Undo.RecordObject(roomLight, "Setup Light Shape");

            RoomLightShapeMatcher roomLightShapeMatcher = new();
            roomLightShapeMatcher.MatchLightShape(roomLight, roomCollider);
        }

        scriptableRoomColliders = EditorGUILayout.ObjectField("Room Colliders", scriptableRoomColliders, typeof(ScriptableRoomColliders), true) as ScriptableRoomColliders;

        if (ConditionalButton("Setup All Lights", scriptableRoomColliders != null)) {
            RoomLightShapeMatcher roomLightShapeMatcher = new();

            foreach (var roomCollider in scriptableRoomColliders.Colliders) {
                Light2D currentRoomLight = roomCollider.GetComponentInChildren<Light2D>();
                roomLightShapeMatcher.MatchLightShape(currentRoomLight, roomCollider);
            }
        }

        GUILayout.Space(5);
        headerRect = GUILayoutUtility.GetRect(0, height: 30, GUILayout.ExpandWidth(true));
        EditorGUI.LabelField(headerRect, "Minimap Sprite");
        GUILayout.Space(5);

        scriptableRooms = EditorGUILayout.ObjectField("Rooms", scriptableRooms, typeof(ScriptableRooms), true) as ScriptableRooms;

        if (ConditionalButton("Create Minimap Sprite", mainRoom != null)) {
            SetupMinimapSprite(mainRoom);
        }

        if (ConditionalButton("Create All Minimap Sprites", scriptableRooms != null)) {
            foreach (GameObject room in scriptableRooms.Rooms) {
                SetupMinimapSprite(room);
            }
        }
    }

    private bool ConditionalButton(string text, bool activeCondition) {

        if (activeCondition) {
            return GUILayout.Button(text);
        }
        else {
            GUI.enabled = false;
            GUILayout.Button(text);
            GUI.enabled = true;

            return false;
        }
    }

    #region Minimap Icon

    private void SetupMinimapSprite(GameObject room) {

        Undo.RecordObject(room, "Setup Minimap Sprite");

        // Create sprite
        SetTilemaps(room);

        Tilemap[] tilemaps = new Tilemap[] { groundTilemap, topWallsTilemap, botWallsTilemap };

        string fileName = room.name + "-MinimapIcon";

        RoomMapSpriteCreator roomMiniMapSpriteCreator = new RoomMapSpriteCreator();
        roomMiniMapSpriteCreator.CreateMiniMapSprite(fileName, tilemaps);

        // Set sprite
        string miniMapIconName = "MinimapIcon";
        Transform miniMapIconTransform = room.transform.Find(miniMapIconName);
        if (miniMapIconTransform == null) Debug.LogError("Could not find MinimapIcon by name!");

        string filePath = roomMiniMapSpriteCreator.GetFilePath();
        Sprite miniMapSprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
        miniMapIconTransform.GetComponent<SpriteRenderer>().sprite = miniMapSprite;

        // Position sprite - broken rn
        //Vector2 tileMapsCenter = roomMiniMapSpriteCreator.GetTileMapsCenter();
        //miniMapIconTransform.position = tileMapsCenter;

        EditorUtility.SetDirty(room);
    }

    #endregion

    private void SetTilemaps(GameObject room = null) {

        GameObject roomToUse = room != null ? room : mainRoom;

        string gridName = "Grid";
        Grid grid = roomToUse.transform.Find(gridName).GetComponent<Grid>();
        if (grid == null) Debug.LogError("Could not find grid by name!");

        string groundTilemapName = "GroundTilemap";
        string topWallsTilemapName = "TopWallsTilemap";
        string botWallsTilemapName = "BotWallsTilemap";

        groundTilemap = grid.transform.Find(groundTilemapName).GetComponent<Tilemap>();
        if (groundTilemap == null) Debug.LogError("Could not find ground tilemap by name!");

        topWallsTilemap = grid.transform.Find(topWallsTilemapName).GetComponent<Tilemap>();
        if (topWallsTilemap == null) Debug.LogError("Could not find top walls tilemap by name!");

        botWallsTilemap = grid.transform.Find(botWallsTilemapName).GetComponent<Tilemap>();
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