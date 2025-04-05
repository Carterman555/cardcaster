using UnityEngine;

public class CopyColliderToCamConfiner : MonoBehaviour {

    [SerializeField] private PolygonCollider2D cameraConfiner;

    private void OnEnable() {
        CopyCollider();
    }

    private void CopyCollider() {
        GameObject cameraConfinerComposite = ReferenceSystem.Instance.CameraConfiner;
        PolygonCollider2D targetCollider = cameraConfinerComposite.AddComponent<PolygonCollider2D>();

        targetCollider.usedByComposite = true;
        targetCollider.offset = cameraConfiner.offset;

        targetCollider.pathCount = cameraConfiner.pathCount;
        for (int i = 0; i < cameraConfiner.pathCount; i++) {
            Vector2[] path = cameraConfiner.GetPath(i);
            targetCollider.SetPath(i, path);
        }

        // Adjust for different positions if needed
        targetCollider.offset += (Vector2)(cameraConfiner.transform.position - cameraConfinerComposite.transform.position);
    }
}
