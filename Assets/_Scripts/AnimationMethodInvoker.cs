using UnityEngine;
using UnityEngine.Events;

public class AnimationMethodInvoker : MonoBehaviour {

    [SerializeField] private UnityEvent[] animationEvents;

    private void InvokeEvent(int eventNumber) {
        animationEvents[eventNumber]?.Invoke();
    }
}
