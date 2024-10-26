using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalHideFlagAttribute))]
public class ConditionalHideFlagPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        ConditionalHideFlagAttribute condHAtt = (ConditionalHideFlagAttribute)attribute;
        bool enabled = GetConditionalHideFlagAttributeResult(condHAtt, property);

        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (enabled) {
            EditorGUI.PropertyField(position, property, label, true);
        }
        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        ConditionalHideFlagAttribute condHAtt = (ConditionalHideFlagAttribute)attribute;
        bool enabled = GetConditionalHideFlagAttributeResult(condHAtt, property);

        if (enabled) {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    private bool GetConditionalHideFlagAttributeResult(ConditionalHideFlagAttribute condHAtt, SerializedProperty property) {
        SerializedProperty sourcePropertyValue = null;

        // Try to find the property in the immediate parent
        sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.FlagSourceField);

        // If not found, search in the parent's parent
        if (sourcePropertyValue == null) {
            string parentPath = property.propertyPath.Substring(0, property.propertyPath.LastIndexOf('.'));
            sourcePropertyValue = property.serializedObject.FindProperty($"{parentPath}.{condHAtt.FlagSourceField}");
        }

        if (sourcePropertyValue != null && sourcePropertyValue.propertyType == SerializedPropertyType.Enum) {
            AbilityAttribute flags = (AbilityAttribute)sourcePropertyValue.intValue;
            return flags.HasFlag(condHAtt.RequiredFlag);
        }
        else {
            Debug.LogWarning($"ConditionalHideFlagAttribute: Property '{condHAtt.FlagSourceField}' not found for '{property.propertyPath}'");
            return true; // Show the property if we can't find the source field
        }
    }
}
#endif

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideFlagAttribute : PropertyAttribute {
    public string FlagSourceField = "";
    public AbilityAttribute RequiredFlag;

    public ConditionalHideFlagAttribute(string flagSourceField, AbilityAttribute requiredFlag) {
        this.FlagSourceField = flagSourceField;
        this.RequiredFlag = requiredFlag;
    }
}