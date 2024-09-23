using System;
using UnityEngine;

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