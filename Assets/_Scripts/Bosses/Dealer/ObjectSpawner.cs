using System;
using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {

    [SerializeField] private Transform prefab;

    [SerializeField] private RandomFloat spawnCooldown;

    [SerializeField] private bool spawnInfinite;
    [SerializeField, ConditionalHideReversed("spawnInfinite")] private int amountToSpawn;

    private void Start() {
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects() {

        if (spawnInfinite) {
            amountToSpawn = int.MaxValue;
        }

        for (int i = 0; i < amountToSpawn; i++) {
            yield return new WaitForSeconds(spawnCooldown.Randomize());

            Vector2 cardPosition = new RoomPositionHelper().GetRandomRoomPos(
                PlayerMovement.Instance.CenterPos,
                avoidRadius: 2f,
                entranceAvoidDistance: 3f
            );

            prefab.Spawn(cardPosition, Containers.Instance.Drops);
        }
    }

    private void OnDisable() {
        StopAllCoroutines();
    }
}
