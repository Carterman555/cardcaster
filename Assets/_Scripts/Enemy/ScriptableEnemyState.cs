using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableEnemyState : ScriptableObject {

    protected Enemy enemy;

    public virtual void Initialize(Enemy enemy) {
        this.enemy = enemy;
    }

    public virtual void DoEnterLogic() { }

    public virtual void DoExitLogic() { }

    public virtual void FrameUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) { }
}