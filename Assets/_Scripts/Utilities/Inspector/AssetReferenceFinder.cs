#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using UnityEditor.SceneManagement;

/// <summary>
/// An advanced tool to find references to any Unity asset (prefab, script, sprite, ScriptableObject, etc.)
/// throughout your project - including in scenes, prefabs, and scriptable objects.
/// By Claude
/// </summary>
public class AssetReferenceFinder : EditorWindow {
    // Target asset to find references to
    private UnityEngine.Object targetAsset;

    // Search parameters
    private bool searchScenes = true;
    private bool searchPrefabs = true;
    private bool searchScriptableObjects = true;
    private bool searchMaterials = true;
    private bool searchAnimations = false;
    private bool includeAllOpenScenes = true;
    private bool includeAssetDependencies = true;

    // Results
    private Dictionary<string, List<ReferenceInfo>> referencesByCategory = new Dictionary<string, List<ReferenceInfo>>();
    private Dictionary<string, bool> foldoutStatus = new Dictionary<string, bool>();
    private Vector2 scrollPosition;

    // UI States
    private bool isSearching = false;
    private float searchProgress = 0f;
    private string searchStatus = "";
    private bool showSearchOptions = true;
    private bool showAdvancedOptions = false;

    // For search optimization
    private HashSet<string> searchedAssets = new HashSet<string>();
    private System.Type targetAssetType;

    // Settings
    private bool groupByAssetType = true;
    private bool showAssetPaths = true;
    private int maxSearchDepth = 3;

    // Structure to hold reference information
    private class ReferenceInfo {
        public UnityEngine.Object ReferencingObject;
        public string Path;
        public string AssetType;
        public string ReferenceName;
        public string ReferenceContext;
        public int InstanceID;

        public ReferenceInfo(UnityEngine.Object obj, string path, string type, string name, string context = "") {
            ReferencingObject = obj;
            Path = path;
            AssetType = type;
            ReferenceName = name;
            ReferenceContext = context;
            InstanceID = obj != null ? obj.GetInstanceID() : 0;
        }
    }

    [MenuItem("Tools/Asset Reference Finder")]
    public static void ShowWindow() {
        GetWindow<AssetReferenceFinder>("Asset Reference Finder");
    }

    private void OnGUI() {
        GUILayout.Label("Asset Reference Finder", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Asset selection field
        targetAsset = EditorGUILayout.ObjectField("Target Asset:", targetAsset, typeof(UnityEngine.Object), false);

        if (targetAsset != null) {
            string path = AssetDatabase.GetAssetPath(targetAsset);
            EditorGUILayout.LabelField("Asset Path:", path);
            EditorGUILayout.LabelField("Asset Type:", targetAsset.GetType().Name);
        }

        EditorGUILayout.Space();

        showSearchOptions = EditorGUILayout.Foldout(showSearchOptions, "Search Options", true);
        if (showSearchOptions) {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            searchScenes = EditorGUILayout.ToggleLeft("Search Scenes", searchScenes, GUILayout.Width(150));
            searchPrefabs = EditorGUILayout.ToggleLeft("Search Prefabs", searchPrefabs, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            searchScriptableObjects = EditorGUILayout.ToggleLeft("Search ScriptableObjects", searchScriptableObjects, GUILayout.Width(150));
            searchMaterials = EditorGUILayout.ToggleLeft("Search Materials", searchMaterials, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            includeAllOpenScenes = EditorGUILayout.ToggleLeft("Include All Open Scenes", includeAllOpenScenes, GUILayout.Width(150));
            searchAnimations = EditorGUILayout.ToggleLeft("Search Animations", searchAnimations, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();

            showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "Advanced Options", true);
            if (showAdvancedOptions) {
                EditorGUI.indentLevel++;
                includeAssetDependencies = EditorGUILayout.ToggleLeft("Include Asset Dependencies", includeAssetDependencies);
                groupByAssetType = EditorGUILayout.ToggleLeft("Group Results by Asset Type", groupByAssetType);
                showAssetPaths = EditorGUILayout.ToggleLeft("Show Full Asset Paths", showAssetPaths);
                maxSearchDepth = EditorGUILayout.IntSlider("Max Search Depth", maxSearchDepth, 1, 5);

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        GUI.enabled = targetAsset != null && !isSearching && (searchScenes || searchPrefabs || searchScriptableObjects || searchMaterials);
        if (GUILayout.Button("Find References")) {
            referencesByCategory.Clear();
            foldoutStatus.Clear();
            searchedAssets.Clear();

            targetAssetType = targetAsset.GetType();
            isSearching = true;
            EditorApplication.delayCall += ExecuteSearch;
        }
        GUI.enabled = true;

        EditorGUILayout.Space();

        // Progress bar during search
        if (isSearching) {
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(GUILayout.Height(20)), searchProgress, searchStatus);
            if (GUILayout.Button("Cancel Search")) {
                isSearching = false;
                EditorUtility.ClearProgressBar();
            }
            Repaint();
        }
        else if (referencesByCategory.Count > 0) {
            // Display results
            int totalReferencesCount = referencesByCategory.Values.Sum(list => list.Count);
            EditorGUILayout.LabelField($"Found {totalReferencesCount} references in {referencesByCategory.Count} categories", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var category in referencesByCategory.Keys.OrderBy(k => k)) {
                if (!foldoutStatus.ContainsKey(category)) {
                    foldoutStatus[category] = true;
                }

                var references = referencesByCategory[category];

                foldoutStatus[category] = EditorGUILayout.Foldout(foldoutStatus[category], $"{category} ({references.Count})", true);
                if (foldoutStatus[category]) {
                    EditorGUI.indentLevel++;

                    foreach (var refInfo in references) {
                        if (refInfo.ReferencingObject == null)
                            continue;

                        EditorGUILayout.BeginHorizontal();

                        string displayName = refInfo.ReferenceName;
                        if (showAssetPaths && !string.IsNullOrEmpty(refInfo.Path)) {
                            displayName = refInfo.Path;
                        }

                        if (!string.IsNullOrEmpty(refInfo.ReferenceContext)) {
                            displayName = $"{displayName} ({refInfo.ReferenceContext})";
                        }

                        if (GUILayout.Button(displayName, EditorStyles.label)) {
                            EditorGUIUtility.PingObject(refInfo.ReferencingObject);
                        }

                        if (GUILayout.Button("Select", GUILayout.Width(60))) {
                            Selection.activeObject = refInfo.ReferencingObject;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Export Results to CSV")) {
                ExportResultsToCSV();
            }
        }
        else if (targetAsset != null && !isSearching) {
            EditorGUILayout.HelpBox("No references found. Try adjusting your search options.", MessageType.Info);
        }
    }

    private void ExecuteSearch() {
        if (!isSearching)
            return;

        try {
            searchProgress = 0f;
            searchStatus = "Preparing search...";

            string assetPath = AssetDatabase.GetAssetPath(targetAsset);

            // Search active scene first
            if (searchScenes) {
                int sceneCount = includeAllOpenScenes ? SceneManager.sceneCount : 1;
                for (int i = 0; i < sceneCount; i++) {
                    if (!isSearching) break;

                    Scene scene = i == 0 ? SceneManager.GetActiveScene() : SceneManager.GetSceneAt(i);
                    if (scene.isLoaded) {
                        searchStatus = $"Searching scene: {scene.name}";
                        FindReferencesInScene(scene);
                    }
                }

                // Search all scenes in the project
                if (isSearching) {
                    string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
                    for (int i = 0; i < sceneGuids.Length; i++) {
                        if (!isSearching) break;

                        string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                        searchProgress = (float)i / sceneGuids.Length * 0.3f + 0.1f;
                        searchStatus = $"Searching scene asset: {Path.GetFileNameWithoutExtension(scenePath)}";

                        // Skip already opened scenes
                        bool isOpen = false;
                        for (int s = 0; s < SceneManager.sceneCount; s++) {
                            if (SceneManager.GetSceneAt(s).path == scenePath) {
                                isOpen = true;
                                break;
                            }
                        }

                        if (!isOpen) {
                            // We'll check dependencies rather than opening each scene
                            string[] dependencies = AssetDatabase.GetDependencies(scenePath, true);
                            if (dependencies.Contains(assetPath)) {
                                // The scene references our target asset!
                                Scene sceneAsset = EditorSceneManager.GetActiveScene();
                                string category = "Scenes";
                                string refName = Path.GetFileNameWithoutExtension(scenePath);
                                AddReferenceToResults(
                                    AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath),
                                    scenePath,
                                    category,
                                    refName
                                );
                            }
                        }
                    }
                }
            }

            // Search prefabs
            if (isSearching && searchPrefabs) {
                string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
                for (int i = 0; i < prefabGuids.Length; i++) {
                    if (!isSearching) break;

                    string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                    searchProgress = (float)i / prefabGuids.Length * 0.3f + 0.4f;
                    searchStatus = $"Searching prefab: {Path.GetFileNameWithoutExtension(path)}";

                    FindReferencesInAsset(path);
                }
            }

            // Search scriptable objects
            if (isSearching && searchScriptableObjects) {
                string[] soGuids = AssetDatabase.FindAssets("t:ScriptableObject");
                for (int i = 0; i < soGuids.Length; i++) {
                    if (!isSearching) break;

                    string path = AssetDatabase.GUIDToAssetPath(soGuids[i]);
                    searchProgress = (float)i / soGuids.Length * 0.15f + 0.7f;
                    searchStatus = $"Searching ScriptableObject: {Path.GetFileNameWithoutExtension(path)}";

                    FindReferencesInAsset(path);
                }
            }

            // Search materials
            if (isSearching && searchMaterials) {
                string[] materialGuids = AssetDatabase.FindAssets("t:Material");
                for (int i = 0; i < materialGuids.Length; i++) {
                    if (!isSearching) break;

                    string path = AssetDatabase.GUIDToAssetPath(materialGuids[i]);
                    searchProgress = (float)i / materialGuids.Length * 0.1f + 0.85f;
                    searchStatus = $"Searching Material: {Path.GetFileNameWithoutExtension(path)}";

                    FindReferencesInAsset(path);
                }
            }

            // Search animation clips
            if (isSearching && searchAnimations) {
                string[] animGuids = AssetDatabase.FindAssets("t:AnimationClip");
                for (int i = 0; i < animGuids.Length; i++) {
                    if (!isSearching) break;

                    string path = AssetDatabase.GUIDToAssetPath(animGuids[i]);
                    searchProgress = (float)i / animGuids.Length * 0.05f + 0.95f;
                    searchStatus = $"Searching Animation: {Path.GetFileNameWithoutExtension(path)}";

                    FindReferencesInAsset(path);
                }
            }
        }
        finally {
            isSearching = false;
            EditorUtility.ClearProgressBar();
        }
    }

    private void FindReferencesInScene(Scene scene) {
        if (!scene.isLoaded)
            return;

        GameObject[] rootObjects = scene.GetRootGameObjects();
        string category = "Scenes";

        for (int i = 0; i < rootObjects.Length; i++) {
            if (!isSearching) return;

            GameObject root = rootObjects[i];
            searchStatus = $"Searching in scene {scene.name}: {root.name}";

            // Check if this GameObject uses the target asset
            CheckGameObjectForReferences(root, scene.path, category);
        }
    }

    private void FindReferencesInAsset(string assetPath) {
        if (searchedAssets.Contains(assetPath))
            return;

        searchedAssets.Add(assetPath);

        // Check if this asset directly depends on our target
        string targetPath = AssetDatabase.GetAssetPath(targetAsset);
        string[] dependencies = AssetDatabase.GetDependencies(assetPath, includeAssetDependencies);

        if (dependencies.Contains(targetPath)) {
            // Determine the asset type for categorization
            System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            string category = DetermineCategory(assetType);

            // Load the asset
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if (asset != null) {
                string assetName = Path.GetFileNameWithoutExtension(assetPath);

                // Handle prefabs specially to find component references
                if (asset is GameObject prefab) {
                    CheckPrefabForReferences(prefab, assetPath, category);
                }
                // Handle ScriptableObjects specially
                else if (asset is ScriptableObject so) {
                    AddReferenceToResults(asset, assetPath, category, assetName);
                }
                // Handle other assets
                else {
                    AddReferenceToResults(asset, assetPath, category, assetName);
                }
            }
        }
    }

    private void CheckPrefabForReferences(GameObject prefab, string prefabPath, string category) {
        // First check if the prefab itself references our target
        if (IsPrefabReferencingTarget(prefab)) {
            string prefabName = prefab.name;
            AddReferenceToResults(prefab, prefabPath, category, prefabName);
        }

        // Then check all components on the prefab and its children
        CheckGameObjectForReferences(prefab, prefabPath, category);
    }

    private bool IsPrefabReferencingTarget(GameObject prefab) {
        // This could be extended to check specific prefab properties
        return false;
    }

    private void CheckGameObjectForReferences(GameObject gameObject, string path, string baseCategory) {
        // Process this GameObject
        Component[] components = gameObject.GetComponents<Component>();
        foreach (Component component in components) {
            if (component == null) continue;

            // Skip certain built-in components that don't typically reference assets
            if (component is Transform || component is RectTransform)
                continue;

            // Check the component itself (mainly for MonoBehaviours)
            CheckComponentForReferences(component, gameObject, path, baseCategory);
        }

        // Process children
        foreach (Transform child in gameObject.transform) {
            CheckGameObjectForReferences(child.gameObject, path, baseCategory);
        }
    }

    private void CheckComponentForReferences(Component component, GameObject gameObject, string path, string baseCategory) {
        if (component == null)
            return;

        // Special case: MonoBehaviour script check
        MonoBehaviour mb = component as MonoBehaviour;
        if (mb != null && mb.GetType() == targetAssetType) {
            string category = "MonoBehaviours";
            string objPath = GetGameObjectPath(gameObject);
            string refName = $"{objPath} ({component.GetType().Name})";
            AddReferenceToResults(gameObject, path, category, refName);
            return;
        }

        // For Image, SpriteRenderer, etc. check for Sprite reference
        if (targetAsset is Sprite) {
            if ((component is UnityEngine.UI.Image image && image.sprite == targetAsset) ||
                (component is SpriteRenderer spriteRenderer && spriteRenderer.sprite == targetAsset)) {
                string category = "Sprites";
                string objPath = GetGameObjectPath(gameObject);
                string refName = $"{objPath} ({component.GetType().Name})";
                AddReferenceToResults(gameObject, path, category, refName, component.GetType().Name);
                return;
            }
        }

        // For Renderer, check for Material reference
        if (targetAsset is Material) {
            if (component is Renderer renderer) {
                bool foundReference = false;
                Material[] materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++) {
                    if (materials[i] == targetAsset) {
                        string category = "Materials";
                        string objPath = GetGameObjectPath(gameObject);
                        string refName = $"{objPath} ({component.GetType().Name})";
                        string context = $"Material slot {i}";
                        AddReferenceToResults(gameObject, path, category, refName, context);
                        foundReference = true;
                        break;
                    }
                }

                if (foundReference) return;
            }
        }

        // Check for prefab references
        if (targetAsset is GameObject targetPrefab) {
            string targetPrefabPath = AssetDatabase.GetAssetPath(targetPrefab);

            // Check if this component has a prefab reference (via fields)
            System.Reflection.FieldInfo[] fields = component.GetType().GetFields(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            foreach (var field in fields) {
                if (field.FieldType == typeof(GameObject) || field.FieldType.IsSubclassOf(typeof(Component))) {
                    var value = field.GetValue(component);
                    if (value != null) {
                        GameObject referencedObj = null;

                        if (value is GameObject go) {
                            referencedObj = go;
                        }
                        else if (value is Component comp) {
                            referencedObj = comp.gameObject;
                        }

                        if (referencedObj != null) {
                            var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(referencedObj);
                            if (prefabAsset != null && AssetDatabase.GetAssetPath(prefabAsset) == targetPrefabPath) {
                                string category = "Prefabs";
                                string objPath = GetGameObjectPath(gameObject);
                                string refName = $"{objPath} ({component.GetType().Name})";
                                string context = $"Field: {field.Name}";
                                AddReferenceToResults(gameObject, path, category, refName, context);
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Check for ScriptableObject references
        if (targetAsset is ScriptableObject targetSO) {
            // Check if this component references the ScriptableObject
            System.Reflection.FieldInfo[] fields = component.GetType().GetFields(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            foreach (var field in fields) {
                if (field.FieldType.IsAssignableFrom(targetSO.GetType())) {
                    var value = field.GetValue(component);
                    if (value != null && value.Equals(targetSO)) {
                        string category = "ScriptableObjects";
                        string objPath = GetGameObjectPath(gameObject);
                        string refName = $"{objPath} ({component.GetType().Name})";
                        string context = $"Field: {field.Name}";
                        AddReferenceToResults(gameObject, path, category, refName, context);
                        break;
                    }
                }
            }
        }
    }

    private string GetGameObjectPath(GameObject obj) {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        int depth = 0;

        while (parent != null && depth < maxSearchDepth) {
            path = parent.name + "/" + path;
            parent = parent.parent;
            depth++;
        }

        if (parent != null) {
            path = ".../" + path;
        }

        return path;
    }

    private string DetermineCategory(System.Type assetType) {
        if (assetType == typeof(ScriptableObject) || assetType.IsSubclassOf(typeof(ScriptableObject)))
            return "ScriptableObjects";
        else if (assetType == typeof(GameObject))
            return "Prefabs";
        else if (assetType == typeof(Material))
            return "Materials";
        else if (assetType == typeof(Texture) || assetType == typeof(Texture2D) || assetType == typeof(Sprite))
            return "Textures";
        else if (assetType == typeof(AnimationClip))
            return "Animations";
        else if (assetType == typeof(SceneAsset))
            return "Scenes";
        else if (assetType == typeof(AudioClip))
            return "Audio";
        else if (assetType == typeof(Shader))
            return "Shaders";
        else if (assetType == typeof(Font))
            return "Fonts";
        else if (assetType == typeof(MonoScript))
            return "Scripts";
        else
            return "Other Assets";
    }

    private void AddReferenceToResults(UnityEngine.Object asset, string path, string baseCategory, string name, string context = "") {
        if (asset == null)
            return;

        string category = groupByAssetType ? baseCategory : "All References";

        if (!referencesByCategory.ContainsKey(category)) {
            referencesByCategory[category] = new List<ReferenceInfo>();
        }

        // Check if we already have this exact reference
        if (!referencesByCategory[category].Any(r => r.InstanceID == asset.GetInstanceID() && r.ReferenceContext == context)) {
            ReferenceInfo info = new ReferenceInfo(asset, path, asset.GetType().Name, name, context);
            referencesByCategory[category].Add(info);
        }
    }

    private void ExportResultsToCSV() {
        string filePath = EditorUtility.SaveFilePanel("Save CSV", "", "AssetReferenceFinderResults.csv", "csv");
        if (string.IsNullOrEmpty(filePath))
            return;

        using (StreamWriter writer = new StreamWriter(filePath)) {
            // Write header
            writer.WriteLine("Category,Asset Type,Reference Name,Path,Context");

            // Write data
            foreach (var category in referencesByCategory.Keys) {
                foreach (var refInfo in referencesByCategory[category]) {
                    string line = $"\"{category}\",\"{refInfo.AssetType}\",\"{refInfo.ReferenceName}\",\"{refInfo.Path}\",\"{refInfo.ReferenceContext}\"";
                    writer.WriteLine(line);
                }
            }
        }

        EditorUtility.RevealInFinder(filePath);
    }
}
#endif