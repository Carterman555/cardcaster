using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckOfDoom : MonoBehaviour, IHasStats, IBoss {

    [SerializeField] private ScriptableBoss scriptableBoss;
    public Stats GetStats() {
        return scriptableBoss.Stats;
    }

    private DeckOfDoomState currentState;
    private DeckOfDoomState previousActionState;

    private readonly DeckOfDoomState[] actionStates = new DeckOfDoomState[] { DeckOfDoomState.Spin, DeckOfDoomState.Split, DeckOfDoomState.Dash };

    [SerializeField] private List<DeckOfDoomStateDurationPair> stateDurationsList;
    private Dictionary<DeckOfDoomState, RandomFloat> stateDurations = new();
    private float stateTimer;

    [SerializeField] private Animator anim;

    private void Awake() {
        InitializeDurationDict();

        shootBehavior = GetComponent<StraightShootBehavior>();
        shootBehavior.SetShootTarget(StraightShootBehavior.ShootTarget.Random);

        bounceMoveBehavior = GetComponent<BounceMoveBehaviour>();
    }

    private void InitializeDurationDict() {
        foreach (var stateDuration in stateDurationsList) {
            stateDurations.Add(stateDuration.State, stateDuration.Duration);
        }
    }

    private void OnEnable() {
        //ChangeState(DeckOfDoomState.Split);
        ChangeState(DeckOfDoomState.BetweenStates);

        stateTimer = 0f;
    }

    private void Update() {

        print($"State: {currentState.ToString()}");

        stateTimer += Time.deltaTime;
        if (stateTimer > stateDurations[currentState].Value) {

            if (currentState == DeckOfDoomState.BetweenStates) {
                //ChangeToRandomState(previousActionState);
                ChangeState(DeckOfDoomState.Split);
            }
            else {
                ChangeState(DeckOfDoomState.BetweenStates);
            }
        }
    }

    private void ChangeToRandomState(DeckOfDoomState stateToAvoid) {
        DeckOfDoomState[] availableStates = actionStates.Where(s => s != stateToAvoid).ToArray();
        DeckOfDoomState newState = availableStates.RandomItem();
        ChangeState(newState);
    }

    private void ChangeState(DeckOfDoomState newState) {

        DeckOfDoomState previousState = currentState;
        currentState = newState;

        if (actionStates.Contains(previousState)) {
            previousActionState = previousState;
        }

        stateTimer = 0;
        stateDurations[newState].Randomize();

        if (previousState == DeckOfDoomState.BetweenStates) {
        }
        else if (previousState == DeckOfDoomState.Spin) {
            shootBehavior.enabled = false;
            anim.SetBool("spinning", false);
        }
        else if (previousState == DeckOfDoomState.Split) {
            anim.SetBool("deckSplitFace", false);
            bounceMoveBehavior.enabled = false;

            RejoinSplits();
        }
        else if (previousState == DeckOfDoomState.Dash) {
        }

        if (newState == DeckOfDoomState.BetweenStates) {
        }
        else if (newState == DeckOfDoomState.Spin) {
            //... start shooting after shoot delay
            Invoke(nameof(EnableShooting), shootDelay);
            anim.SetBool("spinning", true);
        }
        else if (newState == DeckOfDoomState.Split) {
            SpawnSplits();
            anim.SetBool("deckSplitFace", true);
            bounceMoveBehavior.enabled = true;
        }
        else if (newState == DeckOfDoomState.Dash) {
        }
    }


    #region Spin

    private StraightShootBehavior shootBehavior;

    [Header("Spin State")]
    [SerializeField] private float shootDelay;

    // to have delay before shooting
    private void EnableShooting() {
        shootBehavior.enabled = true;
    }

    #endregion


    #region Split

    private BounceMoveBehaviour bounceMoveBehavior;

    [SerializeField] private DeckOfDoomSplit deckSplitPrefab;
    private DeckOfDoomSplit[] splits;

    [SerializeField] private int numOfSplits = 3;

    private void SpawnSplits() {

        splits = new DeckOfDoomSplit[numOfSplits];
        for (int i = 0; i < numOfSplits; i++) {
            DeckOfDoomSplit deckSplit = deckSplitPrefab.Spawn(transform.position, Containers.Instance.Enemies);
            splits[i] = deckSplit;
        }
    }

    private void RejoinSplits() {
        foreach (DeckOfDoomSplit split in splits) {
            split.GetComponent<BounceMoveBehaviour>().enabled = false;
            split.transform.DOMove(transform.position, duration: 0.3f).SetEase(Ease.InSine).OnComplete(() => {
                split.gameObject.ReturnToPool();
            });
        }
    }


    #endregion

    #region Dash



    #endregion

}

[Serializable]
public enum DeckOfDoomState {
    BetweenStates = 0,
    Spin = 1,
    Split = 2,
    Dash = 3
}

[Serializable]
public class DeckOfDoomStateDurationPair {
    public DeckOfDoomState State;
    public RandomFloat Duration;
}
