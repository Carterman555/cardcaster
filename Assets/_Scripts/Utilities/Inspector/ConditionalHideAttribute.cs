using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    public string ConditionalSourceField = "";

    public ConditionalHideAttribute(string conditionalSourceField)
    {
        this.ConditionalSourceField = conditionalSourceField;
    }
}
