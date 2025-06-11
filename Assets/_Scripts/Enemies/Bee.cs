using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// Script execution order reason: needs invoke ChangeSwarmState in onEnable after swarmMovementBehavior sets itself to leader in onEnable
public class Bee : Enemy {

    private Rigidbody2D rb;
    private NavMeshAgent agent;

    private SwarmMovementBehavior swarmMovement;
    private WanderMovementBehavior wanderMovement;
    private FaceMoveDirectionBehavior faceBehavior;

    private List<Transform> bluePlantsInRoom;

    private Transform reproducingPlant;

    [SerializeField] private float reproduceTime;
    private float reproduceTimer;

    private enum SwarmState { MovingToPlant, Wandering, Reproducing }
    private SwarmState swarmState; // all bees in a swarm always have the same swarmState

    private enum BeeState { FollowingSwarmBehavior, ShootingStinger, Launching }
    private BeeState beeState;

    protected override void Awake() {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        swarmMovement = GetComponent<SwarmMovementBehavior>();
        wanderMovement = GetComponent<WanderMovementBehavior>();
        faceBehavior = GetComponent<FaceMoveDirectionBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        reproducingPlant = null;

        // renable cause could be disabled from launch
        swarmMovement.enabled = true;
        agent.enabled = true;

        beeState = BeeState.FollowingSwarmBehavior;

        if (Room.GetCurrentRoom() != null) {
            Transform[] allObjectsInRoom = Room.GetCurrentRoom().GetComponentsInChildren<Transform>();
            bluePlantsInRoom = allObjectsInRoom.Where(g => g.CompareTag("BluePlant")).ToList();

            foreach (Transform plant in bluePlantsInRoom) {
                plant.GetComponent<MonoBehaviourEventInvoker>().OnDisabled += OnPlantDisabled;
            }
        }
        else {
            Debug.LogWarning("Trying to find blue plants, but no room is set!");
        }

        reproduceTimer = float.PositiveInfinity;

        anim.SetBool("launching", false);

        if (bluePlantsInRoom.Count > 0) {
            ChangeSwarmState(SwarmState.MovingToPlant);
        }
        else if (bluePlantsInRoom.Count == 0) {
            ChangeSwarmState(SwarmState.Wandering);
        }
    }

    protected override void OnDisable() {
        base.OnDisable();

        foreach (Transform plant in bluePlantsInRoom) {
            plant.GetComponent<MonoBehaviourEventInvoker>().OnDisabled -= OnPlantDisabled;
        }
        bluePlantsInRoom.Clear();
    }

    private void OnPlantDisabled(GameObject plant) {
        plant.GetComponent<MonoBehaviourEventInvoker>().OnDisabled -= OnPlantDisabled;
        bluePlantsInRoom.Remove(plant.transform);

        if (swarmMovement.IsLeader) {
            if (bluePlantsInRoom.Count > 0) {
                ChangeSwarmState(SwarmState.MovingToPlant);
            }
            else {
                ChangeSwarmState(SwarmState.Wandering);
            }
        }
    }

    protected override void Update() {
        base.Update();

        if (swarmMovement.IsLeader && beeState != BeeState.Launching) {
            HandleControllingSwarm();
        }

        HandleShooting();
        HandleLaunch();
    }

    private void HandleControllingSwarm() {
        switch (swarmState) {
            case SwarmState.MovingToPlant:

                if (bluePlantsInRoom.Count == 0) {
                    ChangeSwarmState(SwarmState.Wandering);
                    return;
                }

                reproducingPlant = GetClosestPlant();
                bool anyBeeNearPlant = swarmMovement.GetUnitsInSwarm().Any(u => Vector2.SqrMagnitude(u.transform.position - reproducingPlant.position) < 0.5f);
                if (anyBeeNearPlant) {
                    ChangeSwarmState(SwarmState.Reproducing);
                }

                break;

            case SwarmState.Wandering:

                if (bluePlantsInRoom.Count > 0) {
                    ChangeSwarmState(SwarmState.MovingToPlant);
                    return;
                }

                if (fleeDistance >= chaseDistance) {
                    Debug.LogWarning("Chase distance must be greater than flee distance!");
                }

                HandleWanderMovement();

                break;

            case SwarmState.Reproducing:

                if (bluePlantsInRoom.Count == 0) {
                    ChangeSwarmState(SwarmState.Wandering);
                    return;
                }

                if (reproducingPlant == null || !reproducingPlant.gameObject.activeSelf) {
                    ChangeSwarmState(SwarmState.MovingToPlant);
                    return;
                }

                reproduceTimer -= Time.deltaTime;
                if (reproduceTimer < 0f) {
                    scriptableEnemy.Prefab.Spawn(reproducingPlant.transform.position, Containers.Instance.Enemies);

                    //... this will indirectly invoke OnPlantDisabled
                    reproducingPlant.GetComponent<BreakOnDamaged>().Damage(0f);
                }

                break;
        }
    }

    private void ChangeSwarmState(SwarmState swarmState) {

        if (!swarmMovement.IsLeader) {
            Debug.LogError("Trying to change swarm state, but not through leader!");
            return;
        }

        if (beeState == BeeState.Launching) {
            return;
        }

        Bee[] beesInSwarm = swarmMovement.GetUnitsInSwarm().Select(u => u.GetComponent<Bee>()).ToArray();
        foreach (Bee bee in beesInSwarm) {
            bee.swarmState = swarmState;
        }

        switch (swarmState) {
            case SwarmState.MovingToPlant:
                foreach (Bee bee in beesInSwarm) {
                    bee.IncreaseAgentAvoidance();
                }

                //... delay to wait for plant to get destroyed and new bee to join swarm
                StartCoroutine(DelayedMoveToNearestPlant());

                break;

            case SwarmState.Wandering:
                foreach (Bee bee in beesInSwarm) {
                    bee.IncreaseAgentAvoidance();
                }

                swarmMovement.StopAtCurrentPositions();

                break;

            case SwarmState.Reproducing:
                foreach (Bee bee in beesInSwarm) {
                    bee.DecreaseAgentAvoidance();
                }

                reproduceTimer = reproduceTime;

                reproducingPlant = GetClosestPlant();

                swarmMovement.Shuffle(reproducingPlant.position);

                break;
        }
    }

    private IEnumerator DelayedMoveToNearestPlant() {

        float delay = 0.1f;
        yield return new WaitForSeconds(delay);

        if (swarmMovement.IsLeader) {
            reproducingPlant = GetClosestPlant();

            swarmMovement.SetSwarmDestination(reproducingPlant.position);
        }
    }

    private Transform GetClosestPlant() {
        float minDistanceSquared = float.MaxValue;
        Transform closestPlant = null;
        foreach (Transform plant in bluePlantsInRoom) {

            if (!plant.gameObject.activeSelf) {
                Debug.LogError("Inactive plant is in list!");
                return null;
            }

            float xDiff = transform.position.x - plant.position.x;
            float yDiff = transform.position.y - plant.position.y;
            float distanceSquared = (xDiff * xDiff) + (yDiff * yDiff);
            if (distanceSquared < minDistanceSquared) {
                minDistanceSquared = distanceSquared;
                closestPlant = plant;
            }
        }

        if (closestPlant == null) {
            Debug.LogError("Could not find closest plant!");
        }

        return closestPlant;
    }

    private void DecreaseAgentAvoidance() {
        float largeAvoidanceOffset = 0.05f;
        agent.baseOffset = largeAvoidanceOffset;

        float largeAvoidanceRadius = 0.05f;
        agent.radius = largeAvoidanceRadius;

        float largeAvoidanceHeight = 0.05f;
        agent.height = largeAvoidanceHeight;
    }

    private void IncreaseAgentAvoidance() {
        float largeAvoidanceOffset = 0;
        agent.baseOffset = largeAvoidanceOffset;

        float largeAvoidanceRadius = 0.4f;
        agent.radius = largeAvoidanceRadius;

        float largeAvoidanceHeight = 0.5f;
        agent.height = largeAvoidanceHeight;
    }

    #region Wander

    [Header("Wander")]
    [SerializeField] private float fleeDistance;
    [SerializeField] private float chaseDistance;

    private void HandleWanderMovement() {

        if (!swarmMovement.IsLeader) {
            Debug.LogError("Trying to set wander destination, but not through leader!");
            return;
        }

        float playerDistanceSquared = Vector2.SqrMagnitude(PlayerMovement.Instance.CenterPos - transform.position);
        bool currentlyCloseToPlayer = playerDistanceSquared < fleeDistance * fleeDistance;
        bool currentlyFarFromPlayer = playerDistanceSquared > chaseDistance * chaseDistance;

        float moveDirectionVariation = 45f;
        float randomMoveDistance = Random.Range(1f, 2f);

        if (currentlyCloseToPlayer) {
            if (swarmMovement.AnyUnitNearSwarmDest(distanceThreshold: 1f) || !swarmMovement.SwarmDestinationSet) {
                Vector2 fromPlayerDirection = (transform.position - PlayerMovement.Instance.CenterPos).normalized;

                float randomFleeDegrees = Random.Range(-moveDirectionVariation, moveDirectionVariation);
                fromPlayerDirection.RotateDirection(randomFleeDegrees);

                Vector2 targetPos = (Vector2)transform.position + fromPlayerDirection * randomMoveDistance;
                swarmMovement.SetSwarmDestination(targetPos);
            }
        }
        else if (currentlyFarFromPlayer) {
            if (swarmMovement.AnyUnitNearSwarmDest(distanceThreshold: 1f) || !swarmMovement.SwarmDestinationSet) {
                Vector2 toPlayerDirection = (PlayerMovement.Instance.CenterPos - transform.position).normalized;

                float randomChaseDegrees = Random.Range(-moveDirectionVariation, moveDirectionVariation);
                toPlayerDirection.RotateDirection(randomChaseDegrees);

                Vector2 targetPos = (Vector2)transform.position + toPlayerDirection * randomMoveDistance;
                swarmMovement.SetSwarmDestination(targetPos);
            }
        }
        else {
            swarmMovement.SetSwarmDestination(wanderMovement.TargetDestination);
        }

    }

    #endregion

    #region Shoot Stinger

    [Header("Shoot")]
    [SerializeField] private float chanceToShootPerSecond;
    [SerializeField] private StraightMovement stingerPrefab;
    [SerializeField] private Transform shootPoint;

    private Vector2 shootDirection;

    [SerializeField] private float delayBeforeShoot;
    [SerializeField] private float delayAfterShoot;
    private float beforeShootTimer;
    private float afterShootTimer;

    private StopMovement shootStopMovement;

    private void HandleShooting() {

        if (beeState == BeeState.FollowingSwarmBehavior) {
            bool shoot = UnityEngine.Random.value < chanceToShootPerSecond * Time.deltaTime;
            if (shoot) {
                shootStopMovement = gameObject.AddComponent<StopMovement>();

                beforeShootTimer = delayBeforeShoot;

                bool playerToRight = PlayerMovement.Instance.CenterPos.x > transform.position.x;
                faceBehavior.ForceFace(playerToRight);

                beeState = BeeState.ShootingStinger;
            }
        }
        else if (beeState == BeeState.ShootingStinger) {

            beforeShootTimer -= Time.deltaTime;
            if (beforeShootTimer < 0f) {
                anim.SetTrigger("attack");
                shootDirection = PlayerMovement.Instance.CenterPos - transform.position;

                beforeShootTimer = float.PositiveInfinity;
            }

            afterShootTimer -= Time.deltaTime;
            if (afterShootTimer < 0f) {
                Destroy(shootStopMovement);

                faceBehavior.StopForcing();

                afterShootTimer = float.PositiveInfinity;

                beeState = BeeState.FollowingSwarmBehavior;
            }
        }
    }

    // played by anim method invoker
    public void ShootProjectile() {
        StraightMovement newProjectile = stingerPrefab.Spawn(shootPoint.position, Containers.Instance.Projectiles);
        newProjectile.Setup(shootDirection.normalized);
        newProjectile.GetComponent<DamageOnContact>().Setup(GetEnemyStats().Damage, GetEnemyStats().KnockbackStrength);

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BasicEnemyShoot);

        afterShootTimer = delayAfterShoot;
    }

    #endregion


    #region Launch
    [SerializeField] private float chanceToLaunchPerSecond;

    [SerializeField] private float launchAcceleration;
    [SerializeField] private float launchSpeed;

    private Vector2 launchDirection;

    private void HandleLaunch() {

        if (beeState == BeeState.FollowingSwarmBehavior) {
            if (Random.value < chanceToLaunchPerSecond * Time.deltaTime) {
                swarmMovement.enabled = false;
                agent.enabled = false;

                Vector2 toPlayerDirection = (PlayerMovement.Instance.CenterPos - transform.position).normalized;
                launchDirection = toPlayerDirection;

                bool playerToRight = PlayerMovement.Instance.CenterPos.x > transform.position.x;
                faceBehavior.ForceFace(playerToRight);

                float yAngle = playerToRight ? 0f : 180f;

                float toPlayerRotation = toPlayerDirection.DirectionToRotation().eulerAngles.z + 50f;

                float launchRotation = toPlayerRotation;
                if (!playerToRight) {
                    launchRotation = 310f - toPlayerRotation;
                }

                transform.rotation = Quaternion.Euler(
                    transform.rotation.eulerAngles.x,
                    yAngle,
                    launchRotation);


                anim.SetBool("launching", true);

                beeState = BeeState.Launching;
            }
        }
        else if (beeState == BeeState.Launching) {
            rb.velocity = Vector2.MoveTowards(rb.velocity, launchDirection * launchSpeed, launchAcceleration * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (beeState == BeeState.Launching && GameLayers.ObstacleLayerMask.ContainsLayer(collision.gameObject.layer)) {
            GetComponent<DropEssenceOnDeath>().IsEnabled = false; // sets to enabled when bee is enabled (in enemy script)
            health.Die();
        }
    }

    #endregion
}
