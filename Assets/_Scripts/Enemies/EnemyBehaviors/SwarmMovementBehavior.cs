using System.Collections.Generic;
using System.Linq;
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
 * A bee joins a swarm if they are close enough.
 */
public class SwarmMovementBehavior : MonoBehaviour, IEffectable, IEnemyMovement {

    public bool IsLeader { get; private set; }
    public SwarmMovementBehavior Leader { get; set; }
    public bool InSwarm => Leader != null && Leader.unitsInSwarm.Count > 1;

    private bool shuffling;

    [SerializeField] private TriggerEventInvoker swarmTrigger;

    [SerializeField] private float positionVariability;

    private NavMeshAgent agent;
    private IHasEnemyStats hasStats;
    private Knockback knockback;

    [SerializeField] private bool showDebugText;
    private DebugText debugText;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        hasStats = GetComponent<IHasEnemyStats>();
        agent.speed = hasStats.GetEnemyStats().MoveSpeed;

        knockback = GetComponent<Knockback>();
    }

    private void OnEnable() {
        swarmTrigger.OnTriggerEnter_Col += OnSwarmTriggerEnter;

        Leader = this;
        IsLeader = true;
        unitsInSwarm = new List<SwarmMovementBehavior>() { this };
    }

    private void OnDisable() {
        LeaveSwarm();
    }

    private void Start() {
        if (showDebugText) {
            debugText = DebugText.Create(transform, new Vector2(0, 1f), "");
        }
    }

    private void Update() {

        // debug start
        if (showDebugText) {
            if (IsLeader) {
                debugText.SetText(gameObject.GetInstanceID() + ": Leader, swarm amount = " + unitsInSwarm.Count);
            }
            else {
                if (Leader != null) {
                    debugText.SetText(gameObject.GetInstanceID() + ": Member, Leader = " + Leader.gameObject.GetInstanceID());
                }
                else {
                    debugText.SetText(gameObject.GetInstanceID() + ": No leader");
                }
            }
        }
        // debug end

        if (IsLeader) {
            LeaderUpdate();
        }

        if (IsMoving() && agent.isStopped) {
            agent.isStopped = false;
            return;
        }

        if (shuffling) {
            float distanceThreshold = 0.1f;
            if (agent.remainingDistance < distanceThreshold) {
                float shuffleRadius = 1.5f;
                Vector2 randomPos = Leader.swarmDestination + (UnityEngine.Random.insideUnitCircle * shuffleRadius);
                agent.SetDestination(randomPos);
            }
        }
    }

    public void LeaveSwarm() {

        Leader.unitsInSwarm.Remove(this);
        shuffling = false;

        if (IsLeader) {
            if (unitsInSwarm.Count > 0) {

                SwarmMovementBehavior newLeader = unitsInSwarm.RandomItem();
                newLeader.unitsInSwarm = unitsInSwarm;

                foreach (SwarmMovementBehavior swarmMovement in newLeader.unitsInSwarm) {
                    swarmMovement.IsLeader = false;
                    swarmMovement.Leader = newLeader;
                }
                newLeader.IsLeader = true;
            }
        }

        Leader = this;
        IsLeader = true;
        unitsInSwarm = new List<SwarmMovementBehavior> { this };
    }

    private void OnSwarmTriggerEnter(Collider2D col) {

        if (InSwarm || !enabled) {
            return;
        }

        if (col.TryGetComponent(out SwarmTrigger swarmTrigger)) {
            SwarmMovementBehavior otherSwarmMovement = swarmTrigger.GetComponentInParent<SwarmMovementBehavior>();
            if (otherSwarmMovement.enabled) {
                otherSwarmMovement.Leader.JoinSwarm(this);
            }
        }
    }

    public void SetDestination(Vector2 destination) {

        if (!agent.enabled) {
            Debug.LogError($"{gameObject.GetInstanceID()}: Trying to set destination of agent which is disabled.\n" +
                $"Gameobject active: {gameObject.activeSelf}");
            return;
        }

        if (!agent.isOnNavMesh) {
            Debug.LogError($"{gameObject.GetInstanceID()}: Trying to set destination of agent which is not on nav mesh.");
            return;
        }

        agent.SetDestination(destination);
    }

    public void OnAddEffect(UnitEffect unitEffect) {
        if (unitEffect is StopMovement) {
            agent.isStopped = true;
        }
    }

    public bool IsMoving() {
        bool hasStopEffect = TryGetComponent(out StopMovement stopMovement);
        return !hasStopEffect && !knockback.IsApplyingKnockback() && enabled && agent.enabled && agent.isOnNavMesh;
    }

    #region Swarm Leader

    [SerializeField] private float[] ringDistances;
    [SerializeField] private int[] ringPositionCounts;

    private List<SwarmMovementBehavior> unitsInSwarm;

    private Vector2 swarmDestination = Vector2.zero;
    public bool SwarmDestinationSet => swarmDestination != Vector2.zero;

    private void LeaderUpdate() {

    }

    public void JoinSwarm(SwarmMovementBehavior joiningUnit) {

        if (!IsLeader) {
            Debug.LogError("Trying to join swarm, but not through leader!");
            return;
        }

        if (unitsInSwarm.Contains(joiningUnit)) {
            return;
        }

        joiningUnit.IsLeader = false;
        joiningUnit.Leader = this;
        unitsInSwarm.Add(joiningUnit);

        if (SwarmDestinationSet) {
            if (shuffling) {
                Shuffle(swarmDestination);
            }
            else {
                SetSwarmDestination(swarmDestination);
            }
        }
    }

    [SerializeField] private bool debugShowDestinations;
    [SerializeField] private GameObject debugCirclePrefab;
    private GameObject debugCircle;

    public void SetSwarmDestination(Vector2 destination) {

        foreach (SwarmMovementBehavior unit in unitsInSwarm) {
            unit.shuffling = false;
        }

        float maxDistance = 2f;
        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, maxDistance, NavMesh.AllAreas)) {
            swarmDestination = hit.position;
        }
        else {
            Debug.LogWarning("Could not find valid nav mesh position near set swarm destination!");
            return;
        }

        List<Vector3> positions = GetPositionsAround(swarmDestination, ringDistances, ringPositionCounts);
        if (debugShowDestinations) {
            if (debugCircle == null) {
                debugCircle = debugCirclePrefab.Spawn();
            }
            debugCircle.transform.position = swarmDestination;
        }


        int positionIndex = 0;
        foreach (SwarmMovementBehavior unit in unitsInSwarm) {
            Vector3 randomVariability = Random.insideUnitCircle * positionVariability;
            unit.SetDestination(positions[positionIndex] + randomVariability);

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

    public void StopAtCurrentPositions() {
        if (!IsLeader) {
            Debug.LogError("Trying to stop swarm, but not through leader!");
            return;
        }

        foreach (SwarmMovementBehavior unit in unitsInSwarm) {
            unit.SetDestination(unit.transform.position);
        }
    }

    public void Shuffle(Vector2 shufflePos) {
        if (!IsLeader) {
            Debug.LogError("Trying to shuffle, but not through leader!");
            return;
        }

        foreach (SwarmMovementBehavior unit in unitsInSwarm) {
            unit.shuffling = true;
        }

        swarmDestination = shufflePos;
    }

    public bool AnyUnitNearSwarmDest(float distanceThreshold = 0.25f) {
        if (!IsLeader) {
            Debug.LogError("Trying to check if any unit near swarm dest, but not through leader!");
            return false;
        }

        if (!enabled || !SwarmDestinationSet) {
            return false;
        }

        float thresholdSquared = distanceThreshold * distanceThreshold;
        return unitsInSwarm.Any(u => Vector2.SqrMagnitude((Vector2)u.transform.position - swarmDestination) < thresholdSquared);
    }

    public bool AllUnitsNearSwarmDest() {

        if (!IsLeader) {
            Debug.LogError("Trying to check if swarm is moving, but not through leader!");
            return false;
        }

        if (!enabled || !SwarmDestinationSet) {
            return false;
        }

        // calculate the radius of a circle that would fit the swarm
        float unitArea = agent.radius * agent.height * 2;
        float swarmArea = unitArea * unitsInSwarm.Count;
        float radiusBuffer = 1f;
        float radius = Mathf.Sqrt(swarmArea / Mathf.PI) + radiusBuffer;
        float radiusSquared = radius * radius;

        //... check if all units are within that radius of the swarm destination
        bool swarmNearDestination = unitsInSwarm.All(u => Vector2.SqrMagnitude((Vector2)u.transform.position - swarmDestination) < radiusSquared);
        return swarmNearDestination;
    }

    public List<SwarmMovementBehavior> GetUnitsInSwarm() {
        if (!IsLeader) {
            Debug.LogError("Trying to get units in swarm, but not through leader!");
        }

        return unitsInSwarm;
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
