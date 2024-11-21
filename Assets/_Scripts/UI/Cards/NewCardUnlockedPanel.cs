using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCardUnlockedPanel : StaticInstance<NewCardUnlockedPanel> {

    [SerializeField] private CardImage cardImage;

    public void Setup(ScriptableCardBase card) {
        cardImage.Setup(card);
    }
}
