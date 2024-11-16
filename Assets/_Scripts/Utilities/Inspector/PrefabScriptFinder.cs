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
        string displayName = isPrefab ? GetPrefabPath(obj) : GetGameObjectPath(obj);
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

    private string GetPrefabPath(GameObject obj) {
        GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);
        if (prefabRoot == null) prefabRoot = obj;

        string path = obj.name;
        Transform current = obj.transform;
        Transform rootTransform = prefabRoot.transform;

        while (current != rootTransform && current.parent != null) {
            path = current.parent.name + "/" + path;
            current = current.parent;
        }

        return path;
    }

    private void FindPrefabsWithScript() {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        System.Type targetType = targetScript.GetClass();

        for (int i = 0; i < guids.Length; i++) {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null) {
                // Check the prefab and all its children for the script
                Component[] components = prefab.GetComponentsInChildren(targetType, true);
                if (components != null && components.Length > 0) {
                    // Add each game object that has the component
                    foreach (Component component in components) {
                        if (!foundPrefabs.Contains(component.gameObject)) {
                            foundPrefabs.Add(component.gameObject);
                        }
                    }
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
        System.Type targetType = targetScript.GetClass();

        for (int i = 0; i < rootObjects.Length; i++) {
            GameObject root = rootObjects[i];
            Component[] components = root.GetComponentsInChildren(targetType, true);

            if (components != null && components.Length > 0) {
                foreach (Component component in components) {
                    if (!foundSceneObjects.Contains(component.gameObject)) {
                        foundSceneObjects.Add(component.gameObject);
                    }
                }
            }

            // Update progress bar
            if (EditorUtility.DisplayCancelableProgressBar(
                "Searching Scene Objects",
                $"Checking: {root.name}",
                i / (float)rootObjects.Length)) {
                break;
            }
        }

        EditorUtility.ClearProgressBar();
    }
}
#endif