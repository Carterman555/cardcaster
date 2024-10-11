using Mono.CSharp;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MergeBehavior : EnemyBehavior {

    public event Action OnLeaderMerged;
    public event Action OnMerged;

    private TriggerContactTracker mergeTracker;
    private List<IMergable> nearbyMergables = new();
    private IMergable mergingPartner;

    private float toMergeDelay;
    private float mergeTime;

    private float mergeTimer; // used for both timing to be ready to merge and timing how long the merging takes

    private enum MergeStage { MergingNotAllowed, MergingAllowed, ReadyToMerging, Merging }

    private MergeStage mergeStage;

    //... so only one stronger enemy is spawned, the leader is the one that spawns the stronger enemy
    private bool isMergeLeader;
    private Enemy mergedEnemyPrefab;

    private Rigidbody2D rb;

    public MergeBehavior(Enemy enemy, TriggerContactTracker mergeTracker, Enemy mergedEnemyPrefab, float toMergeDelay, float mergeTime) : base(enemy) {

        this.mergeTracker = mergeTracker;
        this.mergedEnemyPrefab = mergedEnemyPrefab;
        this.toMergeDelay = toMergeDelay;
        this.mergeTime = mergeTime;

        if (enemy.TryGetComponent(out Rigidbody2D rigidbody2D)) {
            rb = rigidbody2D;
        }
        else {
            Debug.LogError("Object With Merge Behavior Does Not Have Rigidbody2D!");
        }

        
    }

    private DebugText debugText;//debug

    public bool IsReadyToMerge() {
        return mergeStage == MergeStage.ReadyToMerging;
    }

    public bool SameMergePrefab(Enemy mergedEnemyPrefab) {
        return this.mergedEnemyPrefab.Equals(mergedEnemyPrefab);
    }

    public bool IsMerging() {
        return mergeStage == MergeStage.Merging;
    }

    public bool IsMergeLeader() {
        return isMergeLeader;
    }

    public float GetMergeProgress() {

        if (!IsMerging()) {
            Debug.LogWarning("Trying to get progress of merging when not merging!");
            return 0f;
        }

        return mergeTimer / mergeTime;
    }

    public IMergable GetMergingPartner() {
        return mergingPartner;
    }

    public override void OnEnable() {
        base.OnEnable();

        mergeTimer = 0;
        mergeStage = MergeStage.MergingNotAllowed;
        isMergeLeader = false;

        debugText = DebugText.Create(enemy.transform, new Vector2(0, 1), mergeStage.ToString());//debug

        mergeTracker.OnEnterContact += TryAddMergable;
        mergeTracker.OnExitContact += TryRemoveMergable;
    }

    public override void OnDisable() {
        base.OnDisable();
        mergeTracker.OnEnterContact -= TryAddMergable;
        mergeTracker.OnExitContact -= TryRemoveMergable;

        debugText.gameObject.ReturnToPool(); // debug
    }

    private void TryAddMergable(GameObject @object) {
        if (@object.TryGetComponent(out IMergable mergable)) {

            // if they are the same enemy, they can merge
            if (mergable.GetType().Equals(enemy.GetType())) {
                nearbyMergables.Add(mergable);
            }
        }
    }

    private void TryRemoveMergable(GameObject @object) {
        if (@object.TryGetComponent(out IMergable mergable)) {
            if (nearbyMergables.Contains(mergable)) {
                nearbyMergables.Remove(mergable);
            }
        }
    }

    public override void FrameUpdateLogic() {
        base.FrameUpdateLogic();

        debugText.SetText(mergeStage.ToString()); // debug

        if (mergeStage == MergeStage.MergingAllowed) {
            mergeTimer += Time.deltaTime;
            if (mergeTimer > toMergeDelay) {
                mergeTimer = 0;
                mergeStage = MergeStage.ReadyToMerging;
            }
        }
        else if (mergeStage == MergeStage.ReadyToMerging) {

            foreach (IMergable nearbyMergable in nearbyMergables) {

                MergeBehavior nearbyMergeBehavior = nearbyMergable.GetMergeBehavior();

                if (nearbyMergeBehavior.IsReadyToMerge() && nearbyMergeBehavior.SameMergePrefab(mergedEnemyPrefab)) {

                    StartMerging(nearbyMergable);
                    nearbyMergeBehavior.StartMerging(this as IMergable);

                    isMergeLeader = true;
                    break;
                }
            }
        }
        else if (mergeStage == MergeStage.Merging) {
            rb.velocity = Vector2.zero;

            mergeTimer += Time.deltaTime;
            if (mergeTimer > mergeTime) {
                mergeTimer = 0;

                Merge();
            }
        }
    }

    public void AllowMerging() {
        if (mergeStage == MergeStage.MergingNotAllowed) {
            mergeStage = MergeStage.MergingAllowed;
            mergeTimer = 0;
        }
    }
    public void DontAllowMerging() {
        mergeStage = MergeStage.MergingNotAllowed;
        mergeTimer = 0;
    }

    public void StartMerging(IMergable other) {
        mergeStage = MergeStage.Merging;
        mergingPartner = other;
    }

    public void Merge() {

        if (mergedEnemyPrefab == null) {
            return;
        }

        // so only one stronger enemy is spawned
        if (isMergeLeader) {

            //... the merged enemy pos is between the two merging enemies
            Vector2 mergedEnemyPos = (enemy.transform.position + mergingPartner.GetObject().transform.position) / 2;
            mergedEnemyPrefab.Spawn(mergedEnemyPos, Containers.Instance.Enemies);

            OnLeaderMerged?.Invoke();
        }

        OnMerged?.Invoke();

        enemy.gameObject.ReturnToPool();
    }
}
