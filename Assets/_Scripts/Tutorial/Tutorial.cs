using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class Tutorial : MonoBehaviour {

    public static event Action OnTutorialRoomStart;

    [Header("Main")]
    [SerializeField] private TriggerEventInvoker startTutorialTrigger;

    private BaseTutorialStep[] tutorialSteps;
    private int currentStepIndex;

    private BaseTutorialStep CurrentTutorialStep => tutorialSteps[currentStepIndex];

    private bool tutorialActive;

    private static bool playerDied;

    [Header("Dialog Steps")]
    [SerializeField] private InputActionReference nextStepInput;

    [SerializeField] private LocalizedString welcomeLocString;
    [SerializeField] private LocalizedString playerDiedLocString;
    [SerializeField] private LocalizedString faceKeyboardLocString;
    [SerializeField] private LocalizedString faceControllerLocString;
    [SerializeField] private LocalizedString combatLocString;
    [SerializeField] private LocalizedString dashLocString;
    [SerializeField] private LocalizedString card1KeyboardLocString;
    [SerializeField] private LocalizedString card1ControllerLocString;
    [SerializeField] private LocalizedString card2KeyboardLocString;
    [SerializeField] private LocalizedString card2ControllerLocString;
    [SerializeField] private LocalizedString modifyCard1LocString;
    [SerializeField] private LocalizedString modifyCard2LocString;
    [SerializeField] private LocalizedString essenceLocString;
    [SerializeField] private LocalizedString holeLocString;

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
    [SerializeField] private ScriptableSpinningFuryCard abilityCard;
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
        startTutorialTrigger.OnTriggerEnter += TryStartTutorial;
        PlayerMovement.Instance.GetComponent<PlayerHealth>().OnDeathAnimComplete += OnPlayerDeath;

        Vector2 playerTutorialPos = new Vector2(-6f, 0);
        PlayerMovement.Instance.GetComponent<Rigidbody2D>().MovePosition(playerTutorialPos);

        Invoke(nameof(EnableStartTrigger), Time.deltaTime);
    }

    // it wasn't enabled at the start because the player is touching it before it's repositioned to playerTutorialPos 
    private void EnableStartTrigger() {
        startTutorialTrigger.GetComponent<Collider2D>().enabled = true;
    }

    private void OnDisable() {
        startTutorialTrigger.OnTriggerEnter -= TryStartTutorial;

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
        PlayerMovement.Instance.GetComponent<PlayerHealth>().OnDeathAnimComplete -= OnPlayerDeath;
        playerDied = true;

        DeckManager.Instance.ClearDeckAndEssence();
        GameSceneManager.Instance.LoadTutorial();
    }

    private void StartTutorial() {

        tutorialActive = true;

        LocalizedString correctWelcomeText = playerDied ? playerDiedLocString : welcomeLocString;

        tutorialSteps = new BaseTutorialStep[] {
            new DialogStep(nextStepInput, correctWelcomeText),
            new DialogStep(nextStepInput, faceKeyboardLocString, faceControllerLocString, faceInput),
            new DialogStep(nextStepInput, combatLocString, attackAction),
            new SpawnEnemyStep(practiceEnemy, enemySpawnPoint),
            new EventDialogStep(PlayerMovement.Instance.OnDash, dashLocString, dashInput),
            new DialogStep(nextStepInput, card1KeyboardLocString, card1ControllerLocString, firstCardInput),
            new DialogStep(nextStepInput, card2KeyboardLocString, card2ControllerLocString, firstCardInput),
            new GiveTeleportCardStep(teleportCard, roomTwoTrigger),
            new DialogStep(nextStepInput, modifyCard1LocString),
            new DialogStep(nextStepInput, modifyCard2LocString),
            new GiveModifyCardStep(modifierCard, abilityCard),
            new CombatModifyCardStep(practiceEnemy, modifyEnemySpawnPoints),
            new PickupEssenceStep(essencePrefab, essenceSpawnPoints, essenceLocString),
            new HoleStep(hole, createHoleParticles, holeLocString)
        };
        currentStepIndex = 0;

        //... so move to next step when this step is completed
        CurrentTutorialStep.OnStepCompleted += NextTutorialStep;
        CurrentTutorialStep.OnEnterStep();
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
    private LocalizedString dialog;
    private InputActionReference dialogAction;

    public DialogStep(InputActionReference nextStepInput, LocalizedString dialog, InputActionReference dialogAction = null) {
        this.nextStepInput = nextStepInput;
        this.dialog = dialog;
        this.dialogAction = dialogAction;
        inputDependentDialog = false;
    }

    private LocalizedString keyboardDialog;
    private LocalizedString controllerDialog;
    private bool inputDependentDialog;

    public DialogStep(InputActionReference nextStepInput, LocalizedString keyboardDialog, LocalizedString controllerDialog, InputActionReference dialogAction = null) {
        this.nextStepInput = nextStepInput;
        this.keyboardDialog = keyboardDialog;
        this.controllerDialog = controllerDialog;
        this.dialogAction = dialogAction;
        inputDependentDialog = true;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        LocalizedString locDialog;
        if (inputDependentDialog) {
            if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Keyboard) {
                locDialog = keyboardDialog;
            }
            else if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
                locDialog = controllerDialog;
            }
            else {
                Debug.LogWarning("No matching control scheme type!");
                locDialog = dialog;
            }
        }
        else {
            locDialog = dialog;
        }

        DialogBox.Instance.ShowText(locDialog, dialogAction: dialogAction);

        nextStepInput.action.performed += CompleteStep;
    }

    private void CompleteStep(InputAction.CallbackContext context) {
        nextStepInput.action.performed -= CompleteStep;

        CompleteStep();
    }
}

public class EventDialogStep : BaseTutorialStep {

    private UnityEvent nextStepEvent;
    private LocalizedString dialog;
    private InputActionReference dialogAction;

    public EventDialogStep(UnityEvent nextStepEvent, LocalizedString dialog, InputActionReference dialogAction = null) {
        this.nextStepEvent = nextStepEvent;
        this.dialog = dialog;
        this.dialogAction = dialogAction;
    }

    private LocalizedString keyboardDialog;
    private LocalizedString controllerDialog;
    private bool inputDependentDialog;

    public EventDialogStep(UnityEvent nextStepEvent, LocalizedString keyboardDialog, LocalizedString controllerDialog, InputActionReference dialogAction = null) {
        this.nextStepEvent = nextStepEvent;
        this.keyboardDialog = keyboardDialog;
        this.controllerDialog = controllerDialog;
        this.dialogAction = dialogAction;
        inputDependentDialog = true;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        LocalizedString locDialog;
        if (inputDependentDialog) {
            if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Keyboard) {
                locDialog = keyboardDialog;
            }
            else if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
                locDialog = controllerDialog;
            }
            else {
                Debug.LogWarning("No matching control scheme type!");
                locDialog = dialog;
            }
        }
        else {
            locDialog = dialog;
        }

        DialogBox.Instance.ShowText(locDialog, showNextDialogText: false, dialogAction);

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
        enemy.GetComponent<EnemyHealth>().DeathEventTrigger.AddListener(CompleteStep);

        //... so enemy doesn't invoke enemies cleared when killed because classes that take that event are not setup and ready
        //... to have it invoked
        enemy.GetComponent<CheckEnemiesCleared>().enabled = false;
    }

    protected override void CompleteStep() {
        enemyInstance.GetComponent<EnemyHealth>().DeathEventTrigger.RemoveListener(CompleteStep);

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
    private ScriptableSpinningFuryCard abilityCard;

    private bool modifierPlayed;

    public GiveModifyCardStep(ScriptableModifierCardBase modifierCard, ScriptableSpinningFuryCard abilityCard) {
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

            enemyInstances[enemyIndex].GetComponent<EnemyHealth>().DeathEventTrigger.AddListener(TryCompleteStep);

            //... so enemy doesn't invoke enemies cleared when killed because classes that take that event are not setup and ready
            //... to have it invoked
            enemyInstances[enemyIndex].GetComponent<CheckEnemiesCleared>().enabled = false;
        }
    }

    private void TryCompleteStep() {
        bool anyAlive = enemyInstances.Any(e => !e.GetComponent<EnemyHealth>().Dead);

        if (!anyAlive) {
            foreach (var enemyInstance in enemyInstances) {
                enemyInstance.GetComponent<EnemyHealth>().DeathEventTrigger.RemoveListener(TryCompleteStep);
            }
            base.CompleteStep();
        }
    }
}

public class PickupEssenceStep : BaseTutorialStep {

    private EssenceDrop essencePrefab;
    private Transform[] essenceSpawnPoints;

    private EssenceDrop[] essenceInstances;
    private LocalizedString dialog;

    public PickupEssenceStep(EssenceDrop essencePrefab, Transform[] essenceSpawnPoints, LocalizedString dialog) {
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

    private void TryCompleteStep(int n) => DeckManager.Instance.StartCoroutine(TryCompleteStep());

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

    private LocalizedString dialog;

    public HoleStep(GameObject hole, ParticleSystem createHoleParticles, LocalizedString dialog) {
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

[Serializable]
public struct DialogVersion {
    public string Text;
    public ControlSchemeType Input;
    public LocaleIdentifier LanguageIdentifier;
}

[Serializable]
public struct DialogStepText {
    public DialogVersion[] DialogVersions;

    public string GetText(ControlSchemeType input, LocaleIdentifier languageIdentifier) {
        DialogVersion dialogVersion = DialogVersions.FirstOrDefault(d => ((d.Input == ControlSchemeType.Any) || d.Input == input) &&
                                                                            d.LanguageIdentifier == languageIdentifier);
        return dialogVersion.Text;
    }
}
