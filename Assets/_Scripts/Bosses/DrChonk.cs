using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrChonk : MonoBehaviour {

    private DrChonkState currentState;
    private float stateTimer;

    private readonly DrChonkState[] activeStates = new DrChonkState[] { DrChonkState.EatMinions, DrChonkState.Roll, DrChonkState.ShootMinions };

    [SerializeField] private Dictionary<DrChonkState, RandomFloat> stateDurations;


    private void OnEnable() {
        stateTimer = 0f;

        SubEatMinionMethods();

        // spawn 5 healer minions surrounding boss
        SpawnStartingMinions();
    }
    private void OnDisable() {
        UnsubEatMinionMethods();
    }

    private void Update() {

        stateTimer += Time.deltaTime;
        if (stateTimer > stateDurations[currentState].Value) {


            if (currentState == DrChonkState.BetweenStates) {
                ChangeToRandomState();
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

        }
        else if (previousState == DrChonkState.Roll) {

        }
        else if (previousState == DrChonkState.ShootMinions) {

        }

        if (newState == DrChonkState.BetweenStates) {

        }
        else if (newState == DrChonkState.EatMinions) {
            // trigger open mouth and suck animation
        }
        else if (newState == DrChonkState.Roll) {

        }
        else if (newState == DrChonkState.ShootMinions) {

        }
    }

    [SerializeField] private GameObject healerMinionPrefab;

    private void SpawnStartingMinions() {

        int startingMinionAmount = 5;
        float distanceFromBoss = 2f;

        Vector2 spawnDirection = Vector2.up;
        float rotationBetweenMinions = 360f / startingMinionAmount;

        for (int i = 0; i < startingMinionAmount; i++) {

            spawnDirection.RotateDirection(rotationBetweenMinions);

            Vector2 pos = (Vector2)transform.position + (spawnDirection * distanceFromBoss);
            healerMinionPrefab.Spawn(pos, Containers.Instance.Enemies);
        }
    }

    #region Eating Minions

    [SerializeField] private TriggerContactTracker suckMinionTrigger;
    [SerializeField] private TriggerContactTracker eatMinionTrigger;

    private void SubEatMinionMethods() {
        suckMinionTrigger.OnEnterContact += TrySuckMinion;
        eatMinionTrigger.OnEnterContact += TryEatMinion;
    }

    private void UnsubEatMinionMethods() {
        suckMinionTrigger.OnEnterContact -= TrySuckMinion;
        eatMinionTrigger.OnEnterContact -= TryEatMinion;
    }

    private void TrySuckMinion(GameObject minion) {
        if (currentState == DrChonkState.EatMinions) {
            // suck in minion
        }
    }

    public void TryEatMinion(GameObject minion) {
        if (currentState == DrChonkState.EatMinions) {
            // heal effect on boss
            // heal boss
        }
    }

    #endregion

    #region Roll

    #endregion


    #region Shooting Minions

    #endregion
}

public enum DrChonkState {
    BetweenStates = 0,
    EatMinions = 1,
    Roll = 2,
    ShootMinions = 3
}
