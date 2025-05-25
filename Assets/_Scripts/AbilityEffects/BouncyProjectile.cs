using UnityEngine;

public class BouncyProjectile : MonoBehaviour {

    private GameObject projectile;
    private BounceOnContact bounceOnContact;

    private ReturnOnContact returnOnContact;

    private void OnEnable() {
        if (transform.parent.gameObject.layer == GameLayers.ProjectileLayer) {
            projectile = transform.parent.gameObject;

            // make it bounce
            returnOnContact = projectile.GetComponentInChildren<ReturnOnContact>();
            if (returnOnContact != null) {
                returnOnContact.enabled = false;
            }
            bounceOnContact = projectile.AddComponent<BounceOnContact>();

            //... bounce off the same layers that would have been destroyed by
            bounceOnContact.BounceLayerMask = returnOnContact.LayerMask;
        }
        else {
            projectile = null;
        }
    }

    private void OnDisable() {

        if (projectile == null) {
            return;
        }

        // disable the bounce
        if (returnOnContact != null) {
            returnOnContact.enabled = false;
        }
        Destroy(bounceOnContact);
    }
}
