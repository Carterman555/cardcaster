using UnityEngine;

public class UnitEffect : MonoBehaviour {

    private bool removeAfterDuration;
    private float duration;

    private void OnEnable() {
        IEffectable[] effectables = GetComponents<IEffectable>();
        foreach (IEffectable effectable in effectables) {
            effectable.OnAddEffect(this);
        }
    }

    public virtual void Setup(bool removeAfterDuration = false, float duration = 0) {
        this.removeAfterDuration = removeAfterDuration;
        this.duration = duration;
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

    // to remove all effects when unit is returned to pool
    protected virtual void OnDisable() {
        Destroy(this);
    }
}