#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ScriptableObject), true)]
public class ScriptableObjectDrawer : PropertyDrawer {
    // Cached scriptable object editor
    private Editor editor = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Draw label and object field
        EditorGUI.BeginProperty(position, label, property);

        // Adjust position for object field
        Rect objectFieldRect = EditorGUI.PrefixLabel(position, label);

        // Draw object field
        EditorGUI.PropertyField(objectFieldRect, property, GUIContent.none, true);

        // Only proceed if object is set
        if (property.objectReferenceValue != null) {
            // Draw foldout if object exists
            property.isExpanded = EditorGUI.Foldout(objectFieldRect, property.isExpanded, GUIContent.none);

            // Draw foldout properties
            if (property.isExpanded) {
                // Ensure editor is created safely
                if (editor == null || editor.target != property.objectReferenceValue)
                    Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);

                // Indent child fields
                EditorGUI.indentLevel++;

                // Safely draw inspector GUI
                if (editor != null)
                    editor.OnInspectorGUI();

                // Reset indent level
                EditorGUI.indentLevel--;
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif