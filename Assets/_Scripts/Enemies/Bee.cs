using QFSW.QC;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.WSA;

// Script execution order reason: needs invoke ChangeSwarmState in onEnable after swarmMovementBehavior sets itself to leader in onEnable
public class Bee : Enemy {

    private SwarmMovementBehavior swarmMovement;
    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private FaceMoveDirectionBehavior faceBehavior;

    // TODO - remove flowers from list that get destroyed
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

        swarmMovement = GetComponent<SwarmMovementBehavior>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        faceBehavior = GetComponent<FaceMoveDirectionBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();

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

        shootTimer = shootCooldown.Randomize() * EnemyStats.AttackCooldown;
        launchTimer = attemptLaunchCooldown.Randomize();

        anim.SetBool("launching", false);

        ChangeSwarmState(SwarmState.MovingToPlant);
    }

    private void OnPlantDisabled(GameObject plant) {
        plant.GetComponent<MonoBehaviourEventInvoker>().OnDisabled -= OnPlantDisabled;
        bluePlantsInRoom.Remove(plant.transform);
    }

    protected override void Update() {
        base.Update();

        if (swarmMovement.IsLeader) {
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
                else if (!swarmMovement.IsSwarmMoving()) {
                    swarmMovement.SetSwarmDestination(reproducingPlant.position);
                }

                break;

            case SwarmState.Wandering:
                // TODO
                break;

            case SwarmState.Reproducing:

                if (bluePlantsInRoom.Count == 0) {
                    ChangeSwarmState(SwarmState.Wandering);
                    return;
                }

                if (reproducingPlant == null) {
                    ChangeSwarmState(SwarmState.MovingToPlant);
                    return;
                }

                reproduceTimer -= Time.deltaTime;
                if (reproduceTimer < 0f) {
                    scriptableEnemy.Prefab.Spawn(reproducingPlant.transform.position);
                    reproducingPlant.GetComponent<BreakOnDamaged>().Damage(0f);

                    reproducingPlant = GetClosestPlant();
                    swarmMovement.SetSwarmDestination(reproducingPlant.position);

                    ChangeSwarmState(SwarmState.MovingToPlant);
                }

                break;
        }
    }

    private void ChangeSwarmState(SwarmState swarmState) {

        if (!swarmMovement.IsLeader) {
            Debug.LogError("Trying to change swarm state, but not through leader!");
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
                break;

            case SwarmState.Wandering:
                foreach (Bee bee in beesInSwarm) {
                    bee.IncreaseAgentAvoidance();
                }
                break;

            case SwarmState.Reproducing:
                reproduceTimer = reproduceTime;

                reproducingPlant = GetClosestPlant();
                swarmMovement.Shuffle(reproducingPlant.position);
                foreach (Bee bee in beesInSwarm) {
                    bee.DecreaseAgentAvoidance();
                }
                break;
        }
    }

    private Transform GetClosestPlant() {
        float minDistanceSquared = float.MaxValue;
        Transform closestPlant = null;
        foreach (Transform plant in bluePlantsInRoom) {

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

    #region Shoot Stinger

    private float shootTimer;
    [SerializeField] private RandomFloat shootCooldown;
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
            shootTimer -= Time.deltaTime;
            if (shootTimer < 0f) {
                shootTimer = shootCooldown.Randomize() * EnemyStats.AttackCooldown;

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
        newProjectile.GetComponent<DamageOnContact>().Setup(EnemyStats.Damage, EnemyStats.KnockbackStrength);

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BasicEnemyShoot);

        afterShootTimer = delayAfterShoot;
    }

    #endregion


    #region Launch

    [SerializeField] private RandomFloat attemptLaunchCooldown;
    private float launchTimer;

    [SerializeField] private float launchAcceleration;
    [SerializeField] private float launchSpeed;

    private Vector2 launchDirection;

    [Command]
    private void LaunchLeader() {
        if (swarmMovement.Leader) {
            launchTimer = 0;
        }
    }

    private void HandleLaunch() {

        if (beeState == BeeState.FollowingSwarmBehavior) {

            launchTimer -= Time.deltaTime;
            if (launchTimer < 0f) {
                launchTimer = attemptLaunchCooldown.Randomize();

                anim.SetBool("launching", true);

                swarmMovement.enabled = false;
                agent.enabled = false;

                Vector2 toPlayerDirection = (PlayerMovement.Instance.CenterPos - transform.position).normalized;
                launchDirection = toPlayerDirection;

                bool playerToRight = PlayerMovement.Instance.CenterPos.x > transform.position.x;
                faceBehavior.ForceFace(playerToRight);

                float yAngle = playerToRight ? 0f : 180f;

                bool playerBelowBee = PlayerMovement.Instance.CenterPos.y < transform.position.y;
                float toPlayerAngle = toPlayerDirection.DirectionToRotation().eulerAngles.z + 50f;

                float launchAngle = toPlayerAngle;
                if (!playerToRight) {
                    launchAngle = 310f - toPlayerAngle;
                }

                transform.rotation = Quaternion.Euler(
                    transform.rotation.eulerAngles.x,
                    yAngle,
                    launchAngle);

                beeState = BeeState.Launching;
            }
        }
        else if (beeState == BeeState.Launching) {
            rb.velocity = Vector2.MoveTowards(rb.velocity, launchDirection * launchSpeed, launchAcceleration * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == GameLayers.WallLayer) {
            GetComponent<DropEssenceOnDeath>().IsEnabled = false;
            health.Die();
        }
    }

    #endregion
}
