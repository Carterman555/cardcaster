using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAutoAttack : MonoBehaviour {

    [SerializeField] private TriggerContactTracker enemyTracker;

    public bool AutoAttacking { get; private set; }

    private void Update() {
    }
}
