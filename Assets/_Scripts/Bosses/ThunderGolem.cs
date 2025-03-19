using DG.Tweening;
using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.ParticleSystem;

public class ThunderGolem : MonoBehaviour, IHasStats, IBoss {

    [SerializeField] private ScriptableBoss scriptableBoss;
    public Stats Stats => scriptableBoss.Stats;

    private GolemState currentState;
    private GolemState previousActionState;

    private readonly GolemState[] actionStates = new GolemState[] { GolemState.Charge, GolemState.Chase, GolemState.Areas };

    [SerializeField] private List<GolemStateDurationPair> stateDurationsList;
    private Dictionary<GolemState, RandomFloat> stateDurations = new();
    private float stateTimer;

    [SerializeField] private Animator anim;

    [SerializeField] private Transform centerPoint;

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
        stateTimer = 0f;

        ChangeState(GolemState.BetweenStates);

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
        else if (previousState == GolemState.Charge) {
        }
        else if (previousState == GolemState.Chase) {
            anim.SetBool("moving", false);
            chaseBehavior.enabled = false;
            visual.material.SetFloat("_Glow", 0f);
        }
        else if (previousState == GolemState.Areas) {
        }

        if (newState == GolemState.BetweenStates) {
        }
        else if (newState == GolemState.Charge) {
            StartCoroutine(Charge());
        }
        else if (newState == GolemState.Chase) {
            anim.SetBool("moving", true);
            chaseBehavior.enabled = true;
            visual.material.SetFloat("_Glow", 3f);

            StartCoroutine(PositionShootPoint());
            StartCoroutine(ShootProjectiles());
        }
        else if (newState == GolemState.Areas) {
            StartCoroutine(SpawnElectricAreas());
        }
    }


    #region Charge

    [Header("Charge")]
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private ParticleSystem electricFieldParticles;

    [SerializeField] private ParticleSystem electricExplosionPrefab;

    [SerializeField] private StraightMovement electricProjectilePrefab;
    [SerializeField] private RandomInt projectileAmount;
    [SerializeField] private RandomFloat projectileDistance;
    [SerializeField] private RandomFloat projectileSpeed;

    private IEnumerator Charge() {

        var emissionModule = electricFieldParticles.emission;

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.ThunderGolemCharge);

        while (currentState == GolemState.Charge) {

            //... ranges from 0 to 1 depending on how long golem has been in charge state
            float normalizedStateTimer = Mathf.InverseLerp(0, stateDurations[currentState].Value, stateTimer);

            float maxGlow = 100;
            float glow = Mathf.Lerp(0, maxGlow, normalizedStateTimer);
            visual.material.SetFloat("_Glow", glow);

            float maxParticleRate = 100;
            float particleRate = Mathf.Lerp(0, maxParticleRate, normalizedStateTimer);
            emissionModule.rateOverTime = particleRate;

            yield return null;
        }

        visual.material.SetFloat("_Glow", 0f);
        emissionModule.rateOverTime = 0;

        ChargeExplode();
    }

    private void ChargeExplode() {
        electricExplosionPrefab.Spawn(centerPoint.position, Containers.Instance.Effects);

        projectileAmount.Randomize();
        for (int i = 0; i < projectileAmount.Value; i++) {

            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
            Vector2 position = (Vector2)centerPoint.position + randomDirection * projectileDistance.Randomize();

            StraightMovement projectile = electricProjectilePrefab.Spawn(position, Containers.Instance.Projectiles);

            projectile.Setup(randomDirection, projectileSpeed.Randomize());
            projectile.GetComponent<DamageOnContact>().Setup(Stats.Damage, Stats.KnockbackStrength);
        }

        CameraShaker.Instance.ShakeCamera(2f);
    }

    #endregion


    #region Chase

    [Header("Chase")]
    [SerializeField] private ChasePlayerBehavior chaseBehavior;
    [SerializeField] private StraightShootBehavior shootBehavior; // never enabled, just plays shootBehavior.ShootProjectile(); from this script

    [SerializeField] private Transform shootPoint;

    [SerializeField] private ParticleSystem smallElectricExplosionPrefab;


    private IEnumerator PositionShootPoint() {
        while (currentState == GolemState.Chase) {

            Vector2 toPlayerDirection = (PlayerMovement.Instance.transform.position - transform.position).normalized;
            float distanceFromGolem = 2.5f;
            shootPoint.position = (Vector2)transform.position + (toPlayerDirection * distanceFromGolem);

            yield return null;
        }
    }

    private IEnumerator ShootProjectiles() {
        while (currentState == GolemState.Chase) {

            float glowDuration = 0f;

            if (glowDuration > Stats.AttackCooldown) {
                Debug.LogWarning("glowDuration should not be greater than GetStats().AttackCooldown!");
                glowDuration = Stats.AttackCooldown;
            }

            // minus glowDuration so glowing effect doesn't add to shoot cooldown
            yield return new WaitForSeconds(Stats.AttackCooldown - glowDuration);

            shootBehavior.ShootProjectile();
        }
    }

    #endregion


    #region Area

    [Header("Area")]
    [SerializeField] private ElectricArea electricAreaPrefab;
    [SerializeField] private RandomFloat areaSpawnCooldown;

    [SerializeField][Range(0f, 1f)] private float spawnOnPlayerProbability;

    private IEnumerator SpawnElectricAreas() {

        while (currentState == GolemState.Areas) {

            yield return new WaitForSeconds(areaSpawnCooldown.Randomize());

            Vector2 spawnPosition;
            if (UnityEngine.Random.value < spawnOnPlayerProbability) {
                spawnPosition = PlayerMovement.Instance.transform.position;
            }
            else {
                spawnPosition = new RoomPositionHelper().GetRandomRoomPos(obstacleAvoidDistance: 0f, wallAvoidDistance: 3f);
            }

            electricAreaPrefab.Spawn(spawnPosition, Containers.Instance.Projectiles);
        }

    }

    #endregion

}

[Serializable]
public enum GolemState {
    BetweenStates = 0,
    Charge = 1,
    Chase = 2,
    Areas = 3
}

[Serializable]
public class GolemStateDurationPair {
    public GolemState State;
    public RandomFloat Duration;
}
