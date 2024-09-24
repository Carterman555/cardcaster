using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEffect : MonoBehaviour {

    private bool removeAfterDuration;
    private float duration;

    public virtual void Setup(bool removeAfterDuration = false, float duration = 0) {
        this.removeAfterDuration = removeAfterDuration;
        this.duration = duration;

        GetComponent<IEffectable>().OnAddEffect(this);
    }

    public void SetDuration(float duration) {
        this.duration = duration;
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