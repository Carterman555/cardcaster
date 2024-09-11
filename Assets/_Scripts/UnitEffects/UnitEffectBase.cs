using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class UnitEffectBase {

    protected Enemy enemy;
    private bool removeAfterDuration;
    private float duration;

    public virtual void OnEffectAdded(Enemy enemy, bool removeAfterDuration = false, float duration = 0) {
        this.enemy = enemy;
        this.removeAfterDuration = removeAfterDuration;
        this.duration = duration;
    }

    public virtual void OnEffectRemoved() { }

    public virtual void UpdateLogic() {
        if (removeAfterDuration) {
            duration -= Time.deltaTime;
            if (duration < 0) {
                enemy.RemoveEffect(this);
            }
        }
    }
}

public class StopMovement : UnitEffectBase {

    public override void OnEffectAdded(Enemy enemy, bool removeAfterDuration = false, float duration = 0) {
        base.OnEffectAdded(enemy, removeAfterDuration, duration);

        if (enemy.TryGetComponent(out Rigidbody2D rb)) {
            rb.velocity = Vector3.zero;
        }
        if (enemy.TryGetComponent(out NavMeshAgent agent)) {
            agent.isStopped = true;
        }
    }

}

public class Burn : UnitEffectBase {


}