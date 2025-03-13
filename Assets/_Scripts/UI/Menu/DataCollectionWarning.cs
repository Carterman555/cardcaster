using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataCollectionWarning : MonoBehaviour {

    private static bool showedScreen;

    [SerializeField] private InputAction acceptAction;
    [SerializeField] private InputAction declineAction;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI instructionsText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        showedScreen = false;
    }

    private void Awake() {
        if (showedScreen) {
            gameObject.SetActive(false);
            mainMenu.SetActive(true);
        }
        else {
            mainMenu.SetActive(false);
        }
    }

    private void OnEnable() {
        InputManager.OnControlSchemeChanged += UpdateInstructions;
    }

    private void OnDisable() {
        InputManager.OnControlSchemeChanged -= UpdateInstructions;
    }

    private void Start() {
        acceptAction.Enable();
        declineAction.Enable();

        UpdateInstructions();
    }

    private void Update() {
        if (declineAction.triggered) {
            Application.Quit();
        }
        if (acceptAction.triggered) {
            canvasGroup.DOFade(0f, duration: 0.3f).OnComplete(() => {
                gameObject.SetActive(false);
                mainMenu.SetActive(true);
            });

            showedScreen = true;
        }
    }

    private void UpdateInstructions() {
        instructionsText.text = $"Press {InputManager.Instance.GetBindingText(declineAction)} to exit." +
            $" If you agree, you can continue by pressing '{InputManager.Instance.GetBindingText(acceptAction)}'";
    }
}
