using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour, IHasEnemyStats {

    [SerializeField] protected ScriptableEnemy scriptableEnemy;
    public EnemyStats EnemyStats => scriptableEnemy.Stats;


}
