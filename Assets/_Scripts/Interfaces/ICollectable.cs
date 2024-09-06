using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectable {

    public string GetName();
    public string GetDescription();

    public Rarity GetRarity();

    public int GetCost();

    public Sprite GetSprite();
    public Sprite GetOutlineSprite();
}

public enum Rarity {
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Epic = 3,
    Mythic = 4
}
