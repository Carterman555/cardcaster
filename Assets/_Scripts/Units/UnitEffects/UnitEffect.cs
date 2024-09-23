using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEffect : MonoBehaviour {

    private static Dictionary<System.Guid, UnitEffect> unitEffectIds = new();
    public static void RemoveEffect(System.Guid effectToRemoveId) {
        
        if (!unitEffectIds.ContainsKey(effectToRemoveId)) {
            Debug.LogError("Trying To Remove Effect With Invalid ID!");
        }

        Destroy(unitEffectIds[effectToRemoveId]);
        unitEffectIds.Remove(effectToRemoveId);
    }

    private bool removeAfterDuration;
    private float duration;

    private System.Guid id;

    public virtual System.Guid Setup(bool removeAfterDuration = false, float duration = 0) {
        this.removeAfterDuration = removeAfterDuration;
        this.duration = duration;

        GetComponent<IEffectable>().OnAddEffect(this);

        id = System.Guid.NewGuid();
        return id;
    }

    protected virtual void Update() {
        if (removeAfterDuration) {
            duration -= Time.deltaTime;
            if (duration < 0) {
                Destroy(this);
            }
        }
    }

    private void OnDisable() {
        Destroy(this);
    }
}