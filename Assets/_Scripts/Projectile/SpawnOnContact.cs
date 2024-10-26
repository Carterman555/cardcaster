using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnContact : MonoBehaviour {

    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private Container spawnContainer;

    [SerializeField] private LayerMask layerMask;

    //... just so this script can be disabled 
    private void OnEnable() { }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (layerMask.ContainsLayer(collision.gameObject.layer) && enabled) {
            spawnPrefab.Spawn(transform.position, Containers.Instance.GetContainer(spawnContainer));
        }
    }
}
