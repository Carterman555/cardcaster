#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// to find which prefabs and scene object are using a certain script
/// </summary>
public class PrefabScriptFinder : EditorWindow {
    private MonoScript targetScript;
    private Vector2 scrollPosition;
    private List<GameObject> foundPrefabs = new List<GameObject>();
    private List<GameObject> foundSceneObjects = new List<GameObject>();
    private bool isSearching = false;
    private bool searchPrefabs = true;
    private bool searchSceneObjects = true;
    private bool showPrefabResults = true;
    private bool showSceneResults = true;

    [MenuItem("Tools/Prefab Script Finder")]
    public static void ShowWindow() {
        GetWindow<PrefabScriptFinder>("Script Finder");
    }

    private void OnGUI() {
        GUILayout.Label("Find Objects Using Script", EditorStyles.boldLabel);

        // Script selection field
        targetScript = (MonoScript)EditorGUILayout.ObjectField(
            "Target Script:",
            targetScript,
            typeof(MonoScript),
            false
        );

        EditorGUILayout.Space();

        // Search options
        EditorGUILayout.BeginHorizontal();
        searchPrefabs = EditorGUILayout.ToggleLeft("Search Prefabs", searchPrefabs, GUILayout.Width(120));
        searchSceneObjects = EditorGUILayout.ToggleLeft("Search Scene Objects", searchSceneObjects);
        EditorGUILayout.EndHorizontal();

        // Search button
        if (GUILayout.Button("Search") && targetScript != null && (searchPrefabs || searchSceneObjects)) {
            isSearching = true;
            foundPrefabs.Clear();
            foundSceneObjects.Clear();

            if (searchPrefabs)
                FindPrefabsWithScript();

            if (searchSceneObjects)
                FindSceneObjectsWithScript();

            isSearching = false;
        }

        EditorGUILayout.Space();

        // Show results in a scrollview
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Prefab Results
        if (foundPrefabs.Count > 0) {
            showPrefabResults = EditorGUILayout.Foldout(showPrefabResults, $"Prefabs Found ({foundPrefabs.Count})", true);
            if (showPrefabResults) {
                EditorGUI.indentLevel++;
                foreach (GameObject prefab in foundPrefabs) {
                    DrawObjectEntry(prefab, true);
                }
                EditorGUI.indentLevel--;
            }
        }

        // Scene Object Results
        if (foundSceneObjects.Count > 0) {
            showSceneResults = EditorGUILayout.Foldout(showSceneResults, $"Scene Objects Found ({foundSceneObjects.Count})", true);
            if (showSceneResults) {
                EditorGUI.indentLevel++;
                foreach (GameObject sceneObj in foundSceneObjects) {
                    DrawObjectEntry(sceneObj, false);
                }
                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.EndScrollView();

        // Show message if no results found
        if (!isSearching && targetScript != null && foundPrefabs.Count == 0 && foundSceneObjects.Count == 0) {
            EditorGUILayout.HelpBox("No objects found with this script.", MessageType.Info);
        }
    }

    private void DrawObjectEntry(GameObject obj, bool isPrefab) {
        if (obj == null) return;

        EditorGUILayout.BeginHorizontal();

        // Object name and hierarchy path
        string displayName = isPrefab ? obj.name : GetGameObjectPath(obj);
        if (GUILayout.Button(displayName, EditorStyles.label)) {
            EditorGUIUtility.PingObject(obj);
        }

        // Select button
        if (GUILayout.Button("Select", GUILayout.Width(60))) {
            Selection.activeObject = obj;
        }

        EditorGUILayout.EndHorizontal();
    }

    private string GetGameObjectPath(GameObject obj) {
        string path = obj.name;
        Transform parent = obj.transform.parent;

        while (parent != null) {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        return path;
    }

    private void FindPrefabsWithScript() {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");

        for (int i = 0; i < guids.Length; i++) {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null) {
                System.Type targetType = targetScript.GetClass();
                if (prefab.GetComponent(targetType) != null) {
                    foundPrefabs.Add(prefab);
                }
            }

            // Update progress bar
            if (EditorUtility.DisplayCancelableProgressBar(
                "Searching Prefabs",
                $"Checking: {(prefab != null ? prefab.name : path)}",
                i / (float)guids.Length)) {
                break;
            }
        }

        EditorUtility.ClearProgressBar();
    }

    private void FindSceneObjectsWithScript() {
        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = activeScene.GetRootGameObjects();
        List<GameObject> allObjects = new List<GameObject>();

        // Gather all objects in the scene
        foreach (GameObject root in rootObjects) {
            allObjects.Add(root);
            allObjects.AddRange(GetAllChildren(root));
        }

        System.Type targetType = targetScript.GetClass();

        for (int i = 0; i < allObjects.Count; i++) {
            GameObject obj = allObjects[i];

            if (obj.GetComponent(targetType) != null) {
                foundSceneObjects.Add(obj);
            }

            // Update progress bar
            if (EditorUtility.DisplayCancelableProgressBar(
                "Searching Scene Objects",
                $"Checking: {obj.name}",
                i / (float)allObjects.Count)) {
                break;
            }
        }

        EditorUtility.ClearProgressBar();
    }

    private List<GameObject> GetAllChildren(GameObject obj) {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in obj.transform) {
            children.Add(child.gameObject);
            children.AddRange(GetAllChildren(child.gameObject));
        }
        return children;
    }
}
#endif