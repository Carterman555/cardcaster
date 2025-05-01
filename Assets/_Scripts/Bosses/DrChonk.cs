using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DrChonk : MonoBehaviour, IHasEnemyStats, IBoss {

    [SerializeField] private ScriptableBoss scriptableBoss;
    public EnemyStats EnemyStats => scriptableBoss.Stats;

    private DrChonkState currentState;
    private DrChonkState previousActionState;

    private readonly DrChonkState[] actionStates = new DrChonkState[] { DrChonkState.Smash, DrChonkState.Roll, DrChonkState.ShootMinions };

    [SerializeField] private List<DrChunkStateDurationPair> stateDurationsList;
    private Dictionary<DrChonkState, RandomFloat> stateDurations = new();
    private float stateTimer;

    [SerializeField] private Transform centerPoint;

    [SerializeField] private Animator anim;
    private EnemyHealth health;

    [SerializeField] private float healAmount;

    [SerializeField] private bool debugState;
    [ConditionalHide("debugState")][SerializeField] private DrChonkState stateToDebug;

    private void Awake() {
        health = GetComponent<EnemyHealth>();
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

        SubShootMinionMethods();
        SubBounceMethods();

        // spawn 5 healer minions surrounding boss
        SpawnStartingMinions();
    }
    private void OnDisable() {
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
        else if (currentState == DrChonkState.Smash) {
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
        else if (previousState == DrChonkState.Smash) {
            StopCoroutine(smashUpdateCor);
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
        else if (newState == DrChonkState.Smash) {
            slowSmashing = true;
            smashUpdateCor = StartCoroutine(SmashUpdate());
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
            GameObject healerMinion = healerMinionPrefab.Spawn(pos, Containers.Instance.Enemies);
            healerMinion.GetComponent<BounceMoveBehaviour>().SetDirection(spawnDirection);
        }
    }

    [SerializeField] private ParticleSystem healEffect;

    public void Heal() {
        health.Heal(healAmount);
        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.DrChonkHeal);

        // to correct rotation when boss has different rotation due to BounceMoveBehaviour
        healEffect.transform.rotation = Quaternion.identity;
        healEffect.Play();
    }

    #region Smash

    [Header("Smash")]
    [SerializeField] private StraightMovement shockwaveProjectile;
    [SerializeField] private int projectileCount;
    [SerializeField] private float shockwaveDamage;
    [SerializeField] private float shockwaveKnockbackStrength;

    [SerializeField] private RandomInt slowSmashAmount;
    [SerializeField] private RandomInt quickSmashAmount;

    [SerializeField] private float slowSmashCooldown;
    [SerializeField] private float quickSmashCooldown;

    [SerializeField] private ParticleSystem smashParticles;

    private bool slowSmashing;

    private bool thisShotIsAlternate;

    private Coroutine smashUpdateCor;

    private IEnumerator SmashUpdate() {
        while (true) {
            int amount = slowSmashing ? slowSmashAmount.Randomize() : quickSmashAmount.Randomize();
            float cooldown = slowSmashing ? slowSmashCooldown : quickSmashCooldown;
            for (int i = 0; i < amount; i++) {
                yield return new WaitForSeconds(cooldown);

                anim.SetTrigger("smash");
            }

            slowSmashing = !slowSmashing;
        }
    }

    // played by animation
    public void CreateShockwaves() {

        // Calculate the angle between each shockwave
        float angleStep = 360f / projectileCount;
        float angle = 0f;

        if (thisShotIsAlternate) {
            angle = angleStep * 0.5f;
        }
        thisShotIsAlternate = !thisShotIsAlternate;

        for (int i = 0; i < projectileCount; i++) {
            Vector2 projectileDirection = angle.RotationToDirection();

            float distanceFromCenter = 1f;

            Vector2 spawnPosition = (Vector2)transform.position + projectileDirection * distanceFromCenter;
            StraightMovement projectile = shockwaveProjectile
                .Spawn(spawnPosition, Containers.Instance.Projectiles);
            projectile.Setup(projectileDirection);
            projectile.GetComponent<DamageOnContact>().Setup(shockwaveDamage, shockwaveKnockbackStrength);
            projectile.transform.up = projectileDirection;

            angle += angleStep;
        }

        smashParticles.Play();

        CameraShaker.Instance.ShakeCamera(1.5f);

        AudioManager.Instance.PlaySingleSound(AudioManager.Instance.AudioClips.DrChonkSmash);
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
    Smash = 1,
    Roll = 2,
    ShootMinions = 3
}

[Serializable]
public class DrChunkStateDurationPair {
    public DrChonkState State;
    public RandomFloat Duration;
}

