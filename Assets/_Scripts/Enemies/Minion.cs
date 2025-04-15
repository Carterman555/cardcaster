using System;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class Minion : Enemy {

    private ChasePlayerBehavior moveBehavior;
    private CircleSlashBehavior attackBehavior;
    private MergeBehavior mergeBehavior;

    [SerializeField] private bool isMergable;

    [SerializeField] private bool splitOnDeath;
    [ConditionalHide("splitOnDeath")][SerializeField] private Minion splitEnemyPrefab;

    //... prevent the player from farming infinite essence by only dropping essences, if the 
    //... enemy is the smallest size it has been
    [SerializeField] private Enemy[] minionSizeOrder;
    private string[] minionSizeOrderNames;

    public string MinReachedSize { get; private set; }
    public void SetMinReachedSize(string value) {
        string minName = RemoveCloneFromName(value);
        if (!minionSizeOrderNames.Contains(minName)) {
            Debug.LogError("minionSizeOrderNames does not contain name: " + minName);
            return;
        }

        MinReachedSize = minName;

        if (TryGetComponent(out DropEssenceOnDeath dropEssenceOnDeath)) {
            bool isMinSizeReached = MinReachedSize == RemoveCloneFromName(name);
            dropEssenceOnDeath.IsEnabled = !FromSpawnBehavior && isMinSizeReached;
        }
    }

    protected override void Awake() {
        base.Awake();

        moveBehavior = GetComponent<ChasePlayerBehavior>();
        attackBehavior = GetComponent<CircleSlashBehavior>();

        if (isMergable) {
            mergeBehavior = GetComponent<MergeBehavior>();
            mergeBehavior.AllowMerging();
        }

        minionSizeOrderNames = minionSizeOrder.Select(m => RemoveCloneFromName(m.name)).ToArray();
    }

    protected override void OnEnable() {
        base.OnEnable();
        moveBehavior.enabled = true;
        attackBehavior.enabled = false;
        SetMinReachedSize(name); // gets reset but might get set right after by split or merge
    }

    private void Start() {
        health.DeathEventTrigger.AddListener(OnDeath);
    }

    private void OnDestroy() {
        health.DeathEventTrigger.RemoveListener(OnDeath);
    }

    protected override void Update() {
        base.Update();

        // stop chasing the player when merging
        if (isMergable && mergeBehavior.IsMerging()) {
            if (moveBehavior.enabled) {
                moveBehavior.enabled = false;
            }
        }
        else {
            if (!moveBehavior.enabled) {
                moveBehavior.enabled = true;
            }
        }
    }

    // only merges when not close to player
    protected override void OnPlayerEnteredRange(GameObject player) {
        base.OnPlayerEnteredRange(player);

        if (!isMergable || !mergeBehavior.IsMerging()) {
            moveBehavior.enabled = false;
            attackBehavior.enabled = true;

            if (isMergable) {
                mergeBehavior.DontAllowMerging();
            }
        }
    }

    protected override void OnPlayerExitedRange(GameObject player) {
        base.OnPlayerExitedRange(player);

        moveBehavior.enabled = true;
        attackBehavior.enabled = false;

        if (isMergable) {
            mergeBehavior.AllowMerging();
        }
    }

    public override void OnAddEffect(UnitEffect unitEffect) {
        base.OnAddEffect(unitEffect);

        if (unitEffect is StopMovement) {
            moveBehavior.enabled = false;
        }
    }

    private void OnDeath() {

        if (isMergable && mergeBehavior.IsMerging()) {
            mergeBehavior.StopMerging();
        }

        SpawnTwoMinions();
    }

    #region Split On Destroy

    private void SpawnTwoMinions() {

        if (!splitOnDeath) {
            return;
        }

        float offsetValue = 0.5f;

        Vector3 firstOffset = new(-offsetValue, 0);
        Minion firstMinion = splitEnemyPrefab.Spawn(transform.position + firstOffset, Containers.Instance.Enemies);
        firstMinion.SetFromSpawnBehavior(FromSpawnBehavior);

        Vector3 secondOffset = new(offsetValue, 0);
        Minion secondMinion = splitEnemyPrefab.Spawn(transform.position + secondOffset, Containers.Instance.Enemies);
        secondMinion.SetFromSpawnBehavior(FromSpawnBehavior);

        // prevent the player from farming infinite essence by only dropping essences, if the enemy is the smallest
        // size it has been
        string minSizeName = MinSizeName(MinReachedSize, splitEnemyPrefab.name);
        if (!minionSizeOrderNames.Contains(minSizeName)) {
            Debug.LogError("minionSizeOrderNames does not contain minSizeName: " + minSizeName);
            return;
        }

        firstMinion.SetMinReachedSize(minSizeName);
        secondMinion.SetMinReachedSize(minSizeName);
    }

    private string RemoveCloneFromName(string name) {
        if (name.EndsWith("(Clone)")) {
            return name[..^7];
        }
        else {
            return name;
        }
    }

    public string MinSizeName(string name1, string name2) {

        name1 = RemoveCloneFromName(name1);
        name2 = RemoveCloneFromName(name2);

        if (!minionSizeOrderNames.Contains(name1)) {
            Debug.LogError("minionSizeOrderNames does not contain name1: " + name1);
            return "";
        }
        if (!minionSizeOrderNames.Contains(name2)) {
            Debug.LogError("minionSizeOrderNames does not contain name2: " + name2);
            return "";
        }

        int currentSmallestIndex = Array.IndexOf(minionSizeOrderNames, name1);
        int splitIndex = Array.IndexOf(minionSizeOrderNames, name2);

        int smallestIndex = Mathf.Min(currentSmallestIndex, splitIndex);
        return minionSizeOrderNames[smallestIndex];
    }

    #endregion
}
