using DG.Tweening;
using MoreMountains.Tools;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Serialization;

[Serializable]
public enum FakeDealerState {
    StartingDialog,
    FleeDialog,
    BetweenStates,
    Swing,
    Lasers,
    Smashers
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

    private bool inSecondStage;

    [SerializeField] private Animator anim;
    private EnemyHealth health;
    private NavMeshAgent agent;
    private ChasePlayerBehavior chasePlayerBehavior;

    [SerializeField] private Transform centerPoint;

    private int defeatedAmount;

    [Header("Debug")]
    [SerializeField] private bool debugState;
    [ConditionalHide("debugState")][SerializeField] private FakeDealerState stateToDebug;
    [SerializeField] private bool debugStartSecondStage;

    private void Awake() {
        health = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
        chasePlayerBehavior = GetComponent<ChasePlayerBehavior>();

        InitializeDurationDict();
    }

    private void InitializeDurationDict() {
        foreach (var stateDuration in stateDurationsList) {
            stateDurations.Add(stateDuration.State, stateDuration.Duration);
        }
    }

    private void OnEnable() {

        defeatedAmount = ES3.Load("DealerDefeatedAmount", 0, ES3EncryptionMigration.GetES3Settings());
        if (defeatedAmount >= 3) {
            enabled = false;
            return;
        }

        health.DamagedEventTrigger.AddListener(OnDamaged);
        health.DeathEventTrigger.AddListener(OnDefeated);

        stateTimer = 0f;
        ChangeState(FakeDealerState.StartingDialog);

        inSecondStage = debugStartSecondStage;

        UpdateVisual();

        BossHealthUI.Instance.RemainAtSliver = true;

        //... more bloody the more times he's been defeated
        anim.SetInteger("defeatedAmount", defeatedAmount);

        //... only spawn blank memory cards when fighting real dealer
        GetComponent<SpawnBlankMemoryCards>().enabled = false;
    }

    private void OnDisable() {
        health.DamagedEventTrigger.RemoveListener(OnDamaged);
        health.DeathEventTrigger.RemoveListener(OnDefeated);

        if (spinSwordAudioSource != null && !spinSwordAudioSource.IsReturned()) {
            spinSwordAudioSource.ReturnToPool();
        }
    }

    private void Update() {

        if (currentState == FakeDealerState.StartingDialog ||
            currentState == FakeDealerState.FleeDialog) {

            HandleDialog();
            return;
        }

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
        FakeDealerState[] actionStates = new FakeDealerState[] {
            FakeDealerState.Swing,
            FakeDealerState.Lasers,
            FakeDealerState.Smashers
        };
        FakeDealerState[] availableStates = actionStates.Where(s => s != stateToAvoid).ToArray();
        ChangeState(availableStates.RandomItem());
    }

    private void ChangeState(FakeDealerState newState) {

        FakeDealerState previousState = currentState;
        currentState = newState;

        if (previousState != FakeDealerState.BetweenStates) {
            previousActionState = previousState;
        }

        stateTimer = 0;
        if (stateDurations.ContainsKey(newState)) {
            stateDurations[newState].Randomize();
        }

        if (newState == FakeDealerState.BetweenStates) {
        }
        else if (newState == FakeDealerState.Swing) {
            OnEnterSwingState();
        }
        else if (newState == FakeDealerState.Lasers) {
            StartCoroutine(ShootLasers());
        }
        else if (newState == FakeDealerState.Smashers) {
            SpawnSmashers();
        }

        if (previousState == FakeDealerState.BetweenStates) {
        }
        else if (previousState == FakeDealerState.Swing) {
            OnExitSwingState();
        }
        else if (previousState == FakeDealerState.Lasers) {
        }
        else if (previousState == FakeDealerState.Smashers) {
            RemoveSmashers();
        }

        OnChangeStateDialog();
    }

    private void OnDamaged() {
        if (health.HealthProportion < 0.5f) {
            inSecondStage = true;
            UpdateVisual();
        }
    }

    private void OnDefeated() {
        StartCoroutine(OnDefeatCor());
    }
    private IEnumerator OnDefeatCor() {

        defeatedAmount++;
        ES3.Save("DealerDefeatedAmount", defeatedAmount, ES3EncryptionMigration.GetES3Settings());

        Transform projectileContainer = Containers.Instance.Projectiles;
        foreach (Transform projectile in projectileContainer) {
            if (projectile.gameObject.activeSelf) {
                projectile.gameObject.ReturnToPool();
            }
        }

        // repeatedly set to between states so doesn't switch to action state
        int showFleeDialogDelay = 2;
        for (int i = 0; i < showFleeDialogDelay; i++) {
            ChangeState(FakeDealerState.BetweenStates);
            yield return new WaitForSeconds(1f);
        }

        ChangeState(FakeDealerState.FleeDialog);
    }

    #region Swing

    [Header("Swing")]
    [SerializeField] private SwingingSword swingingSword;
    private SwingingSword spawnedSwingingSword;

    [SerializeField] private float swingMoveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float swordSwingSpeed;

    private GameObject spinSwordAudioSource;

    private void OnEnterSwingState() {
        spawnedSwingingSword = swingingSword.Spawn(centerPoint.position, Containers.Instance.Projectiles);
        spawnedSwingingSword.Setup(inSecondStage, swordSwingSpeed);
        spawnedSwingingSword.gameObject.SetActive(true);

        StartCoroutine(EnterSwingStateCor());
    }

    private IEnumerator EnterSwingStateCor() {

        float fadeInSwordDelay = 1.5f;
        yield return new WaitForSeconds(fadeInSwordDelay);

        agent.speed = swingMoveSpeed;
        agent.acceleration = acceleration;
        chasePlayerBehavior.enabled = true;

        float sfxDelay = 1f;
        yield return new WaitForSeconds(sfxDelay);

        spinSwordAudioSource = AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.DealerSwordSpin, loop: true);
    }

    private void OnExitSwingState() {
        spawnedSwingingSword.FadeOut();
        chasePlayerBehavior.enabled = false;

        spinSwordAudioSource.ReturnToPool();
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

        int numOfLasers = inSecondStage ? 3 : 2;

        yield return new WaitForSeconds(shootCooldown);

        while (currentState == FakeDealerState.Lasers) {
            for (int i = 0; i < numOfLasers; i++) {
                float rotation = 360f / numOfLasers;
                Vector2 thisLaserDirection = shootDirection.GetDirectionRotated(i * rotation).normalized;

                float distance = 1f;
                Vector2 shootPos = (Vector2)centerPoint.position + (thisLaserDirection * distance);
                StraightMovement laser = laserPrefab.Spawn(shootPos, Containers.Instance.Projectiles);

                laser.Setup(thisLaserDirection, laserSpeed);
                laser.GetComponent<DamageOnContact>().Setup(laserDamage, 0f);

                laserShootParticles.Spawn(shootPos, Containers.Instance.Effects);
            }
            shootDirection.RotateDirection(laserRotateAngle);

            AudioManager.Instance.PlaySingleSound(AudioManager.Instance.AudioClips.LaserShoot);

            yield return new WaitForSeconds(shootCooldown);
        }
    }

    #endregion

    #region Smashers

    [Header("Smashers")]
    [SerializeField] private Smasher smasherPrefab;
    [SerializeField] private Smasher smallSmasherPrefab;
    [SerializeField] private Vector2[] smasherPositions;
    [SerializeField] private Vector2[] smallSmasherPositions;

    private List<Smasher> smashers;

    private void SpawnSmashers() {
        smashers = new();
        for (int i = 0; i < smasherPositions.Count(); i++) {
            Vector2 pos = (Vector2)Room.GetCurrentRoom().transform.position + smasherPositions[i];
            Smasher smasher = smasherPrefab.Spawn(pos, Containers.Instance.Projectiles);
            smashers.Add(smasher);
        }

        if (inSecondStage) {
            for (int i = 0; i < smallSmasherPositions.Count(); i++) {
                Vector2 pos = (Vector2)Room.GetCurrentRoom().transform.position + smallSmasherPositions[i];
                Smasher smasher = smallSmasherPrefab.Spawn(pos, Containers.Instance.Projectiles);
                smashers.Add(smasher);
            }
        }

        AudioManager.Instance.PlaySingleSound(AudioManager.Instance.AudioClips.SpawningSmashers);
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
    [SerializeField] private ParticleSystem sparksFake;
    [SerializeField] private ParticleSystem sparksReal;

    private void UpdateVisual() {
        sparksFake.gameObject.SetActive(true);
        sparksReal.gameObject.SetActive(false);

        var emission = sparksFake.emission;
        emission.enabled = inSecondStage;
    }

    #endregion

    #region Dialog

    [Header("Dialog")]
    [SerializeField] private InputActionReference nextDialogInput;

    [SerializeField] private LocalizedString[] startDialogs;
    [SerializeField] private LocalizedString[] fleeDialogs;

    private void OnChangeStateDialog() {
        if (currentState == FakeDealerState.StartingDialog) {
            DialogBox.Instance.ShowText(startDialogs.RandomItem());
        }
        else if (currentState == FakeDealerState.FleeDialog) {
            DialogBox.Instance.ShowText(fleeDialogs.RandomItem());
        }
        else {
            // not dialog state, so do nothing
        }
    }

    private void HandleDialog() {
        if (currentState == FakeDealerState.StartingDialog) {
            if (nextDialogInput.action.WasPerformedThisFrame()) {
                DialogBox.Instance.Hide();
                BossManager.Instance.ResumeEnterBossPlayer();

                ChangeState(FakeDealerState.BetweenStates);
            }
        }
        else if (currentState == FakeDealerState.FleeDialog) {
            if (nextDialogInput.action.WasPerformedThisFrame()) {
                DialogBox.Instance.Hide();

                StartCoroutine(Flee());
            }
        }
        else {
            Debug.LogError("Trying to handle dialog when dialog state not active!");
        }
    }

    #endregion

    #region Flee

    [Header("Flee")]
    [SerializeField] private ParticleSystem fleeParticles;
    [SerializeField] private CardDrop cardDropPrefab;

    private IEnumerator Flee() {

        float beforeFleeDelay = 1f;
        yield return new WaitForSeconds(beforeFleeDelay);

        ParticleSystem fleeParticlesInstance = fleeParticles.Spawn(transform.position, Containers.Instance.Effects);
        fleeParticlesInstance.gameObject.SetActive(true);

        gameObject.ReturnToPool();

        CardDrop memoryCard = cardDropPrefab.Spawn(transform.position, Containers.Instance.Drops);

        // at this point defeatedAmount already been incremented from this defeat
        CardType cardType = CardType.BlankMemoryCard1;
        switch (defeatedAmount) {
            case 1:
                cardType = CardType.BlankMemoryCard1;
                break;
            case 2:
                cardType = CardType.BlankMemoryCard1;
                break;
            case 3:
                cardType = CardType.BlankMemoryCard1;
                break;
            default:
                break;
        }

        memoryCard.SetCard(ResourceSystem.Instance.GetCardInstance(cardType));
    }

    [ContextMenu("ResetDefeatedAmount")]
    private void ResetDefeatedAmount() {
        ES3.Save("DealerDefeatedAmount", 0, ES3EncryptionMigration.GetES3Settings());
    }

    [Command]
    private void SetDefeatedAmount(int amount) {
        ES3.Save("DealerDefeatedAmount", amount, ES3EncryptionMigration.GetES3Settings());
    }

    #endregion
}
