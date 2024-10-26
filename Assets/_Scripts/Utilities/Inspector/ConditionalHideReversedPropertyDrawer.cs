using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalHideReversedAttribute))]
public class ConditionalHideReversedPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        ConditionalHideReversedAttribute condHAtt = (ConditionalHideReversedAttribute)attribute;
        bool enabled = !GetConditionalHideAttributeResult(condHAtt, property); // Note the ! operator
        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (enabled) {
            EditorGUI.PropertyField(position, property, label, true);
        }
        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        ConditionalHideReversedAttribute condHAtt = (ConditionalHideReversedAttribute)attribute;
        bool enabled = !GetConditionalHideAttributeResult(condHAtt, property); // Note the ! operator
        if (enabled) {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    private bool GetConditionalHideAttributeResult(ConditionalHideReversedAttribute condHAtt, SerializedProperty property) {
        bool enabled = true;
        string propertyPath = property.propertyPath;
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField);
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
        if (sourcePropertyValue != null) {
            enabled = sourcePropertyValue.boolValue;
        }
        else {
            Debug.LogWarning("Attempting to use a ConditionalHideReversedAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }
        return enabled;
    }
}
#endif

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideReversedAttribute : PropertyAttribute {
    public string ConditionalSourceField = "";
    public ConditionalHideReversedAttribute(string conditionalSourceField) {
        this.ConditionalSourceField = conditionalSourceField;
    }
}