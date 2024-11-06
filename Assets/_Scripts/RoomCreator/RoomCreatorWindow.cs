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


        if (GUILayout.Button("Test")) {
            roomTilemapCreator.CreateRoomTiles(groundTilemap, topWallsTilemap, botWallsTilemap);
        }
    }
}
#endif