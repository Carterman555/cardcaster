using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour {

    [Header("Dialog")]
    [SerializeField] private TriggerContactTracker dialogTrigger;
    [SerializeField] private InputActionReference nextDialogInput;

    private string welcomeText = "Hello, I am The Dealer. I normally trade cards, but for now will guide you.";

    private void OnEnable() {
        dialogTrigger.OnEnterContact_GO += StartTutorial;
    }
    private void OnDisable() {
        dialogTrigger.OnEnterContact_GO -= StartTutorial;
    }

    private void StartTutorial(GameObject triggerObject) {
        DialogBox.Instance.ShowText(welcomeText);
    }
}
