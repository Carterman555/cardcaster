using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Tutorial : MonoBehaviour {

    public static event Action OnTutorialRoomStart;

    [Header("Main")]
    [SerializeField] private TriggerContactTracker startTutorialTrigger;

    private BaseTutorialStep[] tutorialSteps;
    private int currentStepIndex;

    private BaseTutorialStep CurrentTutorialStep => tutorialSteps[currentStepIndex];

    private bool tutorialActive;

    private static bool playerDied;

    [Header("Dialog Steps")]
    [SerializeField] private InputActionReference nextStepInput;

    private string welcomeText = "Hello, I am The Dealer. I normally trade cards, but for now will guide you.";

    private string faceTextKeyboard = "First I will teach you that you can change the direction you are facing and aim your sword with your mouse position.";
    private string faceTextController = "First I will teach you that you can change the direction you are facing with {ACTION}.";

    private string combatText = "In the Card Dungeon, you need to fight off enemies. I'll spawn one in for you," +
        " so you can learn. Press {ACTION} to swing your sword and kill him!";

    private string dashText = "Nice. You can also dash with {ACTION}, which can be a useful way to move around. You're also " +
        "invincible while dashing. Try it.";

    private string card1TextKeyboard = "Good. In the card dungeon, you will find magical cards. I will give a teleport card." +
        " Drag it on the other side of this wall to the right to teleport there.";
    private string card1TextController = "Good. In the card dungeon, you will find magical cards. I will give a teleport card." +
        " Press {ACTION}, and use the right joystick to move the card to where you want to teleport.";

    private string card2TextKeyboard = "You can also hold the hotkey ({ACTION}), and release where you want to teleport. These are" +
        " simple instructions. If you fail, I will get angry.";
    private string card2TextController = "Once it's in the correct position, press {ACTION} again to teleport. These are" +
        " simple instructions. If you fail, I will get angry.";


    private string modify1CardText = "Some cards have the magical power to modify other cards that perform abilities.";

    private string modify2CardText = "Play Scorch then Spinning Fury, and watch what happens.";

    private string essenceText = "You might have noticed that cards cost essence. Enemies drop essence and I'll give you" +
        " some now to make sure you what they look like. Pick them all up.";

    private string holeText = "See this hole I created? You should fall into it.";

    [Header("Combat Step")]
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private ScriptableEnemy practiceEnemy;
    [SerializeField] private Transform enemySpawnPoint;

    [Header("Face Step")]
    [SerializeField] private InputActionReference faceInput;

    [Header("Dash Step")]
    [SerializeField] private InputActionReference dashInput;

    [Header("Teleport Step")]
    [SerializeField] private InputActionReference firstCardInput;
    [SerializeField] private ScriptableTeleportCard teleportCard;
    [SerializeField] private TriggerContactTracker roomTwoTrigger;

    [Header("Modify Step")]
    [SerializeField] private ScriptableModifierCardBase modifierCard;
    [SerializeField] private ScriptableSwordSwingCard abilityCard;
    [SerializeField] private Transform[] modifyEnemySpawnPoints;

    [Header("Essence Step")]
    [SerializeField] private EssenceDrop essencePrefab;
    [SerializeField] private Transform[] essenceSpawnPoints;

    [Header("Hole Step")]
    [SerializeField] private GameObject hole;
    [SerializeField] private ParticleSystem createHoleParticles;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Init() {
        playerDied = false;
    }

    private void OnEnable() {
        startTutorialTrigger.OnEnterContact += TryStartTutorial;

        PlayerMovement.Instance.GetComponent<Health>().OnDeathAnimComplete += OnPlayerDeath;
    }
    private void OnDisable() {
        startTutorialTrigger.OnEnterContact -= TryStartTutorial;

        if (tutorialActive) {
            CurrentTutorialStep.OnStepCompleted -= NextTutorialStep;
        }
    }

    private void Start() {
        OnTutorialRoomStart?.Invoke();
    }

    private void TryStartTutorial() {
        if (!tutorialActive) {
            StartTutorial();
        }
    }

    private void OnPlayerDeath() {
        PlayerMovement.Instance.GetComponent<Health>().OnDeathAnimComplete -= OnPlayerDeath;
        playerDied = true;

        DeckManager.Instance.ClearDeckAndEssence();
        GameSceneManager.Instance.LoadTutorial();
    }

    private void StartTutorial() {

        tutorialActive = true;

        if (playerDied) {
            welcomeText = "Let's start this again, shall we. " + welcomeText;
        }

        tutorialSteps = new BaseTutorialStep[] {
            new DialogStep(nextStepInput, welcomeText),
            new DialogStep(nextStepInput, faceTextKeyboard, faceTextController, faceInput),
            new DialogStep(nextStepInput, combatText, attackAction),
            new SpawnEnemyStep(practiceEnemy, enemySpawnPoint),
            new EventDialogStep(PlayerMovement.Instance.OnDash, dashText, dashInput),
            new DialogStep(nextStepInput, card1TextKeyboard, card1TextController, firstCardInput),
            new DialogStep(nextStepInput, card2TextKeyboard, card2TextController, firstCardInput),
            new GiveTeleportCardStep(teleportCard, roomTwoTrigger),
            new DialogStep(nextStepInput, modify1CardText),
            new DialogStep(nextStepInput, modify2CardText),
            new GiveModifyCardStep(modifierCard, abilityCard),
            new CombatModifyCardStep(practiceEnemy, modifyEnemySpawnPoints),
            new PickupEssenceStep(essencePrefab, essenceSpawnPoints, essenceText),
            new HoleStep(hole, createHoleParticles, holeText)
        };
        currentStepIndex = 0;

        //... so move to next step when this step is completed
        CurrentTutorialStep.OnStepCompleted += NextTutorialStep;
        CurrentTutorialStep.OnEnterStep();

        Vector2 playerTutorialPos = new Vector2(-6f, 0);
        PlayerMovement.Instance.transform.position = playerTutorialPos;
    }

    private void NextTutorialStep() {

        // unsub from previous step and sub to new step
        CurrentTutorialStep.OnStepCompleted -= NextTutorialStep;
        currentStepIndex++;

        // if there are no more steps complete the tutorial
        bool noMoreSteps = currentStepIndex >= tutorialSteps.Length;
        if (noMoreSteps) {
            tutorialActive = false;
            ES3.Save("TutorialCompleted", true);

            return;
        }

        //... so move to next step when this step is completed
        CurrentTutorialStep.OnStepCompleted += NextTutorialStep;
        CurrentTutorialStep.OnEnterStep();
    }

    private void Update() {
        if (tutorialActive) {
            CurrentTutorialStep.Update();
        }
    }

    public static void ResetPlayerDied() {
        playerDied = false;
    }

    public bool InGiveCardsStep() {
        return CurrentTutorialStep is GiveModifyCardStep;
    }
}

public class BaseTutorialStep {

    public event Action OnStepCompleted;

    public virtual void OnEnterStep() {

    }

    public virtual void Update() {

    }

    protected virtual void CompleteStep() {
        OnStepCompleted?.Invoke();
    }
}

public class DialogStep : BaseTutorialStep {

    private InputActionReference nextStepInput;
    private string dialog;
    private InputActionReference dialogAction;

    public DialogStep(InputActionReference nextStepInput, string dialog, InputActionReference dialogAction = null) {
        this.nextStepInput = nextStepInput;
        this.dialog = dialog;
        this.dialogAction = dialogAction;

        multipleDialogs = false;
    }

    private string keyboardDialog;
    private string controllerDialog;
    private bool multipleDialogs;

    public DialogStep(InputActionReference nextStepInput, string keyboardDialog, string controllerDialog, InputActionReference dialogAction = null) {
        this.nextStepInput = nextStepInput;
        this.keyboardDialog = keyboardDialog;
        this.controllerDialog = controllerDialog;
        this.dialogAction = dialogAction;

        multipleDialogs = true;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        if (multipleDialogs) {
            if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Keyboard) {
                dialog = keyboardDialog;
            }
            else if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
                dialog = controllerDialog;
            }
            else {
                Debug.LogError("Could not find input scheme: " + InputManager.Instance.GetControlScheme());
            }
        }

        if (dialogAction != null) {
            string actionText = InputManager.Instance.GetBindingText(dialogAction, shortDisplayName: false);
            dialog = dialog.Replace("{ACTION}", actionText);
        }

        DialogBox.Instance.ShowText(dialog);

        nextStepInput.action.performed += CompleteStep;
    }

    private void CompleteStep(InputAction.CallbackContext context) {
        nextStepInput.action.performed -= CompleteStep;

        CompleteStep();
    }
}

public class EventDialogStep : BaseTutorialStep {

    private UnityEvent nextStepEvent;
    private string dialog;
    private InputActionReference dialogAction;

    public EventDialogStep(UnityEvent nextStepEvent, string dialog, InputActionReference dialogAction = null) {
        this.nextStepEvent = nextStepEvent;
        this.dialog = dialog;
        this.dialogAction = dialogAction;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        if (dialogAction != null) {
            string actionText = InputManager.Instance.GetBindingText(dialogAction, shortDisplayName: false);
            dialog = dialog.Replace("{ACTION}", actionText);
        }

        DialogBox.Instance.ShowText(dialog, showNextDialogText: false);

        nextStepEvent.AddListener(CompleteStep);
    }

    protected override void CompleteStep() {
        nextStepEvent.RemoveListener(CompleteStep);

        base.CompleteStep();
    }
}

public class SpawnEnemyStep : BaseTutorialStep {

    private ScriptableEnemy practiceEnemy;
    private Transform enemySpawnPoint;
    private Enemy enemyInstance;

    public SpawnEnemyStep(ScriptableEnemy practiceEnemy, Transform enemySpawnPoint) {
        this.practiceEnemy = practiceEnemy;
        this.enemySpawnPoint = enemySpawnPoint;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();
        DialogBox.Instance.Hide();

        EnemySpawner.Instance.SpawnEnemy(practiceEnemy.Prefab, enemySpawnPoint.position);

        Enemy.OnAnySpawn += OnEnemySpawn;
    }

    private void OnEnemySpawn(Enemy enemy) {
        Enemy.OnAnySpawn -= OnEnemySpawn;

        enemyInstance = enemy;
        enemy.GetComponent<Health>().OnDeath += CompleteStep;

        //... so enemy doesn't invoke enemies cleared when killed because classes that take that event are not setup and ready
        //... to have it invoked
        enemy.GetComponent<CheckEnemiesCleared>().enabled = false;
    }

    protected override void CompleteStep() {
        enemyInstance.GetComponent<Health>().OnDeath -= CompleteStep;

        base.CompleteStep();
    }
}

public class GiveTeleportCardStep : BaseTutorialStep {

    private ScriptableTeleportCard teleportCard;
    private TriggerContactTracker roomTwoTrigger;

    public GiveTeleportCardStep(ScriptableTeleportCard teleportCard, TriggerContactTracker roomTwoTrigger) {
        this.teleportCard = teleportCard;
        this.roomTwoTrigger = roomTwoTrigger;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        DialogBox.Instance.Hide();

        DeckManager.Instance.GainCard(teleportCard.CloneScriptableObject());

        roomTwoTrigger.OnEnterContact += CompleteStep;
    }

    protected override void CompleteStep() {
        roomTwoTrigger.OnEnterContact -= CompleteStep;

        Trainer.Instance.TeleportToRoomTwo();

        base.CompleteStep();
    }
}

public class GiveModifyCardStep : BaseTutorialStep {

    private ScriptableModifierCardBase modifierCard;
    private ScriptableSwordSwingCard abilityCard;

    private bool modifierPlayed;

    public GiveModifyCardStep(ScriptableModifierCardBase modifierCard, ScriptableSwordSwingCard abilityCard) {
        this.modifierCard = modifierCard;
        this.abilityCard = abilityCard;

        modifierPlayed = false;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        DialogBox.Instance.Hide();

        GiveCards();
    }

    private void GiveCards() {
        DeckManager.Instance.GainCard(modifierCard.CloneScriptableObject());
        DeckManager.Instance.GainCard(abilityCard.CloneScriptableObject());
    }

    public override void Update() {
        base.Update();

        // once the modifier card is played, the step will complete when the modifier is applied
        if (AbilityManager.Instance.IsModifierActive(modifierCard) && !modifierPlayed) {
            modifierPlayed = true;

            AbilityManager.OnApplyModifiers += CompleteStep;
        }
    }

    protected override void CompleteStep() {
        AbilityManager.OnApplyModifiers -= CompleteStep;

        base.CompleteStep();
    }
}

public class CombatModifyCardStep : BaseTutorialStep {

    private ScriptableEnemy scriptableEnemy;
    private Transform[] enemySpawnPoints;
    private Enemy[] enemyInstances;

    public CombatModifyCardStep(ScriptableEnemy scriptableEnemy, Transform[] enemySpawnPoints) {

        this.scriptableEnemy = scriptableEnemy;
        this.enemySpawnPoints = enemySpawnPoints;

        enemyInstances = new Enemy[enemySpawnPoints.Length];
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        for (int enemyIndex = 0; enemyIndex < enemySpawnPoints.Length; enemyIndex++) {
            Transform spawnPoint = enemySpawnPoints[enemyIndex];
            enemyInstances[enemyIndex] = scriptableEnemy.Prefab.Spawn(spawnPoint.position, Containers.Instance.Enemies);

            enemyInstances[enemyIndex].GetComponent<Health>().OnDeath += TryCompleteStep;

            //... so enemy doesn't invoke enemies cleared when killed because classes that take that event are not setup and ready
            //... to have it invoked
            enemyInstances[enemyIndex].GetComponent<CheckEnemiesCleared>().enabled = false;
        }
    }

    private void TryCompleteStep() {
        bool anyAlive = enemyInstances.Any(e => !e.GetComponent<Health>().IsDead());

        if (!anyAlive) {
            foreach (var enemyInstance in enemyInstances) {
                enemyInstance.GetComponent<Health>().OnDeath -= TryCompleteStep;
            }
            base.CompleteStep();
        }
    }
}

public class PickupEssenceStep : BaseTutorialStep {

    private EssenceDrop essencePrefab;
    private Transform[] essenceSpawnPoints;

    private EssenceDrop[] essenceInstances;

    private string dialog;

    public PickupEssenceStep(EssenceDrop essencePrefab, Transform[] essenceSpawnPoints, string dialog) {
        this.essencePrefab = essencePrefab;
        this.essenceSpawnPoints = essenceSpawnPoints;
        this.dialog = dialog;

        essenceInstances = new EssenceDrop[essenceSpawnPoints.Length];
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        DialogBox.Instance.ShowText(dialog, showNextDialogText: false);

        DropEssence();

        DeckManager.OnEssenceChanged_Amount += TryCompleteStep;
    }

    private void DropEssence() {
        for (int i = 0; i < essenceSpawnPoints.Length; i++) {
            essenceInstances[i] = essencePrefab.Spawn(essenceSpawnPoints[i].position, Containers.Instance.Drops);
        }
    }

    private void TryCompleteStep(float n) => DeckManager.Instance.StartCoroutine(TryCompleteStep());

    private IEnumerator TryCompleteStep() {

        //... wait a frame for essence that just got picked up to get disabled
        yield return null;

        bool anyEssenceLeft = essenceInstances.Any(e => e.isActiveAndEnabled);
        if (!anyEssenceLeft) {
            DeckManager.OnEssenceChanged_Amount -= TryCompleteStep;

            base.CompleteStep();
        }
    }
}

public class HoleStep : BaseTutorialStep {

    private GameObject hole;
    private ParticleSystem createHoleParticles;

    private string dialog;

    public HoleStep(GameObject hole, ParticleSystem createHoleParticles, string dialog) {
        this.hole = hole;
        this.createHoleParticles = createHoleParticles;

        this.dialog = dialog;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        DialogBox.Instance.ShowText(dialog, showNextDialogText: false);

        createHoleParticles.Play();

        DialogBox.Instance.StartCoroutine(ActivateHole());

        NextLevelHole.OnFallInHole += CompleteStep;
    }

    private IEnumerator ActivateHole() {

        float activateHoleDelay = 0.1f;
        yield return new WaitForSeconds(activateHoleDelay);

        hole.SetActive(true);
    }

    protected override void CompleteStep() {
        NextLevelHole.OnFallInHole -= CompleteStep;

        base.CompleteStep();
    }
}