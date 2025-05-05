using System;
using UnityEngine;

public class MonoBehaviourEventInvoker : MonoBehaviour {

    public event Action<GameObject> OnAwake;
    public event Action<GameObject> OnStart;
    public event Action<GameObject> OnEnabled;
    public event Action<GameObject> OnDisabled;
    public event Action<GameObject> OnDestroyed;

    private void Awake() {
        OnAwake?.Invoke(gameObject);
    }

    private void Start() {
        OnStart?.Invoke(gameObject);
    }

    private void OnEnable() {
        OnEnabled?.Invoke(gameObject);
    }

    private void OnDisable() {
        OnDisabled?.Invoke(gameObject);
    }

    private void OnDestroy() {
        OnDestroyed?.Invoke(gameObject);
    }
}
