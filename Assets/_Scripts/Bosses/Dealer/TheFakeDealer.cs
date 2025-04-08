using DG.Tweening;
using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public enum FakeDealerState {
    BetweenStates = 0,
    Swing = 1,
    Lasers = 2,
    Smashers = 3
}

[Serializable]
public class FakeDealerDurationPair {
    public FakeDealerState State;
    public RandomFloat Duration;
}

public class TheFakeDealer : MonoBehaviour, IHasEnemyStats, IBoss {

    [SerializeField] private ScriptableBoss scriptableBoss;
    public EnemyStats EnemyStats => scriptableBoss.Stats;

    private FakeDealerState currentState;
    private FakeDealerState previousActionState;

    [SerializeField] private List<FakeDealerDurationPair> stateDurationsList;
    private Dictionary<FakeDealerState, RandomFloat> stateDurations = new();
    private float stateTimer;

    private bool inFirstStage;
    [SerializeField] private bool debugStartSecondStage;

    [SerializeField] private Animator anim;
    private EnemyHealth health;
    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private ChasePlayerBehavior chasePlayerBehavior;

    [SerializeField] private Transform centerPoint;

    [SerializeField] private bool debugState;
    [ConditionalHide("debugState")][SerializeField] private FakeDealerState stateToDebug;

    private void Awake() {
        InitializeDurationDict();

        health = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        chasePlayerBehavior = GetComponent<ChasePlayerBehavior>();
    }

    private void InitializeDurationDict() {
        foreach (var stateDuration in stateDurationsList) {
            stateDurations.Add(stateDuration.State, stateDuration.Duration);
        }
    }

    private void OnEnable() {
        stateTimer = 0f;
        ChangeState(FakeDealerState.BetweenStates);

        inFirstStage = !debugStartSecondStage;

        StartCoroutine(FadeInRed());
        UpdateVisual();

        health.DeathEventTrigger.AddListener(OnDeath);
    }

    private void OnDisable() {
        health.DeathEventTrigger.RemoveListener(OnDeath);
    }

    private void Update() {

        stateTimer += Time.deltaTime;
        if (stateTimer > stateDurations[currentState].Value) {

            if (currentState == FakeDealerState.BetweenStates) {
                if (!debugState) {
                    ChangeToRandomState(previousActionState);
                }
                else {
                    ChangeState(stateToDebug);
                }
            }
            else {
                ChangeState(FakeDealerState.BetweenStates);
            }
        }
    }

    private void ChangeToRandomState(FakeDealerState stateToAvoid) {
        FakeDealerState[] allStates = Enum.GetValues(typeof(FakeDealerState)) as FakeDealerState[];
        FakeDealerState[] availableStates = allStates.Where(s => s != stateToAvoid && s != FakeDealerState.BetweenStates).ToArray();
        ChangeState(availableStates.RandomItem());
    }

    private void OnDeath() {
        StartCoroutine(OnDeathCor());
    }
    private IEnumerator OnDeathCor() {
        ChangeState(FakeDealerState.BetweenStates);

        float delay = 1f;
        yield return new WaitForSeconds(delay);

        GetComponent<DeathParticles>().GenerateParticles();

        gameObject.ReturnToPool();
    }

    private void ChangeState(FakeDealerState newState) {

        FakeDealerState previousState = currentState;
        currentState = newState;

        if (previousState != FakeDealerState.BetweenStates) {
            previousActionState = previousState;
        }

        stateTimer = 0;
        stateDurations[newState].Randomize();

        if (previousState == FakeDealerState.BetweenStates) {
        }
        else if (previousState == FakeDealerState.Swing) {
            chasePlayerBehavior.enabled = false;
        }
        else if (previousState == FakeDealerState.Lasers) {
        }
        else if (previousState == FakeDealerState.Smashers) {
            RemoveSmashers();
        }

        if (newState == FakeDealerState.BetweenStates) {
        }
        else if (newState == FakeDealerState.Swing) {
            EnterSwingState();
        }
        else if (newState == FakeDealerState.Lasers) {
            StartCoroutine(ShootLasers());
        }
        else if (newState == FakeDealerState.Smashers) {
            SpawnSmashers();
        }
    }

    #region Swing

    [Header("Swing")]
    [SerializeField] private MMAutoRotate swordRotate;
    [SerializeField] private SpriteRenderer sword1Renderer;
    [SerializeField] private SpriteRenderer sword2Renderer;

    [SerializeField] private float swingMoveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float swordSwingSpeed;

    private void EnterSwingState() {

        swordRotate.gameObject.SetActive(true);
        swordRotate.enabled = false;
        swordRotate.RotationSpeed = new(0f, 0f, swordSwingSpeed);

        sword2Renderer.gameObject.SetActive(!inFirstStage);

        sword1Renderer.Fade(0f);
        sword2Renderer.Fade(0f);
        sword1Renderer.DOFade(1f, duration: 0.3f);
        sword2Renderer.DOFade(1f, duration: 0.3f).OnComplete(() => {
            swordRotate.enabled = true;
        });


        agent.speed = swingMoveSpeed;
        agent.acceleration = acceleration;
        chasePlayerBehavior.enabled = true;
    }

    #endregion

    #region Lasers

    [Header("Lasers")]
    [SerializeField] private StraightMovement laserPrefab;

    [SerializeField] private float shootCooldown;
    [SerializeField] private float laserSpeed;
    [SerializeField] private float laserDamage;
    [SerializeField] private float laserRotateAngle;

    [SerializeField] private ParticleSystem laserShootParticles;

    private IEnumerator ShootLasers() {

        Vector2 shootDirection = Vector2.up;

        int numOfLasers = inFirstStage ? 2 : 4;

        while (currentState == FakeDealerState.Lasers) {
            yield return new WaitForSeconds(shootCooldown);

            for (int i = 0; i < numOfLasers; i++) {
                float rotation = 360f / numOfLasers;
                Vector2 thisLaserDirection = shootDirection.RotateDirection(i * rotation).normalized;

                float distance = 1f;
                Vector2 shootPos = (Vector2)centerPoint.position + (thisLaserDirection * distance);
                StraightMovement laser = laserPrefab.Spawn(shootPos, Containers.Instance.Projectiles);

                laser.Setup(thisLaserDirection, laserSpeed);
                laser.GetComponent<DamageOnContact>().Setup(laserDamage, 0f);

                laserShootParticles.Spawn(shootPos, Containers.Instance.Effects);
            }
            shootDirection.RotateDirection(laserRotateAngle);
        }
    }

    #endregion

    #region Smashers

    [Header("Smashers")]
    [SerializeField] private Smasher smasherPrefab;
    [SerializeField] private Vector2[] smasherPositions;

    private List<Smasher> smashers;

    private void SpawnSmashers() {
        smashers = new();
        for (int i = 0; i < smasherPositions.Count(); i++) {
            Vector2 pos = (Vector2)Room.GetCurrentRoom().transform.position + smasherPositions[i];
            Smasher smasher = smasherPrefab.Spawn(pos, Containers.Instance.Projectiles);
            smashers.Add(smasher);
        }
    }

    private void RemoveSmashers() {
        foreach (Smasher smasher in smashers) {
            smasher.DoFadeOut().OnComplete(() => {
                smasher.gameObject.ReturnToPool();
            });
        }
        smashers.Clear();
    }

    #endregion

    #region Visual

    [Header("Visual")]
    [SerializeField] private Material redMaterial;
    [SerializeField] private SpriteRenderer visual;
    private Material redMaterialInstance;

    [SerializeField] private ParticleSystem sparksFake;
    [SerializeField] private ParticleSystem sparksReal;

    private IEnumerator FadeInRed() {

        redMaterialInstance = new Material(redMaterial);
        visual.material = redMaterialInstance;

        float glow = 0f;

        float finalGlow = 2f;
        float glowFadeSpeed = 2f;

        while (glow < finalGlow) {
            redMaterialInstance.SetFloat("_ShineGlow", glow);
            glow += glowFadeSpeed * Time.deltaTime;

            yield return null;
        }
    }

    private void UpdateVisual() {
        bool realDealer = false;

        if (realDealer) {
            sparksFake.gameObject.SetActive(false);
            sparksReal.gameObject.SetActive(true);

            var emission = sparksReal.emission;
            emission.enabled = !inFirstStage;
        }
        else {
            sparksFake.gameObject.SetActive(true);
            sparksReal.gameObject.SetActive(false);

            var emission = sparksFake.emission;
            emission.enabled = !inFirstStage;
        }
    }
    #endregion
}
