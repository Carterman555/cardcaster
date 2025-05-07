using MoreMountains.Tools;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

// Script execution order reason: needs invoke ChangeSwarmState in onEnable after swarmMovementBehavior sets itself to leader in onEnable
public class Bee : Enemy {

    private SwarmMovementBehavior swarmMovement;
    private Rigidbody2D rb;
    private NavMeshAgent agent;

    // TODO - remove flowers from list that get destroyed
    private List<Transform> bluePlantsInRoom;

    private Transform reproducingPlant;

    [SerializeField] private float reproduceTime;
    private float reproduceTimer;

    private bool debugSetBluePlants; // testing

    private enum SwarmState { MovingToPlant, Wandering, Reproducing }
    private SwarmState swarmState; // all bees in a swarm always have the same swarmState

    private enum BeeState { FollowingSwarmBehavior, ShootingStinger, Launching }
    private BeeState beeState;

    protected override void Awake() {
        base.Awake();

        swarmMovement = GetComponent<SwarmMovementBehavior>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        if (Room.GetCurrentRoom() == null) {
            Debug.LogWarning("Trying to find blue plants, but no room is set!");
            return;
        }

        Transform[] allObjectsInRoom = Room.GetCurrentRoom().GetComponentsInChildren<Transform>();
        bluePlantsInRoom = allObjectsInRoom.Where(g => g.CompareTag("BluePlant")).ToList();

        foreach (Transform plant in bluePlantsInRoom) {
            plant.GetComponent<MonoBehaviourEventInvoker>().OnDisabled += OnPlantDisabled;
        }

        reproduceTimer = float.PositiveInfinity;

        shootTimer = EnemyStats.AttackCooldown + UnityEngine.Random.Range(-shootCooldownRandomVariation, shootCooldownRandomVariation);
        attemptLaunchTimer = attemptLaunchCooldown;

        anim.SetBool("Launching", false);

        ChangeSwarmState(SwarmState.MovingToPlant);
    }

    private void OnPlantDisabled(GameObject plant) {
        plant.GetComponent<MonoBehaviourEventInvoker>().OnDisabled -= OnPlantDisabled;
        bluePlantsInRoom.Remove(plant.transform);
    }

    protected override void Update() {
        base.Update();

        // testing
        if (!debugSetBluePlants && Room.GetCurrentRoom() != null) {
            Transform[] allObjectsInRoom = Room.GetCurrentRoom().GetComponentsInChildren<Transform>();
            bluePlantsInRoom = allObjectsInRoom.Where(g => g.CompareTag("BluePlant")).ToList();

            foreach (Transform plant in bluePlantsInRoom) {
                plant.GetComponent<MonoBehaviourEventInvoker>().OnDisabled += OnPlantDisabled;
            }

            debugSetBluePlants = true;
        }

        if (!debugSetBluePlants) {
            return;
        }
        // end testing


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

                if (!swarmMovement.IsSwarmMoving()) {
                    reproducingPlant = GetClosestPlant();
                    bool atBluePlant = Vector2.Distance(transform.position, reproducingPlant.position) < 0.1f;
                    if (atBluePlant) {
                        ChangeSwarmState(SwarmState.Reproducing);
                    }
                    else {
                        swarmMovement.SetSwarmDestination(reproducingPlant.position);
                    }
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

                    swarmMovement.StopAndSwarmAroundLeader();

                    reproducingPlant = GetClosestPlant();

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
                break;

            case SwarmState.Wandering:
                break;

            case SwarmState.Reproducing:
                reproduceTimer = reproduceTime;
                swarmMovement.StopAndShuffle();
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
            Debug.Log("Could not find closest plant!");
        }

        return closestPlant;
    }

    #region Shoot Stinger

    private float shootTimer;
    [SerializeField] private float shootCooldownRandomVariation;
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
                shootStopMovement = gameObject.AddComponent<StopMovement>();

                shootTimer = EnemyStats.AttackCooldown + UnityEngine.Random.Range(-shootCooldownRandomVariation, shootCooldownRandomVariation);
                beforeShootTimer = delayBeforeShoot;

                bool playerToRight = PlayerMovement.Instance.CenterPos.x > transform.position.x;
                float yAngle = playerToRight ? 0f : 180f;
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, yAngle, transform.rotation.eulerAngles.z));

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

    [SerializeField] private float attemptLaunchCooldown;
    private float attemptLaunchTimer;

    [SerializeField, Range(0f, 1f)] private float launchChance;

    [SerializeField] private float launchSpeed;

    private Vector2 launchDirection;

    private void HandleLaunch() {

        if (beeState == BeeState.FollowingSwarmBehavior) {

            attemptLaunchTimer -= Time.deltaTime;
            if (attemptLaunchTimer < 0f) {
                attemptLaunchTimer = attemptLaunchCooldown;

                if (UnityEngine.Random.value < launchChance) {
                    print("Launch");

                    anim.SetBool("Launching", true);
                    swarmMovement.enabled = false;
                    agent.enabled = false;

                    launchDirection = transform.position - PlayerMovement.Instance.CenterPos;

                    beeState = BeeState.Launching;

                }
            }
        }
        else if (beeState == BeeState.Launching) {
            Vector2 movement = launchDirection * launchSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }

    #endregion
}
