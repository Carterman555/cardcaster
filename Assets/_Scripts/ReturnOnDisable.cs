using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnOnDisable : MonoBehaviour {
    private void OnDisable() {
        //... return to pool if not already in pool because on disable gets called when an object is returned to pool
        //... and it shouldn't returned twice
        gameObject.TryReturnToPool();
    }
}
