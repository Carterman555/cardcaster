using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.UI;

[Serializable]
public enum RealDealerState {
    StartingDialog,
    DefeatedDialog,
    BetweenStates,
    BoomerangSwords,
    Holograms,
    CardAttack
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

        int defeatedAmount = ES3.Load("DealerDefeatedAmount", 0, ES3EncryptionMigration.GetES3Settings());
        if (defeatedAmount < 3) {
            enabled = false;
            return;
        }

        health.DeathEventTrigger.AddListener(OnDefeated);
        HandCard.OnAnyCardUsed_Card += OnAnyCardUsed;

        stateTimer = 0f;
        ChangeState(RealDealerState.StartingDialog);

        inFirstStage = !debugStartSecondStage;

        StartCoroutine(FadeInRed());
        UpdateVisual();

        anim.SetInteger("defeatedAmount", 3);

        BecomeInvincible();
    }

    private void OnDisable() {
        health.DeathEventTrigger.RemoveListener(OnDefeated);
        HandCard.OnAnyCardUsed_Card += OnAnyCardUsed;
    }

    private void Update() {

        HandleInvincibility();

        if (currentState == RealDealerState.StartingDialog ||
            currentState == RealDealerState.DefeatedDialog) {

            HandleDialog();
            return;
        }

        stateTimer += Time.deltaTime;
        if (stateTimer > stateDurations[currentState].Value) {

            if (currentState == RealDealerState.BetweenStates) {
                if (!debugState) {
                    ChangeToRandomState(previousActionState);
                }
                else {
                    ChangeState(stateToDebug);
                }
            }
            else {
                ChangeState(RealDealerState.BetweenStates);
            }
        }
    }

    private void ChangeToRandomState(RealDealerState stateToAvoid) {
        RealDealerState[] allStates = Enum.GetValues(typeof(RealDealerState)) as RealDealerState[];
        RealDealerState[] availableStates = allStates.Where(s => s != stateToAvoid && s != RealDealerState.BetweenStates).ToArray();
        ChangeState(availableStates.RandomItem());
    }

    private void ChangeState(RealDealerState newState) {

        RealDealerState previousState = currentState;
        currentState = newState;

        if (previousState != RealDealerState.BetweenStates) {
            previousActionState = previousState;
        }

        stateTimer = 0;
        if (stateDurations.ContainsKey(newState)) {
            stateDurations[newState].Randomize();
        }

        if (newState == RealDealerState.BetweenStates) {
        }
        else if (newState == RealDealerState.BoomerangSwords) {
        }
        else if (newState == RealDealerState.Holograms) {
        }
        else if (newState == RealDealerState.CardAttack) {
        }

        if (previousState == RealDealerState.BetweenStates) {
        }
        else if (previousState == RealDealerState.BoomerangSwords) {
        }
        else if (previousState == RealDealerState.Holograms) {
        }
        else if (previousState == RealDealerState.CardAttack) {
        }

        OnChangeStateDialog();
    }

    private void OnDefeated() {
        ChangeState(RealDealerState.BetweenStates);
    }

    #region Invincibility

    [Header("Invincibility")]
    [SerializeField] private ParticleSystem invincibilityEffect;
    [SerializeField] private float invincibilityDuration;
    private float invincibilityTimer;

    private Invincibility invincibility;

    private void OnAnyCardUsed(ScriptableCardBase usedCard) {
        if (usedCard.MemoryCard) {
            RemoveInvincibility();
        }
    }

    private void BecomeInvincible() {
        invincibility = gameObject.AddComponent<Invincibility>();
        invincibilityEffect.Play();
        invincibilityEffect.Clear();

        BossHealthUI.Instance.SetInvincible(true);
    }

    private void RemoveInvincibility() {
        Destroy(invincibility);
        invincibilityEffect.Stop();
        invincibilityEffect.Clear();

        BossHealthUI.Instance.SetInvincible(false);

        invincibilityTimer = 0f;
    }

    private void HandleInvincibility() {
        bool isInvincible = invincibility != null;
        if (!isInvincible) {
            invincibilityTimer += Time.deltaTime;
            if (invincibilityTimer > invincibilityDuration) {
                BecomeInvincible();
            }
        }
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
        sparksFake.gameObject.SetActive(false);
        sparksReal.gameObject.SetActive(true);

        var emission = sparksReal.emission;
        emission.enabled = !inFirstStage;
    }

    #endregion

    #region Dialog

    [Header("Dialog")]
    [SerializeField] private InputActionReference nextDialogInput;

    [SerializeField] private LocalizedString[] startDialogs;
    [SerializeField] private LocalizedString[] defeatedDialogs;

    private void OnChangeStateDialog() {
        if (currentState == RealDealerState.StartingDialog) {
            DialogBox.Instance.ShowText(startDialogs.RandomItem());
        }
        else if (currentState == RealDealerState.DefeatedDialog) {
            DialogBox.Instance.ShowText(defeatedDialogs.RandomItem());
        }
        else {
            // not dialog state, so do nothing
        }
    }

    private void HandleDialog() {
        if (currentState == RealDealerState.StartingDialog) {
            if (nextDialogInput.action.WasPerformedThisFrame()) {
                DialogBox.Instance.Hide();
                BossManager.Instance.ResumeEnterBossPlayer();

                ChangeState(RealDealerState.BetweenStates);
            }
        }
        else if (currentState == RealDealerState.DefeatedDialog) {
            if (nextDialogInput.action.WasPerformedThisFrame()) {
                DialogBox.Instance.Hide();

            }
        }
        else {
            Debug.LogError("Trying to handle dialog when dialog state not active!");
        }
    }

    #endregion
}
