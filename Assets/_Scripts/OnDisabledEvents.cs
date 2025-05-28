using UnityEngine;
using UnityEngine.Events;

public class OnDisabledEvents : MonoBehaviour {

    [SerializeField] private UnityEvent OnDisabled;

    private void OnDisable() {
        OnDisabled?.Invoke();
    }
}
