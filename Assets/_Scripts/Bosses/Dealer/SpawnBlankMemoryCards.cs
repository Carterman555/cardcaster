using System.Collections;
using UnityEngine;

public class SpawnBlankMemoryCards : MonoBehaviour {

    [SerializeField] private BlankMemoryCardDrop blankMemoryCardPrefab;

    [SerializeField] private RandomFloat spawnCooldown;
    [SerializeField] private int amountToSpawn;

    private void Start() {
        StartCoroutine(SpawnCards());
    }

    private IEnumerator SpawnCards() {
        for (int i = 0; i < amountToSpawn; i++) {
            yield return new WaitForSeconds(spawnCooldown.Randomize());

            Vector2 cardPosition = new RoomPositionHelper().GetRandomRoomPos(
                PlayerMovement.Instance.CenterPos,
                avoidRadius: 2f,
                entranceAvoidDistance: 3f
            );

            blankMemoryCardPrefab.Spawn(cardPosition, Containers.Instance.Drops);
        }
    }
}
