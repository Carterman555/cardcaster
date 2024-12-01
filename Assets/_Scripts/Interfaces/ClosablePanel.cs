using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// When escape key is pressed, close the panel that last became active
/// </summary>
public class ClosablePanel : MonoBehaviour {

    private static List<ClosablePanel> activeClosables;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Init() {
        activeClosables = new();
    }

    [SerializeField] private InputActionReference closeInput;
    [SerializeField] private UnityEvent onClose;

    private bool ThisIsInstanceToClose() {
        return this == activeClosables.Last();
    }

    private void OnEnable() {
        activeClosables.Add(this);
    }
    private void OnDisable() {
        activeClosables.Remove(this);
    }

    private void Update() {

        if (ThisIsInstanceToClose()) {
            if (closeInput.action.triggered) {
                onClose?.Invoke();
            }
        }
    }
}
