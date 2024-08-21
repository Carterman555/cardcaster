using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior {

    protected Enemy enemy;

    public virtual void Initialize(Enemy enemy) {
        this.enemy = enemy;
    }

    public virtual void FrameUpdateLogic() { }

    public virtual void PhysicsUpdateLogic() { }

    public virtual void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) { }
}
