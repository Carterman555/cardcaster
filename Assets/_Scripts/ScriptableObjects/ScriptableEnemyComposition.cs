using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyComposition", menuName = "Room Generation/Enemy Composition")]
public class ScriptableEnemyComposition : ScriptableObject {

    [SerializeField] private EnemyProbability[] enemyProbabilities;
    public EnemyProbability[] EnemyProbabilities => enemyProbabilities;

}

[Serializable]
public struct EnemyProbability {
    public ScriptableEnemy ScriptableEnemy;
    public float Probability;
}
