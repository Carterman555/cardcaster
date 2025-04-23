#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public class RoomCreatorWindow : EditorWindow {

    private GameObject mainRoom;

    private Tilemap groundTilemap;
    private Tilemap topWallsTilemap;
    private Tilemap botWallsTilemap;

    private RoomTilemapCreator roomTilemapCreator;
    private EnvironmentType environmentType;

    // objects
    private GameObject torchPrefab;
    private GameObject torch1;
    private GameObject torch2;
    private float approxSpacing;

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
        EditorGUI.LabelField(headerRect, "Setup");
        GUILayout.Space(5);

        if (ConditionalButton("Setup Polygon Colliders", mainRoom != null)) {
            SetTilemaps();

            PolygonCollider2D roomCollider = mainRoom.GetComponent<PolygonCollider2D>();
            Transform camConfiner = mainRoom.transform.Find("CameraConfiner");
            if (camConfiner == null) {
                Debug.LogError("Could not find object with name 'CameraConfiner'!");
                return;
            }
            PolygonCollider2D camConfinerCollider = camConfiner.GetComponent<PolygonCollider2D>();

            // Start undo recording for this object
            Undo.RecordObject(roomCollider, "Setup Room Collider");
            Undo.RecordObject(camConfinerCollider, "Setup Cam Confiner Collider");

            RoomColliderMatcher roomColliderMatcher = new(roomCollider, camConfinerCollider, topWallsTilemap, botWallsTilemap);
            roomColliderMatcher.SetupRoomCollider();
            roomColliderMatcher.SetupCamConfinerCollider();
        }

        if (ConditionalButton("Setup Light Shape", mainRoom != null)) {

            PolygonCollider2D roomCollider = mainRoom.GetComponent<PolygonCollider2D>();

            Transform roomLightTransform = mainRoom.transform.Find("RoomLight");
            if (roomLightTransform == null) {
                Debug.LogError("Could not find object with name 'RoomLight'!");
                return;
            }
            Light2D roomLight = roomLightTransform.GetComponent<Light2D>();

            Undo.RecordObject(roomLight, "Setup Light Shape");

            RoomLightShapeMatcher roomLightShapeMatcher = new();
            roomLightShapeMatcher.MatchLightShape(roomLight, roomCollider);
        }

        if (ConditionalButton("Create Minimap Sprite", mainRoom != null)) {
            SetupMinimapSprite();
        }

        GUILayout.Space(5);
        headerRect = GUILayoutUtility.GetRect(0, height: 30, GUILayout.ExpandWidth(true));
        EditorGUI.LabelField(headerRect, "Objects");
        GUILayout.Space(5);

        torchPrefab = EditorGUILayout.ObjectField("Torch Prefab", torchPrefab, typeof(GameObject), true) as GameObject;
        torch1 = EditorGUILayout.ObjectField("Torch 1", torch1, typeof(GameObject), true) as GameObject;
        torch2 = EditorGUILayout.ObjectField("Torch 2", torch2, typeof(GameObject), true) as GameObject;
        approxSpacing = EditorGUILayout.FloatField("Approx Spacing", approxSpacing);

        if (ConditionalButton("Create Torches", mainRoom != null && torchPrefab != null && torch1 != null && torch2 != null)) {

            Undo.RecordObject(mainRoom, "Create Torches");

            Vector2 startPos = torch1.transform.position;
            Vector2 endPos = torch2.transform.position;

            float distance = Vector2.Distance(startPos, endPos);

            int amount = Mathf.RoundToInt(distance / approxSpacing);
            float actualSpacing = distance / amount;

            Vector2 currentPos = startPos;
            for (int i = 0; i < amount - 1; i++) {
                currentPos = Vector2.MoveTowards(currentPos, endPos, actualSpacing);
                GameObject newTorch = (GameObject)PrefabUtility.InstantiatePrefab(torchPrefab);
                newTorch.transform.SetPositionAndRotation(currentPos, Quaternion.identity);
                newTorch.transform.SetParent(mainRoom.transform);
            }
        }

        GUILayout.Space(5);
        headerRect = GUILayoutUtility.GetRect(0, height: 30, GUILayout.ExpandWidth(true));
        EditorGUI.LabelField(headerRect, "Multiple Room Setup");
        GUILayout.Space(5);

        scriptableRoomColliders = EditorGUILayout.ObjectField("Room Colliders", scriptableRoomColliders, typeof(ScriptableRoomColliders), true) as ScriptableRoomColliders;

        if (ConditionalButton("Setup All Lights", scriptableRoomColliders != null)) {
            RoomLightShapeMatcher roomLightShapeMatcher = new();

            foreach (var roomCollider in scriptableRoomColliders.Colliders) {
                Light2D currentRoomLight = roomCollider.GetComponentInChildren<Light2D>();
                roomLightShapeMatcher.MatchLightShape(currentRoomLight, roomCollider);
            }
        }

        scriptableRooms = EditorGUILayout.ObjectField("Rooms", scriptableRooms, typeof(ScriptableRooms), true) as ScriptableRooms;

        // just in case I want to setup or remake multiple rooms automatically
        //if (ConditionalButton("Example setup multiple rooms", scriptableRooms != null)) {
        //    foreach (GameObject room in scriptableRooms.Rooms) {
        //        ExampleSetupRoom(room);
        //    }
        //}
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

    private void SetupMinimapSprite() {

        if (mainRoom.GetComponent<Room>().ScriptableRoom == null) {
            Debug.LogError("Scriptable Room is not set!");
            return;
        }

        Undo.RecordObject(mainRoom, "Setup Minimap Sprite");

        // Create sprite
        SetTilemaps(mainRoom);

        Tilemap[] tilemaps = new Tilemap[] { groundTilemap, topWallsTilemap, botWallsTilemap };

        string fileName = mainRoom.name + "-MinimapIcon";

        RoomMapSpriteCreator roomMiniMapSpriteCreator = new RoomMapSpriteCreator();
        roomMiniMapSpriteCreator.CreateMiniMapSprite(fileName, tilemaps);

        // Set sprite
        string filePath = roomMiniMapSpriteCreator.GetFilePath();
        Sprite miniMapSprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
        mainRoom.GetComponent<Room>().ScriptableRoom.MapIcon = miniMapSprite;
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