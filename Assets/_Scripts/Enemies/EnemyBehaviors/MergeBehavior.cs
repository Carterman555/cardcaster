using System;
using System.Collections.Generic;
using UnityEngine;

public class MergeBehavior : MonoBehaviour {

    public event Action OnLeaderMerged;
    public event Action OnMerged;

    [SerializeField] private TriggerContactTracker mergeTracker;
    private List<MergeBehavior> nearbyMergables = new();
    private MergeBehavior mergingPartner;

    [SerializeField] private float toMergeDelay;
    [SerializeField] private float mergeTime;
    private float mergeTimer; // used for both timing to be ready to merge and timing how long the merging takes

    private enum MergeStage { MergingNotAllowed, MergingAllowed, ReadyToMerging, Merging }
    private MergeStage mergeStage;

    //... so only one stronger enemy is spawned, the leader is the one that spawns the stronger enemy
    private bool isMergeLeader;

    [SerializeField] private Enemy mergedEnemyPrefab;

    private Rigidbody2D rb;

    [Header("Merging Indicator")]
    [SerializeField] private FillController mergeIndicatorPrefab;
    private FillController mergeIndicator;
    private bool isHandlingIndicator;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        mergeTimer = 0;
        mergeStage = MergeStage.MergingNotAllowed;
        isMergeLeader = false;
        mergingPartner = null;

        AllowMerging();

        mergeTracker.OnEnterContact_GO += TryAddMergable;
        mergeTracker.OnExitContact_GO += TryRemoveMergable;
    }

    private void OnDisable() {
        mergeTracker.OnEnterContact_GO -= TryAddMergable;
        mergeTracker.OnExitContact_GO -= TryRemoveMergable;
    }

    private void TryAddMergable(GameObject @object) {
        if (@object.TryGetComponent(out MergeBehavior mergable)) {

            // if they merge to the same enemy, they can merge
            if (mergable.SameMergePrefab(mergedEnemyPrefab)) {
                nearbyMergables.Add(mergable);
            }
        }
    }

    private void TryRemoveMergable(GameObject @object) {
        if (@object.TryGetComponent(out MergeBehavior mergable)) {
            if (nearbyMergables.Contains(mergable)) {
                nearbyMergables.Remove(mergable);
            }
        }
    }

    private void Update() {

        if (mergeStage == MergeStage.MergingAllowed) {
            mergeTimer += Time.deltaTime;
            if (mergeTimer > toMergeDelay) {
                mergeTimer = 0;
                mergeStage = MergeStage.ReadyToMerging;
            }
        }
        else if (mergeStage == MergeStage.ReadyToMerging) {

            foreach (MergeBehavior nearbyMergeBehavior in nearbyMergables) {

                if (nearbyMergeBehavior.IsReadyToMerge() && nearbyMergeBehavior.SameMergePrefab(mergedEnemyPrefab)) {

                    StartMerging(nearbyMergeBehavior);
                    nearbyMergeBehavior.StartMerging(this);

                    isMergeLeader = true;
                    break;
                }
            }
        }
        else if (mergeStage == MergeStage.Merging) {
            rb.velocity = Vector2.zero;

            // only the merge leader handles the indicator so there aren't two
            if (isMergeLeader) {
                HandleMergeIndicator();
            }

            mergeTimer += Time.deltaTime;
            if (mergeTimer > mergeTime) {
                mergeTimer = 0;

                Merge();
            }
        }
    }

    private void StartMerging(MergeBehavior other) {
        mergeStage = MergeStage.Merging;
        mergingPartner = other;
    }

    private void Merge() {

        // so only one stronger enemy is spawned
        if (isMergeLeader) {

            //... the merged enemy pos is between the two merging enemies
            Vector2 mergedEnemyPos = (transform.position + mergingPartner.transform.position) / 2;
            mergedEnemyPrefab.Spawn(mergedEnemyPos, Containers.Instance.Enemies);

            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Merge);

            OnLeaderMerged?.Invoke();
        }

        if (isHandlingIndicator) {
            DestroyMergingIndicator();
        }

        mergeStage = MergeStage.MergingNotAllowed;

        OnMerged?.Invoke();

        gameObject.ReturnToPool();
    }

    #region Handle Merge Indicator

    /// <summary>
    /// the merge leader creates an indicator to show the progress of the merging
    /// </summary>
    private void HandleMergeIndicator() {

        if (!isHandlingIndicator) {
            isHandlingIndicator = true;

            Vector3 offset = new Vector3(0, 1.5f);
            mergeIndicator = mergeIndicatorPrefab.Spawn(transform.position + offset, Containers.Instance.WorldUI);
            mergeIndicator.SetFillAmount(0);
        }
        else if (isHandlingIndicator) {
            float mergeProgress = mergeTimer / mergeTime;
            mergeIndicator.SetFillAmount(mergeProgress);
        }
    }

    // destroy the indicator when the merging is complete
    private void DestroyMergingIndicator() {
        mergeIndicator.gameObject.ReturnToPool();
        isHandlingIndicator = false;
    }

    #endregion

    #region Get Methods

    public bool IsReadyToMerge() {
        return mergeStage == MergeStage.ReadyToMerging;
    }

    public bool SameMergePrefab(Enemy mergedEnemyPrefab) {
        return this.mergedEnemyPrefab.Equals(mergedEnemyPrefab);
    }

    public bool IsMerging() {
        return mergeStage == MergeStage.Merging;
    }

    #endregion


    #region Interface Control Methods

    public void StopMerging(bool stopPartner = true) {

        if (!IsMerging()) {
            Debug.LogWarning("Trying to stop merging, but already not merging");
        }

        mergeStage = MergeStage.MergingAllowed;
        mergeTimer = 0;

        if (stopPartner) {
            mergingPartner.StopMerging(false);
        }

        if (isHandlingIndicator) {
            DestroyMergingIndicator();
        }

        mergingPartner = null;
    }

    public void AllowMerging() {
        if (mergeStage == MergeStage.MergingNotAllowed) {
            mergeStage = MergeStage.MergingAllowed;
            mergeTimer = 0;
        }
    }
    public void DontAllowMerging() {

        if (IsMerging()) {
            return;
        }

        mergeStage = MergeStage.MergingNotAllowed;
        mergeTimer = 0;
    }

    #endregion
}
