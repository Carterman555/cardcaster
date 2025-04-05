using UnityEngine;

public class UnitBehavior {
    protected GameObject gameObject;
    protected IHasEnemyStats hasStats;

    //private bool stopped;

    protected UnitBehavior(GameObject gameObject, IHasEnemyStats hasStats) {
        this.gameObject = gameObject;
        this.hasStats = hasStats;
    }

    //public virtual void FrameUpdateLogic() { }

    //public virtual void PhysicsUpdateLogic() { }

    //public virtual void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) { }

    //public virtual void OnDisable() { }

    //public virtual void Start() {
    //    stopped = false;
    //}

    //public virtual void Stop() {
    //    stopped = true;
    //}

    //public bool IsStopped() {
    //    return stopped;
    //}
}
