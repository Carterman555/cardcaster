using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrChonk : MonoBehaviour, IHasStats, IBoss {

    [SerializeField] private ScriptableBoss scriptableBoss;
    public Stats Stats => scriptableBoss.Stats;

    private DrChonkState currentState;
    private DrChonkState previousActionState;

    private readonly DrChonkState[] actionStates = new DrChonkState[] { DrChonkState.EatMinions, DrChonkState.Roll, DrChonkState.ShootMinions };

    [SerializeField] private List<DrChunkStateDurationPair> stateDurationsList;
    private Dictionary<DrChonkState, RandomFloat> stateDurations = new();
    private float stateTimer;

    [SerializeField] private Transform centerPoint;

    [SerializeField] private Animator anim;

    [SerializeField] private bool debugState;
    [ConditionalHide("debugState")] [SerializeField] private DrChonkState stateToDebug;

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
        SubShootMinionMethods();
        SubBounceMethods();

        // spawn 5 healer minions surrounding boss
        SpawnStartingMinions();
    }
    private void OnDisable() {
        UnsubEatMinionMethods();
        UnsubShootMinionMethods();
        UnsubBounceMethods();

        if (!Helpers.GameStopping()) {
            OnDefeated();
        }
    }

    private void Update() {

        stateTimer += Time.deltaTime;
        if (stateTimer > stateDurations[currentState].Value) {

            if (currentState == DrChonkState.BetweenStates) {
                if (debugState) {
                    ChangeState(stateToDebug);
                }
                else {
                    ChangeToRandomState(previousActionState);
                }
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

    private void ChangeToRandomState(DrChonkState stateToAvoid) {
        DrChonkState[] availableStates = actionStates.Where(s => s != stateToAvoid).ToArray();
        DrChonkState newState = availableStates.RandomItem();
        ChangeState(newState);
    }

    private void ChangeState(DrChonkState newState) {

        DrChonkState previousState = currentState;
        currentState = newState;

        if (actionStates.Contains(previousState)) {
            previousActionState = previousState;
        }

        stateTimer = 0;
        stateDurations[newState].Randomize();

        if (previousState == DrChonkState.BetweenStates) {

        }
        else if (previousState == DrChonkState.EatMinions) {

            OnStopSucking();

            // close mouth
            anim.SetBool("mouthOpen", false);
            suckEffect.SetActive(false);

            suckAudioSource.Stop();
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

            suckAudioSource.Play();
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

            spawnDirection.RotateDirection(rotationBetweenMinions);

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

    [SerializeField] private AudioSource suckAudioSource;

    private List<HealerMinion> healersSucking = new();

    private Health health;

    private void SubEatMinionMethods() {
        suckMinionTrigger.OnEnterContact_GO += TrySuckMinion;
        eatMinionTrigger.OnEnterContact_GO += TryEatMinion;
    }

    private void UnsubEatMinionMethods() {
        suckMinionTrigger.OnEnterContact_GO -= TrySuckMinion;
        eatMinionTrigger.OnEnterContact_GO -= TryEatMinion;
    }

    private void TrySuckMinion(GameObject collisionObject) {
        if (currentState == DrChonkState.EatMinions) {
            if (collisionObject.TryGetComponent(out HealerMinion healerMinion)) {
                // suck in minion
                healerMinion.SuckToChonk(suckCenter.position);
                healersSucking.Add(healerMinion);
            }
        }
    }

    private void TryEatMinion(GameObject collisionObject) {
        if (currentState == DrChonkState.EatMinions) {
            if (collisionObject.TryGetComponent(out HealerMinion healerMinion)) {
                // eat minion
                collisionObject.ReturnToPool();

                // heal
                health.Heal(eatMinionHealAmount);

                // heal effect
                healEffectPrefab.Spawn(centerPoint.position, Containers.Instance.Effects);

                healersSucking.Remove(healerMinion);

                AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.DrChonkEat);
                AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.DrChonkHeal);
            }
        }
    }

    private void OnStopSucking() {
        foreach (HealerMinion minion in healersSucking) {
            minion.StopSuck();
        }
        healersSucking.Clear();
    }

    #endregion

    #region Roll

    private BounceMoveBehaviour bounceMoveBehaviour;

    private void SubBounceMethods() {
        bounceMoveBehaviour.OnBounce += ShakeCamera;
    }

    private void UnsubBounceMethods() {
        bounceMoveBehaviour.OnBounce -= ShakeCamera;
    }

    private void ShakeCamera() {
        CameraShaker.Instance.ShakeCamera(0.3f);
    }

    #endregion


    #region Shooting Minions

    private StraightShootBehavior straightShootBehavior;

    private List<SpawnOnContact> healMinionProjectiles = new List<SpawnOnContact>();

    private void SubShootMinionMethods() {
        straightShootBehavior.OnShoot_Projectile += OnShootProjectile;
    }
    private void UnsubShootMinionMethods() {
        straightShootBehavior.OnShoot_Projectile -= OnShootProjectile;
    }

    private void OnShootProjectile(GameObject projectile) {

        if (!projectile.TryGetComponent(out SpawnOnContact spawnOnContact)) {
            return;
        }

        //... make sure the component that spawns in a minion is enabled because it could have gotten disabled
        //... from when the boss was defeated (DisableSpawnOnProjectiles())
        spawnOnContact.enabled = true;

        if (!healMinionProjectiles.Contains(spawnOnContact)) {
            healMinionProjectiles.Add(spawnOnContact);
        }
    }

    // prevents the healer minion projectiles from spawning enemies
    private void DisableSpawnOnProjectiles() {
        foreach (SpawnOnContact spawnOnContact in healMinionProjectiles) {
            spawnOnContact.enabled = false;
        }
        healMinionProjectiles.Clear();
    }

    #endregion

    private void OnDefeated() {
        DisableSpawnOnProjectiles();
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

