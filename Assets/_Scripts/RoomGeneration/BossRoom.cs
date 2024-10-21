using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour {
    [SerializeField] private Transform bossSpawnPoint;

    public Transform GetBossSpawnPoint() {
        return bossSpawnPoint;
    }
}
