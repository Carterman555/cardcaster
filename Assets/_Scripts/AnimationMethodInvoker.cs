using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AnimationMethodInvoker : MonoBehaviour {

    [SerializeField] private AnimationEvent[] animationEvents;
    
    // played by animations
    private void InvokeEvent(string key) {

        if (string.IsNullOrEmpty(key)) {
            Debug.LogWarning($"Empty animation key on {transform.parent.name} visual!");
        }

        AnimationEvent animationEvent = animationEvents.FirstOrDefault(e => e.Key == key);

        if (animationEvent.Event == null) {
            //Debug.LogWarning($"Animation event does not have event! {key} on {transform.parent.name} visual!");
        }

        animationEvent.Event?.Invoke();
    }
}

[Serializable]
public struct AnimationEvent {
    public string Key;
    public UnityEvent Event;
}
