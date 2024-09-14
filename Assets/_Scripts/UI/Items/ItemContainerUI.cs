using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemContainerUI : MonoBehaviour {

    [SerializeField] private ItemIcon itemIconPrefab;

    private Dictionary<ScriptableItemBase, ItemIcon> iconDict = new();

    private void OnEnable() {
        ItemManager.OnItemGained += AddItemToUI;
        ItemManager.OnItemRemoved += RemoveItemFromUI;
    }
    private void OnDisable() {
        ItemManager.OnItemGained -= AddItemToUI;
        ItemManager.OnItemRemoved -= RemoveItemFromUI;
    }

    private void AddItemToUI(ScriptableItemBase item) {
        ItemIcon newItemIcon = itemIconPrefab.Spawn(transform);
        newItemIcon.Setup(item);

        iconDict.Add(item, newItemIcon);
    }

    private void RemoveItemFromUI(ScriptableItemBase item) {
        iconDict[item].gameObject.ReturnToPool();

        iconDict.Remove(item);
    }
}
