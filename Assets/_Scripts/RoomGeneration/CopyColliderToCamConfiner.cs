using UnityEngine;

public class CopyColliderToCamConfiner : MonoBehaviour {

    [SerializeField] private PolygonCollider2D cameraConfiner;

    private void OnEnable() {
        GameObject cameraConfinerComposite = ReferenceSystem.Instance.CameraConfiner;
        CopyCollider(cameraConfinerComposite.GetComponent<CompositeCollider2D>());
    }

    public void CopyCollider(CompositeCollider2D targetCompositeCollider) {
        PolygonCollider2D targetPolygonCollider = targetCompositeCollider.gameObject.AddComponent<PolygonCollider2D>();
        targetPolygonCollider.usedByComposite = true;
        targetPolygonCollider.offset = cameraConfiner.offset;

        targetPolygonCollider.pathCount = cameraConfiner.pathCount;
        for (int i = 0; i < cameraConfiner.pathCount; i++) {
            Vector2[] path = cameraConfiner.GetPath(i);
            targetPolygonCollider.SetPath(i, path);
        }

        // Adjust for different positions if needed
        targetPolygonCollider.offset += (Vector2)(cameraConfiner.transform.position - targetPolygonCollider.transform.position);
    }
}
