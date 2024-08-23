using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeBehavior : EnemyBehavior {

    private TriggerContactTracker mergeTracker;
    private float toMergeDelay;
    private float toMergeTimer;

    private float mergingTime;
    private float mergingTimer;

    private bool canMerge;

    private bool merging;

    public void Setup(TriggerContactTracker mergeTracker, float toMergeDelay) {
        this.mergeTracker = mergeTracker;
        this.toMergeDelay = toMergeDelay;

        toMergeTimer = 0;
    }

    public override void FrameUpdateLogic() {
        base.FrameUpdateLogic();

        if (canMerge) {
            toMergeTimer += Time.deltaTime;
            if (toMergeTimer > toMergeDelay) {
                toMergeTimer = 0;
                StartMerging();
            }
        }

        if (merging) {
            mergingTimer += Time.deltaTime;
            if (mergingTimer > toMergeTimer) {
                mergingTimer = 0;
                Merge();
            }
        }
    }

    private void StartMerging() {
        canMerge = false;
        merging = true;

        mergingTimer = 0;
    }

    private void Merge() {
        merging = false;


    }

    public void AllowMerging() {
        canMerge = true;
        toMergeTimer = 0;
    }
    public void DontAllowMerging() {
        canMerge = false;
    }
}
