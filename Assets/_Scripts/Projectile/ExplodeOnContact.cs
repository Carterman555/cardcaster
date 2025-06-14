using UnityEngine;

[RequireComponent(typeof(ExplodeBehavior))]
public class ExplodeOnContact : MonoBehaviour {

    private ExplodeBehavior explodeBehavior;

    public GameObject ExcludedObject { get; set; }

    [SerializeField] private LayerMask layerMask;

    private void Awake() {
        explodeBehavior = GetComponent<ExplodeBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.gameObject == ExcludedObject) {
            return;
        }

        if (layerMask.ContainsLayer(collision.gameObject.layer)) {
            explodeBehavior.Explode(ExcludedObject);
            gameObject.ReturnToPool();
        }
    }
}
