using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Position Spawn")]
public class ScriptablePositionSpawnCard : ScriptableCardBaseOld {

    [SerializeField] private GameObject objectToSpawn;

    public override void Play(Vector2 position) {
        base.Play(position);

        GameObject newObject = objectToSpawn.Spawn(position, Containers.Instance.Projectiles);
    }
}
