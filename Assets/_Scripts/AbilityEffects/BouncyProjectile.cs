using UnityEngine;

public class BouncyProjectile : MonoBehaviour {

    private GameObject projectile;
    private BounceOnContact bounceOnContact;

    private void OnEnable() {
        if (transform.parent.gameObject.layer == GameLayers.ProjectileLayer) {
            projectile = transform.parent.gameObject;

            // make it bounce
            if (projectile.TryGetComponent(out ReturnOnContact returnOnContact)) {
                returnOnContact.enabled = false;
            }
            bounceOnContact = projectile.AddComponent<BounceOnContact>();
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
        if (projectile.TryGetComponent(out ReturnOnContact returnOnContact)) {
            returnOnContact.enabled = true;
        }
        Destroy(bounceOnContact);
    }
}
