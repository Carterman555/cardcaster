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
public class SwarmMovementBehavior : MonoBehaviour, IEffectable, IEnemyMovement {

    public bool IsLeader { get; private set; }
    public SwarmMovementBehavior Leader { get; set; }
    public bool InSwarm => Leader != null;

    public bool Shuffling { get; set; }

    [SerializeField] private TriggerEventInvoker swarmTrigger;
    
    private Vector2 desiredPos;

    private NavMeshAgent agent;
    private IHasEnemyStats hasStats;
    private Knockback knockback;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        hasStats = GetComponent<IHasEnemyStats>();
        agent.speed = hasStats.EnemyStats.MoveSpeed;

        knockback = GetComponent<Knockback>();
    }

    private void OnEnable() {
        swarmTrigger.OnTriggerEnter_Col += OnSwarmTriggerEnter;

        Leader = this;
        IsLeader = true;
        unitsInSwarm = new HashSet<SwarmMovementBehavior>() { this };
    }

    private void Update() {

        if (IsMoving() && agent.isStopped) {
            agent.isStopped = false;
            return;
        }

        if (Shuffling) {
            float distanceThreshold = 0.1f;
            if (agent.remainingDistance < distanceThreshold) {
                float shuffleRadius = 1.5f;
                Vector2 randomPos = Leader.swarmDestination + (UnityEngine.Random.insideUnitCircle * shuffleRadius);
                agent.SetDestination(randomPos);
            }
        }
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
                unitsInSwarm.Add(this);
            }
        }
    }

    public void SetDesiredPos(Vector2 desiredPos) {
        this.desiredPos = desiredPos;
        agent.SetDestination(desiredPos);
    }

    public void OnAddEffect(UnitEffect unitEffect) {
        if (unitEffect is StopMovement) {
            agent.isStopped = true;
        }
    }

    public bool IsMoving() {
        bool hasStopEffect = TryGetComponent(out StopMovement stopMovement);
        return !hasStopEffect && !knockback.IsApplyingKnockback() && enabled;
    }

    #region Swarm Leader

    [SerializeField] private float[] ringDistances;
    [SerializeField] private int[] ringPositionCounts;

    private HashSet<SwarmMovementBehavior> unitsInSwarm;

    private Vector2 swarmDestination = Vector2.zero;

    public void JoinSwarm(SwarmMovementBehavior joiningUnit) {

        if (!IsLeader) {
            Debug.LogError("Trying to join swarm, but not through leader!");
            return;
        }

        joiningUnit.Leader = this;
        unitsInSwarm.Add(joiningUnit);

        StopAndSwarmAroundLeader();
    }

    public void SetSwarmDestination(Vector2 destination) {

        foreach (SwarmMovementBehavior unit in unitsInSwarm) {
            unit.Shuffling = false;
        }

        swarmDestination = destination;

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

    public void StopAndSwarmAroundLeader() {
        if (!IsLeader) {
            Debug.LogError("Trying to swarm around leader, but not through leader!");
            return;
        }

        SetSwarmDestination(transform.position);
    }

    public void StopAndShuffle() {
        if (!IsLeader) {
            Debug.LogError("Trying to shuffle, but not through leader!");
            return;
        }

        foreach (SwarmMovementBehavior unit in unitsInSwarm) {
            unit.Shuffling = true;
        }

        swarmDestination = transform.position;
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
            if (unit.agent.remainingDistance > distanceThreshold) {
                swarmMoving = true;
            }
        }

        return swarmMoving;
    }

    public HashSet<SwarmMovementBehavior> GetUnitsInSwarm() {
        if (!IsLeader) {
            Debug.LogError("Trying to get units in swarm, but not through leader!");
        }

        return unitsInSwarm;
    }

    private void UpdateUnitPositions() {
        bool destinationSet = swarmDestination != Vector2.zero;
        if (destinationSet) {
            SetSwarmDestination(swarmDestination);
        }
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
