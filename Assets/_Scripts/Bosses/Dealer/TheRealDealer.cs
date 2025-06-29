using DG.Tweening;
using MoreMountains.Tools;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

[Serializable]
public enum RealDealerState {
    StartingDialog,
    DefeatedDialog,
    BetweenStates,
    BoomerangSwords,
    Bouncers,
    CardAttack
}

[Serializable]
public class RealDealerDurationPair {
    public RealDealerState State;
    public RandomFloat Duration;
}

public class TheRealDealer : MonoBehaviour, IHasEnemyStats, IBoss {

    [SerializeField] private ScriptableBoss scriptableBoss;
    public EnemyStats GetEnemyStats() {
        return StatsManager.GetScaledEnemyStats(scriptableBoss.Stats);
    }

    private RealDealerState currentState;
    private RealDealerState previousActionState;

    private RealDealerState[] actionStates = new RealDealerState[] {
            RealDealerState.BoomerangSwords,
            RealDealerState.Bouncers,
            RealDealerState.CardAttack
    };

    private bool inSecondStage;

    [SerializeField] private Animator anim;
    private EnemyHealth health;
    private NavMeshAgent agent;
    private ObjectSpawner[] objectSpawners;

    [SerializeField] private Transform centerTransform;

    [SerializeField] private float delayBetweenStates;

    private bool ghost;
    private bool defeated;

    [Header("Debug")]
    [SerializeField] private bool debugState;
    [ConditionalHide("debugState")][SerializeField] private RealDealerState stateToDebug;
    [SerializeField] private bool debugStartSecondStage;

    private void Awake() {
        health = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
        objectSpawners = GetComponents<ObjectSpawner>();
    }

    private void OnEnable() {

        int defeatedAmount = ES3.Load("DealerDefeatedAmount", 0, ES3EncryptionMigration.GetES3Settings());
        if (defeatedAmount < 3) {
            enabled = false;
            return;
        }
        else if (defeatedAmount == 3) {
            ghost = false;
        }
        else {
            ghost = true;
        }

        health.DamagedEventTrigger.AddListener(OnDamaged);
        health.DeathEventTrigger.AddListener(OnDefeated);
        HandCard.OnAnyCardUsed_Card += OnAnyCardUsed;
        BlankMemoryCardDrop.OnPickup += RemoveInvincibility;

        // skip starting dialog if ghost
        if (ghost) {

            // delay to wait for enterBossRoomPlayer to start in BossManager > StartBossFight
            DOVirtual.DelayedCall(Time.deltaTime, () => {
                BossManager.Instance.ResumeEnterBossPlayer();
            });

            float delayToStart = 2f;
            DOVirtual.DelayedCall(delayToStart, () => {
                ChangeState(RealDealerState.BetweenStates);
                foreach (var objectSpawner in objectSpawners) objectSpawner.enabled = true;
            });
        }
        else {
            ChangeState(RealDealerState.StartingDialog);
        }

        inSecondStage = debugStartSecondStage;

        UpdateVisual();

        BecomeInvincible();

        foreach (var objectSpawner in objectSpawners) objectSpawner.enabled = false;

        defeated = false;

        // override the max health set by enemy health because it only takes the fake dealer's max health
        DOVirtual.DelayedCall(Time.deltaTime, () => {
            health.SetMaxHealth(GetEnemyStats().MaxHealth);
        });
    }

    private void OnDisable() {
        health.DamagedEventTrigger.RemoveListener(OnDamaged);
        health.DeathEventTrigger.RemoveListener(OnDefeated);
        HandCard.OnAnyCardUsed_Card -= OnAnyCardUsed;
        BlankMemoryCardDrop.OnPickup -= RemoveInvincibility;
    }

    private void Update() {

        HandleInvincibility();

        if (currentState == RealDealerState.StartingDialog ||
            currentState == RealDealerState.DefeatedDialog) {

            HandleDialog();
            return;
        }
    }

    private void ChangeToNewActionState() {
        RealDealerState[] availableStates = actionStates.Where(s => s != previousActionState).ToArray();
        ChangeState(availableStates.RandomItem());
    }

    private void ChangeState(RealDealerState newState) {

        RealDealerState previousState = currentState;
        currentState = newState;

        if (actionStates.Contains(previousState)) {
            previousActionState = previousState;
        }

        if (newState == RealDealerState.BetweenStates) {
            if (!defeated) {
                StartCoroutine(ActionStateAfterDelay());
            }
        }
        else if (newState == RealDealerState.BoomerangSwords) {
            StartCoroutine(BoomerangSwordsCor());
        }
        else if (newState == RealDealerState.Bouncers) {
            StartCoroutine(BouncerCor());
        }
        else if (newState == RealDealerState.CardAttack) {
            StartCoroutine(CardAttackCor());
        }

        if (previousState == RealDealerState.BetweenStates) {

        }
        else if (previousState == RealDealerState.BoomerangSwords) {

            float fadeDuration = 1f;

            if (heatSeekSword != null && heatSeekSword.gameObject.activeSelf) {
                SpriteRenderer heatSeekSwordRenderer = heatSeekSword.GetComponentInChildren<SpriteRenderer>();
                heatSeekSwordRenderer.DOFade(0f, fadeDuration).OnComplete(() => {
                    heatSeekSword.gameObject.ReturnToPool();
                });
            }

            foreach (MMAutoRotate swordCircle in swordCircles) {
                if (swordCircle != null && swordCircle.gameObject.activeSelf) {
                    SpriteRenderer[] spriteRenderers = swordCircle.GetComponentsInChildren<SpriteRenderer>();
                    foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
                        spriteRenderer.DOFade(0f, fadeDuration);
                    }

                    DOVirtual.DelayedCall(fadeDuration, () => {
                        swordCircle.gameObject.ReturnToPool();
                    });
                }
            }
        }
        else if (previousState == RealDealerState.Bouncers) {
            foreach (var bouncer in bouncers) {
                bouncer.GetComponent<EnemyHealth>().Die();
            }
        }
        else if (previousState == RealDealerState.CardAttack) {
        }

        OnChangeStateDialog();
    }

    private IEnumerator ActionStateAfterDelay() {
        yield return new WaitForSeconds(delayBetweenStates);

        if (debugState) {
            ChangeState(stateToDebug);
        }
        else {
            ChangeToNewActionState();
        }
    }

    private void OnDamaged() {
        if (health.HealthProportion < 0.5f) {
            inSecondStage = true;
            UpdateVisual();
        }
    }

    private void OnDefeated() {
        StopAllCoroutines();
        StartCoroutine(OnDefeatedCor());
    }
    private IEnumerator OnDefeatedCor() {
        defeated = true;

        int defeatedAmount = ES3.Load("DealerDefeatedAmount", 0, ES3EncryptionMigration.GetES3Settings()) + 1;
        ES3.Save("DealerDefeatedAmount", defeatedAmount, ES3EncryptionMigration.GetES3Settings());

        ChangeState(RealDealerState.BetweenStates);
        foreach (var objectSpawner in objectSpawners) objectSpawner.enabled = false;

        yield return new WaitForSeconds(1f);

        sparksReal.Stop();
        anim.SetTrigger("fadeOutRed");

        if (ghost) {
            anim.SetTrigger("die");
        }
        else {
            ChangeState(RealDealerState.DefeatedDialog);
        }
    }

    // played by 'death' animation
    public void Destroy() {

        if (!ghost) {
            ParticleSystem spawnedDestroyParticles = destroyParticles.Spawn(destroyParticles.transform.position, Containers.Instance.Effects);
            spawnedDestroyParticles.Play();

            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.DealerDeath);
        }

        gameObject.ReturnToPool();

        TrashBlankMemoryCards();

        SteamUserStats.SetAchievement("FinalDeal");
        SteamUserStats.StoreStats();
    }

    private void TrashBlankMemoryCards() {
        var blankMemoryCardsInDeck = DeckManager.Instance.GetCardsInDeck()
            .Where(card => card is ScriptableBlankMemoryCard)
            .ToList();

        foreach (var card in blankMemoryCardsInDeck) {
            int index = DeckManager.Instance.GetCardsInDeck().IndexOf(card);
            if (index >= 0) {
                DeckManager.Instance.TrashCard(CardLocation.Deck, index, usingCard: false);
            }
        }

        var blankMemoryCardsInDiscard = DeckManager.Instance.GetCardsInDiscard()
            .Where(card => card is ScriptableBlankMemoryCard)
            .ToList();

        foreach (var card in blankMemoryCardsInDiscard) {
            int index = DeckManager.Instance.GetCardsInDiscard().IndexOf(card);
            if (index >= 0) {
                DeckManager.Instance.TrashCard(CardLocation.Discard, index, usingCard: false);
            }
        }

        var blankMemoryCardsInHand = DeckManager.Instance.GetCardsInHand()
            .Where(card => card is ScriptableBlankMemoryCard)
            .ToList();

        foreach (var card in blankMemoryCardsInHand) {
            int index = Array.FindIndex(DeckManager.Instance.GetCardsInHand(), c => c == card);
            if (index >= 0) {
                DeckManager.Instance.TrashCard(CardLocation.Hand, index, usingCard: false);
            }
        }
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

        if (defeated) {
            return;
        }

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
    [SerializeField] private ParticleSystem sparksFake;
    [SerializeField] private ParticleSystem sparksReal;

    [SerializeField] private ParticleSystem destroyParticles;

    private void UpdateVisual() {
        anim.SetBool("ghost", ghost);

        sparksFake.gameObject.SetActive(false);
        sparksReal.gameObject.SetActive(true);

        var emission = sparksReal.emission;
        emission.enabled = inSecondStage;
    }

    #endregion

    #region Boomerang Swords

    [Header("Boomerang Swords")]
    [SerializeField] private RandomInt boomerangSwordRepetitions;

    [SerializeField] private DealerBoomerangSword boomerangSwordPrefab;

    [SerializeField] private RandomFloat boomerangDelayBeforeShoot;
    [SerializeField] private float boomerangShortShootCooldown;
    [SerializeField] private float boomerangLongShootCooldown;
    [SerializeField] private float boomerangStartingSpeed;
    [SerializeField] private float boomerangAcceleration;

    [SerializeField] private BoomerangCombination[] boomerangCombinations;
    [SerializeField] private float boomerangSpawnDistance;

    [SerializeField] private HeatSeekMovement heatSeekSwordPrefab;
    private HeatSeekMovement heatSeekSword;

    [SerializeField] private MMAutoRotate swordCirclePrefab;
    private MMAutoRotate[] swordCircles = new MMAutoRotate[2];

    private IEnumerator BoomerangSwordsCor() {

        Vector2 roomCenter = FindObjectOfType<BossRoom>().GetBossSpawnPoint().position;
        agent.SetDestination(roomCenter);

        while (Vector2.Distance(roomCenter, transform.position) > 0.1f) {
            yield return null;
        }

        agent.isStopped = true;

        heatSeekSword = heatSeekSwordPrefab.Spawn(transform.position, Containers.Instance.Projectiles);
        heatSeekSword.Setup(PlayerMovement.Instance.CenterTransform);

        EnemyTouchDamage heatSeekSwordDamage = heatSeekSword.GetComponentInChildren<EnemyTouchDamage>();
        heatSeekSwordDamage.enabled = false;

        SpriteRenderer heatSeekSwordRenderer = heatSeekSword.GetComponentInChildren<SpriteRenderer>();
        heatSeekSwordRenderer.Fade(0f);
        heatSeekSwordRenderer.DOFade(1f, duration: 1.5f).OnComplete(() => {
            heatSeekSwordDamage.enabled = true;
        });

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.SpawnSword);

        if (inSecondStage) {
            for (int i = 0; i < 2; i++) {

                Vector3 position = i == 0 ? Vector3.up * 13f : Vector3.down * 13f;
                swordCircles[i] = swordCirclePrefab.Spawn(transform.position + position, Containers.Instance.Projectiles);

                EnemyTouchDamage[] enemyTouchDamages = swordCircles[i].GetComponentsInChildren<EnemyTouchDamage>();
                foreach (EnemyTouchDamage enemyTouchDamage in enemyTouchDamages) {
                    enemyTouchDamage.enabled = false;
                }

                SpriteRenderer[] spriteRenderers = swordCircles[i].GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
                    spriteRenderer.Fade(0f);
                    spriteRenderer.DOFade(1f, duration: 1.5f).OnComplete(() => {
                        spriteRenderer.GetComponent<EnemyTouchDamage>().enabled = true;
                    });
                }

                swordCircles[i].OrbitCenterTransform = transform;
            }
        }

        boomerangSwordRepetitions.Randomize();
        for (int i = 0; i < boomerangSwordRepetitions.Value; i++) {

            bool thirdShot = i != 0 && i % 3 == 0;
            print($"{thirdShot}, {i}");

            if (thirdShot) {
                yield return new WaitForSeconds(boomerangLongShootCooldown);
            }
            else {
                yield return new WaitForSeconds(boomerangShortShootCooldown);
            }

            BoomerangCombination boomerangCombination = boomerangCombinations.RandomItem();

            int orbitDirection = UnityEngine.Random.value > 0.5f ? 1 : -1;
            boomerangDelayBeforeShoot.Randomize();

            foreach (float angle in boomerangCombination.Angles) {

                Vector2 spawnPos = (Vector2)centerTransform.position + (angle.RotationToDirection() * boomerangSpawnDistance);
                DealerBoomerangSword boomerangSword = boomerangSwordPrefab.Spawn(spawnPos, Containers.Instance.Projectiles);

                boomerangSword.Setup(
                    centerTransform,
                    boomerangCombination.Orbiting,
                    orbitDirection,
                    boomerangDelayBeforeShoot.Value,
                    boomerangStartingSpeed,
                    boomerangAcceleration
                );
            }

            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.SpawnSword);
        }

        ChangeState(RealDealerState.BetweenStates);
    }

    #endregion

    #region Bouncers

    [Header("Bouncer")]
    [SerializeField] private float bouncerStateDuration;

    [SerializeField] private Bouncer bouncerPrefab;
    [SerializeField] private int bouncerAmount1;
    [SerializeField] private int bouncerAmount2;

    [SerializeField] private Bouncer miniBouncerPrefab;
    [SerializeField] private int miniBouncerAmount;

    private List<Bouncer> bouncers;

    private IEnumerator BouncerCor() {

        bouncers = new();

        int bouncerAmount = inSecondStage ? bouncerAmount2 : bouncerAmount1;
        for (int i = 0; i < bouncerAmount; i++) {
            Vector2 pos = new RoomPositionHelper().GetRandomRoomPos(wallAvoidDistance: 3f);
            Bouncer bouncer = bouncerPrefab.Spawn(pos, Containers.Instance.Enemies);
            bouncers.Add(bouncer);
        }

        if (inSecondStage) {
            for (int i = 0; i < miniBouncerAmount; i++) {
                Vector2 pos = new RoomPositionHelper().GetRandomRoomPos(wallAvoidDistance: 3f);
                Bouncer miniBouncer = miniBouncerPrefab.Spawn(pos, Containers.Instance.Enemies);
                bouncers.Add(miniBouncer);
            }
        }

        AudioManager.Instance.PlaySingleSound(AudioManager.Instance.AudioClips.SpawningBouncers);

        yield return new WaitForSeconds(bouncerStateDuration);

        ChangeState(RealDealerState.BetweenStates);
    }


    #endregion

    #region Card Attack

    [Header("Card Attack")]
    [SerializeField] private RandomInt cardAttackRepetitions;

    [SerializeField] private CardRing cardRingPrefab;

    [SerializeField] private RandomInt cardRingAmount;
    [SerializeField] private float cardRingCooldown1;
    [SerializeField] private float cardRingCooldown2;

    [SerializeField] private float cardRingRadius;
    [SerializeField] private int amountOfCardsInRing;
    [SerializeField] private float cardRingSpeed;
    [SerializeField] private float cardRingCircularSpeed;

    [SerializeField] private StraightMovement cardPrefab;
    [SerializeField] private RandomInt circleShootAmount;

    [SerializeField] private int circleShootSpiralCount1;
    [SerializeField] private int circleShootSpiralCount2;

    [SerializeField] private float circleShootAngleBetweenCards;

    [SerializeField] private float circleShootBetweenCardDelay1;
    [SerializeField] private float circleShootBetweenCardDelay2;

    [SerializeField] private float circleShootCardSpeed1;
    [SerializeField] private float circleShootCardSpeed2;

    [SerializeField] private float afterCircleShootDelay;

    [SerializeField] private RandomInt circleShootChangeDirectionAmount;
    private int spiralDirection;

    private IEnumerator CardAttackCor() {

        Vector2 roomCenter = FindObjectOfType<BossRoom>().GetBossSpawnPoint().position;
        agent.SetDestination(roomCenter);

        while (Vector2.Distance(roomCenter, transform.position) > 0.1f) {
            yield return null;
        }

        cardAttackRepetitions.Randomize();
        for (int repIndex = 0; repIndex < cardAttackRepetitions.Value; repIndex++) {
            cardRingAmount.Randomize();
            for (int i = 0; i < cardRingAmount.Value; i++) {
                CardRing cardRing = cardRingPrefab.Spawn(centerTransform.position, Containers.Instance.Projectiles);
                cardRing.Setup(
                    cardRingRadius,
                    amountOfCardsInRing,
                    GetEnemyStats().Damage,
                    cardRingSpeed,
                    cardRingCircularSpeed
                );

                float cardRingCooldown = inSecondStage ? cardRingCooldown2 : cardRingCooldown1;
                yield return new WaitForSeconds(cardRingCooldown);

                // 50% chance of long wait between card rings
                bool lastRing = i == cardRingAmount.Value - 1;
                if (!lastRing && UnityEngine.Random.value > 0.5f) {
                    float longDelay = 3.5f;
                    yield return new WaitForSeconds(longDelay);
                }
            }

            circleShootAmount.Randomize();

            Vector2 shootDirection = Vector2.up;

            // only change direction in second stage
            circleShootChangeDirectionAmount.Randomize();
            int cardsShotSinceDirectionChange = 0;
            spiralDirection = 1;

            for (int i = 0; i < circleShootAmount.Value; i++) {
                int circleShootSpiralCount = inSecondStage ? circleShootSpiralCount2 : circleShootSpiralCount1;
                for (int j = 0; j < circleShootSpiralCount; j++) {

                    float angleBetweenSpirals = 360f / circleShootSpiralCount;
                    float spiralAngle = angleBetweenSpirals * j;
                    Vector2 spiralShootDirection = shootDirection.GetDirectionRotated(spiralAngle);

                    float spawnDistance = 3f;
                    Vector2 position = (Vector2)centerTransform.position + (spiralShootDirection * spawnDistance);
                    StraightMovement card = cardPrefab.Spawn(position, Containers.Instance.Projectiles);

                    float circleShootCardSpeed = inSecondStage ? circleShootCardSpeed2 : circleShootCardSpeed1;
                    card.Setup(spiralShootDirection, circleShootCardSpeed);
                    card.GetComponent<DamageOnContact>().Setup(GetEnemyStats().Damage, knockbackStrength: 10f);
                }

                shootDirection.RotateDirection(circleShootAngleBetweenCards * spiralDirection);

                cardsShotSinceDirectionChange++;

                if (inSecondStage) {
                    if (cardsShotSinceDirectionChange >= circleShootChangeDirectionAmount.Value) {
                        spiralDirection = -spiralDirection;

                        circleShootChangeDirectionAmount.Randomize();
                        cardsShotSinceDirectionChange = 0;
                    }
                }

                AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BasicEnemyShoot);

                float circleShootBetweenCardDelay = inSecondStage ? circleShootBetweenCardDelay2 : circleShootBetweenCardDelay1;
                yield return new WaitForSeconds(circleShootBetweenCardDelay);
            }

            yield return new WaitForSeconds(afterCircleShootDelay);
        }

        ChangeState(RealDealerState.BetweenStates);
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
                foreach (var objectSpawner in objectSpawners) objectSpawner.enabled = true;
            }
        }
        else if (currentState == RealDealerState.DefeatedDialog) {
            if (nextDialogInput.action.WasPerformedThisFrame()) {
                DialogBox.Instance.Hide();

                anim.SetTrigger("die");
            }
        }
        else {
            Debug.LogError("Trying to handle dialog when dialog state not active!");
        }
    }

    #endregion
}

[Serializable]
public struct BoomerangCombination {
    public float[] Angles;
    public bool Orbiting;
}
