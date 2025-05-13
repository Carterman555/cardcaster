using TMPro;
using UnityEngine;

public class DebugText : MonoBehaviour {

    public static DebugText Create(Transform target, Vector2 offset, string text) {
        Vector2 pos = (Vector2)target.position + offset;
        DebugText debugText = AssetSystem.Instance.DebugTextPrefab.Spawn(pos, target);
        debugText.SetText(text);
        return debugText;
    }

    private TextMeshPro textMeshPro;

    private void Awake() {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    private void Update() {
        //.. so doesn't flip text when unit flips
        transform.rotation = Quaternion.identity;
    }

    public void SetText(string text) {
        textMeshPro.text = text;
    }
}
