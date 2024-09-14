using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : StaticInstance<ItemInfoPanel>, IInitializable {

    public void Initialize() => Instance = this;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image iconImage;

    public void SetItem(ScriptableItemBase item) {
        title.text = item.GetName();
        description.text = item.GetDescription();
        iconImage.sprite = item.GetSprite();
    }
}
