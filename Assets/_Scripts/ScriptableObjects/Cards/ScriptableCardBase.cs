using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableCardBase : ScriptableObject, ICollectable {

    [SerializeField] private string cardName;
    public string GetName() => cardName;

    [SerializeField] private string description;
    public string GetDescription() => cardName;

    [SerializeField] private int cost;
    public int GetCost() => cost;

    [SerializeField] private Rarity rarity;
    public Rarity GetRarity() => rarity;

    [SerializeField] private Sprite sprite;
    public Sprite GetSprite() => sprite;

    [field: SerializeField] public bool IsPossibleStartingCard { get; private set; }

    [SerializeField] protected float effectDuration;

    public abstract void Play();
}