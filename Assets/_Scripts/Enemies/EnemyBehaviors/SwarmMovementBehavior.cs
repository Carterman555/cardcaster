using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * There will be one bee per swarm that is the group leader. It will calculate the desired pos
 * of all the bees in the swarm to be around him.
 * Also each bee randomly deviates a little from their desired pos to look more natural.
 * The leader bee has a list of the bees in the group and tells them their destination or desired pos.
 * The leader leads the swarm to the nearest plant, then stops their to create a new bee. If there are
 * no plants the bees try to keep a distance from the player (but they are slow), while also wandering
 * with random movement
 * A bee joins a swarm if they are close enough. once a bee is in a swarm, they don't leave
 */
public class SwarmMovementBehavior : MonoBehaviour {

    public bool IsLeader { get; private set; }
    public SwarmMovementBehavior Leader { get; set; }
    public bool InSwarm => Leader != null;

    [SerializeField] private TriggerEventInvoker swarmTrigger;

    private Vector2 desiredPos;
    public NavMeshAgent Agent { get; private set; }

    private IHasEnemyStats hasStats;

    private void Awake() {
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;

        hasStats = GetComponent<IHasEnemyStats>();
        Agent.speed = hasStats.EnemyStats.MoveSpeed;
    }

    private void OnEnable() {
        swarmTrigger.OnTriggerEnter_Col += OnSwarmTriggerEnter;

        Leader = null;
        IsLeader = false;
        unitsInSwarm = new List<SwarmMovementBehavior>();
    }

    private void OnSwarmTriggerEnter(Collider2D col) {

        if (InSwarm) {
            return;
        }

        if (col.TryGetComponent(out SwarmTrigger swarmTrigger)) {
            SwarmMovementBehavior otherSwarmMovement = swarmTrigger.GetComponentInParent<SwarmMovementBehavior>();
            if (otherSwarmMovement.InSwarm) {
                otherSwarmMovement.Leader.JoinSwarm(this);
            }

            // neither bee is in a swarm so create one
            else {
                Leader = this;
                IsLeader = true;
                print(GetInstanceID() + ": set to leader");
            }
        }
    }

    public void SetDesiredPos(Vector2 desiredPos) {
        this.desiredPos = desiredPos;
        Agent.SetDestination(desiredPos);

        print(GetInstanceID() + ": set desired pos to " + desiredPos);
    }


    #region Swarm Leader

    [SerializeField] private float[] ringDistances;
    [SerializeField] private int[] ringPositionCounts;

    private List<SwarmMovementBehavior> unitsInSwarm;

    public void JoinSwarm(SwarmMovementBehavior joiningUnit) {

        if (!IsLeader) {
            Debug.LogError("Trying to join swarm, but not through leader!");
            return;
        }

        print(joiningUnit.gameObject.GetInstanceID() + " is joining swarm where leader is " + GetInstanceID());

        joiningUnit.Leader = this;
        unitsInSwarm.Add(joiningUnit);
        UpdateUnitPositions();
    }

    public void SetSwarmDestination(Vector2 destination) {
        List<Vector3> positions = GetPositionsAround(destination, ringDistances, ringPositionCounts);

        int positionIndex = 0;
        foreach (SwarmMovementBehavior unit in unitsInSwarm) {
            unit.SetDesiredPos(positions[positionIndex]);

            positionIndex++;
            if (positionIndex >= positions.Count) {
                Debug.LogError("Ran out of positions for swarm!");
            }
        }
    }

    public bool IsSwarmMoving() {

        if (!IsLeader) {
            Debug.LogError("Trying to check if swarm is moving, but not through leader!");
            return false;
        }

        // if any units are moving, count the swarm as moving
        bool swarmMoving = false;
        foreach (SwarmMovementBehavior unit in unitsInSwarm) {
            float distanceThreshold = 0.1f;
            if (unit.Agent.remainingDistance > distanceThreshold) {
                swarmMoving = true;
            }
        }

        return swarmMoving;
    }

    private void UpdateUnitPositions() {
        SetSwarmDestination(transform.position);
    }

    private List<Vector3> GetPositionsAround(Vector2 centerPosition, float[] ringDistances, int[] ringPositionCounts) {
        List<Vector3> positions = new() {
            centerPosition
        };
        for (int i = 0; i < ringDistances.Length; i++) {
            positions.AddRange(GetPositionsAround(centerPosition, ringDistances[i], ringPositionCounts[i]));
        }
        return positions;
    }

    private List<Vector3> GetPositionsAround(Vector2 centerPosition, float distance, int positionCount) {
        List<Vector3> positions = new();
        for (int i = 0; i < positionCount; i++) {
            float angle = i * (360f / positionCount);

            Vector2 dir = Vector2.right;
            dir.RotateDirection(angle);

            Vector2 position = centerPosition + dir * distance;
            positions.Add(position);
        }
        return positions;
    }

    #endregion
}
