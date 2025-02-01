using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThunderGolem : MonoBehaviour, IHasStats, IBoss {

    [SerializeField] private ScriptableBoss scriptableBoss;
    public Stats GetStats() {
        return scriptableBoss.Stats;
    }

    private GolemState currentState;
    private GolemState previousActionState;

    private readonly GolemState[] actionStates = new GolemState[] { GolemState.Spin, GolemState.Split, GolemState.CardSurge };

    [SerializeField] private List<GolemStateDurationPair> stateDurationsList;
    private Dictionary<GolemState, RandomFloat> stateDurations = new();
    private float stateTimer;

    [SerializeField] private Animator anim;

    private Health health;

    [SerializeField] private bool debugState;
    [ConditionalHide("debugState")][SerializeField] private GolemState stateToDebug;

    private void Awake() {
        InitializeDurationDict();

        health = GetComponent<Health>();
    }

    private void InitializeDurationDict() {
        foreach (var stateDuration in stateDurationsList) {
            stateDurations.Add(stateDuration.State, stateDuration.Duration);
        }
    }

    private void OnEnable() {
        ChangeState(GolemState.BetweenStates);

        stateTimer = 0f;

        health.OnDeath += OnDeath;
    }

    private void OnDisable() {
        health.OnDeath -= OnDeath;
    }

    private void Update() {

        stateTimer += Time.deltaTime;
        if (stateTimer > stateDurations[currentState].Value) {

            if (currentState == GolemState.BetweenStates) {
                if (!debugState) {
                    ChangeToRandomState(previousActionState);
                }
                else {
                    ChangeState(stateToDebug);
                }
            }
            else {
                ChangeState(GolemState.BetweenStates);
            }
        }
    }

    private void ChangeToRandomState(GolemState stateToAvoid) {
        GolemState[] availableStates = actionStates.Where(s => s != stateToAvoid).ToArray();
        GolemState newState = availableStates.RandomItem();
        ChangeState(newState);
    }

    private void OnDeath() {
        StartCoroutine(OnDeathCor());
    }
    private IEnumerator OnDeathCor() {
        ChangeState(GolemState.BetweenStates);

        float delay = 1f;
        yield return new WaitForSeconds(delay);

        GetComponent<DeathParticles>().GenerateParticles();

        gameObject.ReturnToPool();
    }

    private void ChangeState(GolemState newState) {

        GolemState previousState = currentState;
        currentState = newState;

        if (actionStates.Contains(previousState)) {
            previousActionState = previousState;
        }

        stateTimer = 0;
        stateDurations[newState].Randomize();

        if (previousState == GolemState.BetweenStates) {
        }
        else if (previousState == GolemState.Spin) {
        }
        else if (previousState == GolemState.Split) {
        }
        else if (previousState == GolemState.CardSurge) {
        }

        if (newState == GolemState.BetweenStates) {
        }
        else if (newState == GolemState.Spin) {
        }
        else if (newState == GolemState.Split) {
        }
        else if (newState == GolemState.CardSurge) {
        }
    }
}

[Serializable]
public enum GolemState {
    BetweenStates = 0,
    Spin = 1,
    Split = 2,
    CardSurge = 3
}

[Serializable]
public class GolemStateDurationPair {
    public GolemState State;
    public RandomFloat Duration;
}
