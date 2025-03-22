using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckOfDoom : MonoBehaviour, IHasCommonStats, IBoss {

    [SerializeField] private ScriptableBoss scriptableBoss;
    public CommonStats CommonStats => scriptableBoss.Stats;

    private DeckOfDoomState currentState;
    private DeckOfDoomState previousActionState;

    private readonly DeckOfDoomState[] actionStates = new DeckOfDoomState[] { DeckOfDoomState.Spin, DeckOfDoomState.Split, DeckOfDoomState.CardSurge };

    [SerializeField] private List<DeckOfDoomStateDurationPair> stateDurationsList;
    private Dictionary<DeckOfDoomState, RandomFloat> stateDurations = new();
    private float stateTimer;

    [SerializeField] private Animator anim;

    private Health health;

    [SerializeField] private bool debugState;
    [ConditionalHide("debugState")][SerializeField] private DeckOfDoomState stateToDebug;

    private void Awake() {
        InitializeDurationDict();

        health = GetComponent<Health>();
        shootBehavior = GetComponent<SpiralShootBehaviour>();
        bounceMoveBehavior = GetComponent<BounceMoveBehaviour>();
    }

    private void InitializeDurationDict() {
        foreach (var stateDuration in stateDurationsList) {
            stateDurations.Add(stateDuration.State, stateDuration.Duration);
        }
    }

    private void OnEnable() {
        ChangeState(DeckOfDoomState.BetweenStates);

        stateTimer = 0f;

        health.OnDeath += OnDeath;
    }

    private void OnDisable() {
        health.OnDeath -= OnDeath;
    }

    private void Update() {

        stateTimer += Time.deltaTime;
        if (stateTimer > stateDurations[currentState].Value) {

            if (currentState == DeckOfDoomState.BetweenStates) {
                if (!debugState) {
                    ChangeToRandomState(previousActionState);
                }
                else {
                    ChangeState(stateToDebug);
                }
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

    private void OnDeath() {
        StartCoroutine(OnDeathCor());
    }
    private IEnumerator OnDeathCor() {
        ChangeState(DeckOfDoomState.BetweenStates);

        float delay = 1f;
        yield return new WaitForSeconds(delay);

        GetComponent<DeathParticles>().GenerateParticles();

        gameObject.ReturnToPool();
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

            spinAudioSource.Stop();
        }
        else if (previousState == DeckOfDoomState.Split) {
            anim.SetBool("deckSplitFace", false);
            bounceMoveBehavior.enabled = false;

            RejoinSplits();
        }
        else if (previousState == DeckOfDoomState.CardSurge) {
            StopShootingSurges();
        }

        if (newState == DeckOfDoomState.BetweenStates) {
        }
        else if (newState == DeckOfDoomState.Spin) {
            //... start shooting after shoot delay
            Invoke(nameof(EnableShooting), shootDelay);
            anim.SetBool("spinning", true);

            spinAudioSource.Play();
        }
        else if (newState == DeckOfDoomState.Split) {
            SpawnSplits();
            anim.SetBool("deckSplitFace", true);
            bounceMoveBehavior.enabled = true;
        }
        else if (newState == DeckOfDoomState.CardSurge) {
            StartShootingSurges();
        }
    }


    #region Spin

    [Header("Spin State")]
    private SpiralShootBehaviour shootBehavior;

    [SerializeField] private AudioSource spinAudioSource;
    [SerializeField] private float shootDelay;

    // to have delay before shooting
    private void EnableShooting() {
        shootBehavior.enabled = true;
    }

    #endregion


    #region Split

    [Header("Split State")]
    private BounceMoveBehaviour bounceMoveBehavior;

    [SerializeField] private DeckOfDoomSplit deckSplitPrefab;
    private DeckOfDoomSplit[] splits;

    [SerializeField] private int numOfSplits = 3;

    private void SpawnSplits() {

        splits = new DeckOfDoomSplit[numOfSplits];
        for (int i = 0; i < numOfSplits; i++) {
            DeckOfDoomSplit deckSplit = deckSplitPrefab.Spawn(transform.position, Containers.Instance.Enemies);
            deckSplit.GetComponent<SharedHealth>().SetHealth(GetComponent<Health>());

            splits[i] = deckSplit;
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.DeckOfDoomSplit);
    }

    private void RejoinSplits() {
        foreach (DeckOfDoomSplit split in splits) {
            split.GetComponent<BounceMoveBehaviour>().enabled = false;
            split.transform.DOMove(transform.position, duration: 0.3f).SetEase(Ease.InSine).OnComplete(() => {
                split.gameObject.ReturnToPool();
            });
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.DeckOfDoomSplit);
    }


    #endregion

    #region CardSurge

    [Header("Card Surge State")]
    [SerializeField] private CardSurge cardSurgePrefab;
    [SerializeField] private float surgeCooldown;
    [SerializeField] [Range(0f, 1f)] private float targetPlayerProbability;

    private Coroutine shootSurgesCoroutine;

    private void StartShootingSurges() {
        shootSurgesCoroutine = StartCoroutine(ShootSurges());
    }

    private void StopShootingSurges() {

        if (shootSurgesCoroutine == null) {
            Debug.LogWarning("Tried to stop shooting surges, but coroutine is null!");
            return;
        }

        StopCoroutine(shootSurgesCoroutine);
        shootSurgesCoroutine = null;
    }

    private IEnumerator ShootSurges() {

        while (true) {
            yield return new WaitForSeconds(surgeCooldown);

            SpawnSurge();
        }
    }

    private void SpawnSurge() {
        
        // randomly choose between targeting random pos in room or player
        bool targetPlayer = UnityEngine.Random.Range(0f, 1f) < targetPlayerProbability;
        CardSurge.TargetType targetType = targetPlayer ? CardSurge.TargetType.Player : CardSurge.TargetType.Random;

        CardSurge cardSurge = cardSurgePrefab.Spawn(Containers.Instance.Projectiles);
        cardSurge.Setup(targetType);
    }

    #endregion

}

[Serializable]
public enum DeckOfDoomState {
    BetweenStates = 0,
    Spin = 1,
    Split = 2,
    CardSurge = 3
}

[Serializable]
public class DeckOfDoomStateDurationPair {
    public DeckOfDoomState State;
    public RandomFloat Duration;
}
