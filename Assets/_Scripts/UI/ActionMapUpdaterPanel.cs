using System;
using UnityEngine;

public class ActionMapUpdaterPanel : MonoBehaviour {

    public static event Action OnAnyActiveChanged;

    private void OnEnable() {
        OnAnyActiveChanged?.Invoke();
    }

    private void OnDisable() {
        OnAnyActiveChanged?.Invoke();
    }
}
