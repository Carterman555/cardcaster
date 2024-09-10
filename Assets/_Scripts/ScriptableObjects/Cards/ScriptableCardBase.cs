using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableCardBase : ScriptableObject, ICollectable {

    [SerializeField] private string cardName;
    public string GetName() => cardName;

    [SerializeField] private string description;
    public string GetDescription() => description;

    [SerializeField] private int cost;
    public int GetCost() => cost;

    [SerializeField] private Rarity rarity;
    public Rarity GetRarity() => rarity;

    [SerializeField] private Sprite sprite;
    public Sprite GetSprite() => sprite;

    [SerializeField] private Sprite outlineSprite;
    public Sprite GetOutlineSprite() => outlineSprite;

    [field: SerializeField] public bool IsPossibleStartingCard { get; private set; }

    [field: SerializeField] public bool IsPositional { get; private set; }

    [SerializeField] protected float effectDuration;

    public virtual void Play(Vector2 position) {
        StatsManager.Instance.StartCoroutine(StopAfterDuration());
    }

    private IEnumerator StopAfterDuration() {
        yield return new WaitForSeconds(effectDuration);
        Stop();
    }

    public virtual void Stop() { }
}