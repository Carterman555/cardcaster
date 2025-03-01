using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BouncyProjectile : MonoBehaviour {

    private GameObject projectile;
    private BounceOnContact bounceOnContact;

    private void OnEnable() {
        // find the projectile object
        projectile = null;
        foreach (Transform child in transform.parent) {
            if (child.gameObject.layer == GameLayers.ProjectileLayer) {
                projectile = child.gameObject;
            }
        }

        if (projectile == null) {
            Debug.LogWarning("BouncyProjectile effect added but can't find projectile! Did you forget to change layer to projectile?");
        }

        // make it bounce
        if (projectile.TryGetComponent(out ReturnOnContact returnOnContact)) {
            returnOnContact.enabled = false;
        }
        bounceOnContact = projectile.AddComponent<BounceOnContact>();
    }

    private void OnDisable() {
        // disable the bounce
        if (projectile.TryGetComponent(out ReturnOnContact returnOnContact)) {
            returnOnContact.enabled = true;
        }
        Destroy(bounceOnContact);
    }
}
