using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public interface ICollectable {

    public LocalizedString Name { get; }
    public LocalizedString Description { get; }
    public Rarity Rarity { get; }

    public int Cost { get; }

    public Sprite Sprite { get; }
}

public enum Rarity {
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Epic = 3,
    Mythic = 4
}
