using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrChonk : MonoBehaviour, IHasStats, IBoss {

    private DrChonkState currentState;
    private float stateTimer;

    private readonly DrChonkState[] activeStates = new DrChonkState[] { DrChonkState.EatMinions, DrChonkState.Roll, DrChonkState.ShootMinions };

    [SerializeField] private List<DrChunkStateDurationPair> stateDurationsList;
    private Dictionary<DrChonkState, RandomFloat> stateDurations = new();

    [SerializeField] private Transform centerPoint;

    [SerializeField] private ScriptableBoss scriptableBoss;
    public Stats GetStats() {
        return scriptableBoss.Stats;
    }

    [SerializeField] private Animator anim;

    private void Awake() {
        health = GetComponent<Health>();
        bounceMoveBehaviour = GetComponent<BounceMoveBehaviour>();
        straightShootBehavior = GetComponent<StraightShootBehavior>();

        InitializeDurationDict();
    }

    private void InitializeDurationDict() {
        foreach (var stateDuration in stateDurationsList) {
            stateDurations.Add(stateDuration.State, stateDuration.Duration);
        }
    }

    private void OnEnable() {

        ChangeState(DrChonkState.BetweenStates);

        stateTimer = 0f;

        bounceMoveBehaviour.enabled = false;
        straightShootBehavior.enabled = false;

        SubEatMinionMethods();

        // spawn 5 healer minions surrounding boss
        SpawnStartingMinions();
    }
    private void OnDisable() {
        UnsubEatMinionMethods();

        OnDefeated();
    }

    private void Update() {

        stateTimer += Time.deltaTime;
        if (stateTimer > stateDurations[currentState].Value) {


            if (currentState == DrChonkState.BetweenStates) {
                ChangeToRandomState();
                //ChangeState(DrChonkState.Roll);
            }
            else {
                ChangeState(DrChonkState.BetweenStates);
            }
        }

        if (currentState == DrChonkState.BetweenStates) {
        }
        else if (currentState == DrChonkState.EatMinions) {
        }
        else if (currentState == DrChonkState.Roll) {
        }
        else if (currentState == DrChonkState.ShootMinions) {

        }
    }

    private void ChangeToRandomState() {
        DrChonkState newState = activeStates.RandomItem();
        ChangeState(newState);
    }

    private void ChangeState(DrChonkState newState) {

        DrChonkState previousState = currentState;
        currentState = newState;

        stateTimer = 0;
        stateDurations[newState].Randomize();

        if (previousState == DrChonkState.BetweenStates) {

        }
        else if (previousState == DrChonkState.EatMinions) {

            // close mouth
            anim.SetBool("mouthOpen", false);
            suckEffect.SetActive(false);
        }
        else if (previousState == DrChonkState.Roll) {
            bounceMoveBehaviour.enabled = false;

            // stop rolling animation
            anim.SetBool("rolling", false);
        }
        else if (previousState == DrChonkState.ShootMinions) {
            straightShootBehavior.enabled = false;

            // close mouth
            anim.SetBool("mouthOpen", false);
        }

        if (newState == DrChonkState.BetweenStates) {

        }
        else if (newState == DrChonkState.EatMinions) {
            // open mouth
            anim.SetBool("mouthOpen", true);
            suckEffect.SetActive(true);
        }
        else if (newState == DrChonkState.Roll) {
            bounceMoveBehaviour.enabled = true;

            // start rolling animation
            anim.SetBool("rolling", true);
        }
        else if (newState == DrChonkState.ShootMinions) {
            straightShootBehavior.enabled = true;

            // open mouth
            anim.SetBool("mouthOpen", true);
        }
    }

    [Header("Spawn Starting Healers")]
    [SerializeField] private GameObject healerMinionPrefab;
    [SerializeField] private float startingHealersDistance = 2f;

    private void SpawnStartingMinions() {

        int startingMinionAmount = 5;

        Vector2 spawnDirection = Vector2.up;
        float rotationBetweenMinions = 360f / startingMinionAmount;

        for (int i = 0; i < startingMinionAmount; i++) {

            spawnDirection = spawnDirection.RotateDirection(rotationBetweenMinions);

            Vector2 pos = (Vector2)centerPoint.position + (spawnDirection * startingHealersDistance);
            healerMinionPrefab.Spawn(pos, Containers.Instance.Enemies);
        }
    }

    #region Eating Minions

    [Header("Eat Minions")]
    [SerializeField] private TriggerContactTracker suckMinionTrigger;
    [SerializeField] private TriggerContactTracker eatMinionTrigger;

    [SerializeField] private Transform suckCenter;

    [SerializeField] private float eatMinionHealAmount;

    [SerializeField] private GameObject suckEffect;
    [SerializeField] private ParticleSystem healEffectPrefab;

    private Health health;

    private void SubEatMinionMethods() {
        suckMinionTrigger.OnEnterContact += TrySuckMinion;
        eatMinionTrigger.OnEnterContact += TryEatMinion;
    }

    private void UnsubEatMinionMethods() {
        suckMinionTrigger.OnEnterContact -= TrySuckMinion;
        eatMinionTrigger.OnEnterContact -= TryEatMinion;
    }

    private void TrySuckMinion(GameObject collisionObject) {
        if (currentState == DrChonkState.EatMinions) {
            if (collisionObject.TryGetComponent(out HealerMinion healerMinion)) {
                // suck in minion
                healerMinion.SuckToChonk(suckCenter.position);
            }
        }
    }

    public void TryEatMinion(GameObject collisionObject) {
        if (currentState == DrChonkState.EatMinions) {
            if (collisionObject.TryGetComponent(out HealerMinion healerMinion)) {
                // eat minion
                collisionObject.ReturnToPool();

                // heal
                health.Heal(eatMinionHealAmount);

                // heal effect
                healEffectPrefab.Spawn(centerPoint.position, Containers.Instance.Effects);
            }
        }
    }

    #endregion

    #region Roll

    private BounceMoveBehaviour bounceMoveBehaviour;

    #endregion

    
    #region Shooting Minions

    private StraightShootBehavior straightShootBehavior;

    #endregion

    private void OnDefeated() {
        // kill all enemies
        foreach (Transform enemy in Containers.Instance.Enemies) {
            enemy.GetComponent<Health>().Die();
        }
    }
}

[Serializable]
public enum DrChonkState {
    BetweenStates = 0,
    EatMinions = 1,
    Roll = 2,
    ShootMinions = 3
}

[Serializable]
public class DrChunkStateDurationPair {
    public DrChonkState State;
    public RandomFloat Duration;
}

