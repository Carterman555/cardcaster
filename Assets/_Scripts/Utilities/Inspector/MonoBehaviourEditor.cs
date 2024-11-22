using UnityEngine;
using UnityEditor;


/// <summary>
/// This one is mandatory for ScriptableObjectDrawer since without it, the custom property drawer
/// will throw errors. You need a custom editor class of the component utilising a ScriptableObject.
/// So we just create a dummy editor, that can be used for every MonoBehaviour. With this empty
/// implementation it doesn’t alter anything, it just removes Unitys property drawing bug.
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true)]
public class MonoBehaviourEditor : Editor {

}