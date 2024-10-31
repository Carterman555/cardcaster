using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BouncyProjectile : MonoBehaviour {

    private ReturnOnContact returnOnContact;
    private BounceOnContact bounceOnContact;

    private void OnEnable() {
        // find the projectile object
        returnOnContact = transform.parent.GetComponentInChildren<ReturnOnContact>();

        // make it bounce
        returnOnContact.enabled = false;
        bounceOnContact = returnOnContact.AddComponent<BounceOnContact>();
    }

    private void OnDisable() {
        // disable the bounce
        returnOnContact.enabled = true;
        Destroy(bounceOnContact);
    }
}
