using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Enemy : MonoBehaviour {

    [SerializeField] private ScriptableEnemy scriptableEnemy;

    [SerializeField] private ScriptableEnemyState nearState;
    [SerializeField] private ScriptableEnemyState farState;

    private ScriptableEnemyState nearStateInstance;
    private ScriptableEnemyState farStateInstance;

    private ScriptableEnemyState currentState;

    [SerializeField] private TriggerContactTracker playerTracker;

    private void OnEnable() {
        SubToChangeState();

        InitializeStates();
    }
    private void OnDisable() {
        UnsubToChangeState();
    }

    private void InitializeStates() {
        nearStateInstance = Instantiate(nearState);
        nearStateInstance.Initialize(this);

        farStateInstance = Instantiate(nearState);
        farStateInstance.Initialize(this);
    }

    private void Update() {
        currentState.FrameUpdate();
    }

    private void FixedUpdate() {
        currentState.PhysicsUpdate();
    }

    #region Change State

    private void SubToChangeState() {
        playerTracker.OnEnterContact += SetNearState;
        playerTracker.OnLeaveContact += SetFarState;
    }

    private void UnsubToChangeState() {
        playerTracker.OnEnterContact -= SetNearState;
        playerTracker.OnLeaveContact -= SetFarState;
    }

    private void SetNearState(GameObject @object) {
        ChangeState(nearStateInstance);
    }
    private void SetFarState(GameObject @object) {
        ChangeState(farStateInstance);
    }

    private void ChangeState(ScriptableEnemyState newState) {
        currentState.DoExitLogic();
        currentState = newState;
        currentState.DoEnterLogic();
    }

    #endregion

    private void AnimationTriggerEvent(AnimationTriggerType triggerType) {
        currentState.DoAnimationTriggerEventLogic(triggerType);
    }
}

public enum AnimationTriggerType {
    Die,
    MeleeAttack,
    RangedAttack,
    Pickup,
}