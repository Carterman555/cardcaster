using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum RealDealerState {
    StartingDialog,
    RealRevealDialog,
    FakeDeathDialog,
    RealDeathDialog,
    BetweenStates,
    Swing,
    Lasers,
    Smashers
}

[Serializable]
public class RealDealerDurationPair {
    public RealDealerState State;
    public RandomFloat Duration;
}

public class TheRealDealer : MonoBehaviour, IHasEnemyStats, IBoss {


    [SerializeField] private ScriptableBoss scriptableBoss;
    public EnemyStats EnemyStats => scriptableBoss.Stats;

    private RealDealerState currentState;
    private RealDealerState previousActionState;

    [SerializeField] private List<RealDealerDurationPair> stateDurationsList;
    private Dictionary<RealDealerState, RandomFloat> stateDurations = new();
    private float stateTimer;

    private bool inFirstStage;

    [SerializeField] private Animator anim;
    private EnemyHealth health;

    [Header("Debug")]
    [SerializeField] private bool debugState;
    [ConditionalHide("debugState")][SerializeField] private RealDealerState stateToDebug;
    [SerializeField] private bool debugStartSecondStage;

    private void Awake() {
        InitializeDurationDict();

        health = GetComponent<EnemyHealth>();
    }

    private void InitializeDurationDict() {
        foreach (var stateDuration in stateDurationsList) {
            stateDurations.Add(stateDuration.State, stateDuration.Duration);
        }
    }

    private void OnEnable() {
        health.DeathEventTrigger.AddListener(OnDefeated);

        stateTimer = 0f;
        //ChangeState(FakeDealerState.StartingDialog);

        inFirstStage = !debugStartSecondStage;


    }

    private void OnDisable() {
        health.DeathEventTrigger.RemoveListener(OnDefeated);

        UpdateVisual();
    }

    private void OnDefeated() {
        //ChangeState(RealDealerState.BetweenStates);
    }

    #region Visual

    [SerializeField] private ParticleSystem sparksFake;
    [SerializeField] private ParticleSystem sparksReal;

    private void UpdateVisual() {
        sparksFake.gameObject.SetActive(false);
        sparksReal.gameObject.SetActive(true);

        var emission = sparksReal.emission;
        emission.enabled = !inFirstStage;
    }

    #endregion
}
