using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Bee : Enemy {

    private SwarmMovementBehavior swarmMovement;

    // TODO - remove flowers from list that get destroyed
    private List<Transform> bluePlantsInRoom;

    private Transform reproducingPlant;

    [SerializeField] private float reproduceTime;
    private float reproduceTimer;

    private float shootTimer;
    [SerializeField] private float shootCooldownRandomVariation;

    private bool debugSetBluePlants; // testing

    protected override void Awake() {
        base.Awake();

        swarmMovement = GetComponent<SwarmMovementBehavior>();
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

        HandleMovementAndReproduction();

        shootTimer -= Time.deltaTime;
        if (shootTimer < 0f) {
            shootTimer = EnemyStats.AttackCooldown + UnityEngine.Random.Range(-shootCooldownRandomVariation, shootCooldownRandomVariation);
            anim.SetTrigger("attack");
        }
    }

    private void HandleMovementAndReproduction() {
        if (swarmMovement.IsLeader && bluePlantsInRoom.Count > 0) {

            if (!swarmMovement.IsSwarmMoving()) {
                reproducingPlant = GetClosestPlant();
                bool atBluePlant = Vector2.Distance(transform.position, reproducingPlant.position) < 0.1f;
                if (!atBluePlant) {
                    swarmMovement.SetSwarmDestination(reproducingPlant.position);
                }
                else if (!swarmMovement.Shuffling) {
                    swarmMovement.StopAndShuffle();
                }
            }

            if (swarmMovement.Shuffling) {
                if (reproducingPlant == null) {
                    reproducingPlant = GetClosestPlant();
                    reproduceTimer = float.PositiveInfinity;
                    print("plant not active, find new plant");
                    return;
                }

                bool timerSet = reproduceTimer != float.PositiveInfinity;
                if (!timerSet) {
                    reproduceTimer = reproduceTime;
                    print("Set timer");
                }

                reproduceTimer -= Time.deltaTime;
                if (reproduceTimer < 0f) {
                    scriptableEnemy.Prefab.Spawn(reproducingPlant.transform.position);
                    reproducingPlant.GetComponent<BreakOnDamaged>().Damage(0f);

                    swarmMovement.StopAndSwarmAroundLeader();


                    reproducingPlant = GetClosestPlant();
                    reproduceTimer = float.PositiveInfinity;
                    print("Broke plant");
                }
            }
            else {
                reproduceTimer = float.PositiveInfinity;
            }
        }
    }

    // played by anim method invoker, well not yet (unless I forgot to remove this)
    public void ShootProjectile() {
        StraightMovement newProjectile = projectilePrefab.Spawn(shootPoint.position, Containers.Instance.Projectiles);

        if (!stopAimingOnAnimStart) {
            shootDirection = GetShootDirection();
        }

        if (hasShootVariation) {
            float randomAngle = UnityEngine.Random.Range(-shootVariation, shootVariation);
            shootDirection.RotateDirection(randomAngle);
        }

        newProjectile.Setup(shootDirection.normalized);

        float dmg = overrideDamage ? damage : hasStats.EnemyStats.Damage;
        newProjectile.GetComponent<DamageOnContact>().Setup(dmg, hasStats.EnemyStats.KnockbackStrength);
        PlaySFX();
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
}
