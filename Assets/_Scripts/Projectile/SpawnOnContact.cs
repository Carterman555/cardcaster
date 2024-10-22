using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnContact : MonoBehaviour {

    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private Container spawnContainer;

    [SerializeField] private LayerMask layerMask;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (layerMask.ContainsLayer(collision.gameObject.layer)) {
            spawnPrefab.Spawn(transform.position, Containers.Instance.GetContainer(spawnContainer));
        }
    }
}
