using UnityEngine;

public class ReturnOnContact : MonoBehaviour {

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private bool returnOther;
    [SerializeField, ConditionalHide("returnOther")] private GameObject returnTarget;

    [SerializeField] private bool playSfx;
    [SerializeField, ConditionalHide("playSfx")] private AudioClips returnSfx;

    // because sometimes trigger gets triggered by 2 objects that would return it in the same frame, and
    // only 1 of them should return the object to pool
    private bool returned;

    private void OnEnable() {
        returned = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (layerMask.ContainsLayer(collision.gameObject.layer) && enabled && !returned) {
            if (!returnOther) {
                gameObject.ReturnToPool();
                returned = true;
            }
            else {
                returnTarget.ReturnToPool();
            }

            if (playSfx) {
                AudioManager.Instance.PlaySingleSound(returnSfx);
            }
        }
    }
}
