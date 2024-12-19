using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour {

    [Header("Main")]
    [SerializeField] private TriggerContactTracker startTutorialTrigger;

    private BaseTutorialStep[] tutorialSteps;
    private int currentStepIndex;

    private BaseTutorialStep CurrentTutorialStep => tutorialSteps[currentStepIndex];

    private bool tutorialActive;

    [Header("Dialog Steps")]
    [SerializeField] private InputActionReference nextDialogInput;

    [Header("Combat Step")]
    [SerializeField] private ScriptableEnemy practiceEnemy;
    [SerializeField] private Transform enemySpawnPoint;

    [Header("Dash Step")]
    [SerializeField] private InputActionReference dashInput;

    private void OnEnable() {
        startTutorialTrigger.OnEnterContact += TryStartTutorial;
    }
    private void OnDisable() {
        startTutorialTrigger.OnEnterContact -= TryStartTutorial;
    }

    private void TryStartTutorial() {
        if (!tutorialActive) {
            StartTutorial();
        }
    }

    private void StartTutorial() {

        tutorialSteps = new BaseTutorialStep[] {
            new WelcomeStep(nextDialogInput), new TeachCombatStep(nextDialogInput), new SpawnEnemyStep(practiceEnemy, enemySpawnPoint),
            new DashStep(dashInput), new CardDialogStep1(nextDialogInput), new CardDialogStep2(nextDialogInput),
        };
        currentStepIndex = 0;

        //... so move to next step when this step is completed
        CurrentTutorialStep.OnStepCompleted += NextTutorialStep;
        CurrentTutorialStep.OnEnterStep();

        tutorialActive = true;
    }
    
    private void NextTutorialStep() {

        // unsub from previous step and sub to new step
        CurrentTutorialStep.OnStepCompleted -= NextTutorialStep;
        currentStepIndex++;

        // if there are no more steps complete the tutorial
        bool noMoreSteps = currentStepIndex >= tutorialSteps.Length;
        if (noMoreSteps) {
            tutorialActive = false;
            print("tutorial complete");
            return;
        }

        //... so move to next step when this step is completed
        CurrentTutorialStep.OnStepCompleted += NextTutorialStep;
        CurrentTutorialStep.OnEnterStep();
    }
}

public class BaseTutorialStep {

    public event Action OnStepCompleted;

    public virtual void OnEnterStep() {

    }

    protected virtual void CompleteStep() {
        OnStepCompleted?.Invoke();
    }
}

public class WelcomeStep : BaseTutorialStep {

    private InputActionReference nextDialogInput;

    public WelcomeStep(InputActionReference nextDialogInput) {
        this.nextDialogInput = nextDialogInput;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        string welcomeText = "Hello, I am The Dealer. I normally trade cards, but for now will guide you.";
        DialogBox.Instance.ShowText(welcomeText);

        nextDialogInput.action.performed += CompleteStep;
    }

    private void CompleteStep(InputAction.CallbackContext context) {
        nextDialogInput.action.performed -= CompleteStep;

        CompleteStep();
    }
}

public class TeachCombatStep : BaseTutorialStep {

    private InputActionReference nextDialogInput;

    public TeachCombatStep(InputActionReference nextDialogInput) {
        this.nextDialogInput = nextDialogInput;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        string combatText = "In the Card Dungeon, you need to fight off enemies. I'll spawn one in for you, so you can learn. Left click to swing your sword and kill him!";
        DialogBox.Instance.ShowText(combatText);

        nextDialogInput.action.performed += CompleteStep;
    }

    private void CompleteStep(InputAction.CallbackContext context) {
        nextDialogInput.action.performed -= CompleteStep;

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
        DialogBox.Instance.HideBox();

        EnemySpawner.Instance.SpawnEnemy(practiceEnemy.Prefab, enemySpawnPoint.position);

        Enemy.OnAnySpawn += OnEnemySpawn;
    }

    private void OnEnemySpawn(Enemy enemy) {
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

public class DashStep : BaseTutorialStep {
    private InputActionReference dashInput;

    public DashStep(InputActionReference dashInput) {
        this.dashInput = dashInput;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        string dashText = "Nice. You can also dash with right click, which can be a useful way to move around. Try it.";
        DialogBox.Instance.ShowText(dashText, showEnterText: false);

        dashInput.action.performed += CompleteStep;
    }

    private void CompleteStep(InputAction.CallbackContext context) {
        dashInput.action.performed -= CompleteStep;

        base.CompleteStep();
    }
}

public class CardDialogStep1 : BaseTutorialStep {

    private InputActionReference nextDialogInput;

    public CardDialogStep1(InputActionReference nextDialogInput) {
        this.nextDialogInput = nextDialogInput;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        string combatText = "Good. In the card dungeon, you will find magical cards. I will give a teleport card. Drag it" +
            "on the other side of this wall to the right to teleport there. You can also hold the hotkey (1), and release where" +
            "you want to teleport.";
        DialogBox.Instance.ShowText(combatText);

        nextDialogInput.action.performed += CompleteStep;
    }

    private void CompleteStep(InputAction.CallbackContext context) {
        nextDialogInput.action.performed -= CompleteStep;

        base.CompleteStep();
    }
}

public class CardDialogStep2 : BaseTutorialStep {
    private InputActionReference nextDialogInput;

    public CardDialogStep2(InputActionReference nextDialogInput) {
        this.nextDialogInput = nextDialogInput;
    }

    public override void OnEnterStep() {
        base.OnEnterStep();

        string combatText = "These are simple instructions. If you fail, I will get angry.";
        DialogBox.Instance.ShowText(combatText);

        nextDialogInput.action.performed += CompleteStep;
    }

    private void CompleteStep(InputAction.CallbackContext context) {
        nextDialogInput.action.performed -= CompleteStep;

        base.CompleteStep();
    }
}

public class Step4 : BaseTutorialStep {

}


public class Step5 : BaseTutorialStep {

}


public class Step6 : BaseTutorialStep {

}


public class Step7 : BaseTutorialStep {

}