using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnOnDisable : MonoBehaviour {
    private void OnDisable() {
        gameObject.ReturnToPool();
    }
}
