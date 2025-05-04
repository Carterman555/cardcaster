using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * There will be one bee per swarm that is the group leader. It will calculate the desired pos
 * of all the bees in the swarm to be around him. Each bee also moves up and down from animation.
 * Also each bee randomly deviates a little from their desired pos to look more natural.
 * The leader bee has a list of the bees in the group and tells them their destination or desired pos.
 * The leader leads the swarm to the nearest plant, then stops their to create a new bee. If there are
 * no plants the bees try to keep a distance from the player (but they are slow), while also wandering
 * with random movement
 * A bee joins a swarm if they are close enough. once a bee is in a swarm, they don't leave
 */
public class SwarmMovementBehavior : MonoBehaviour {

    private bool isLeader;
    private SwarmMovementBehavior leader;

    [SerializeField] private TriggerEventInvoker swarmTrigger;

    private void OnEnable() {
        swarmTrigger.OnTriggerEnter_Col += OnSwarmTriggerEnter;

        leader = null;
        isLeader = false;
    }

    private void OnSwarmTriggerEnter(Collider2D col) {
        if (col.TryGetComponent(out SwarmTrigger swarmTrigger)) {

        }
    }
}
