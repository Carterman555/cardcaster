using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemContainerUI : MonoBehaviour {

    [SerializeField] private Image itemIconPrefab;

    private Dictionary<ScriptableItem, Image> iconDict = new();

    private void OnEnable() {
        ItemManager.OnItemGained += AddItemToUI;
        ItemManager.OnItemRemoved += RemoveItemFromUI;
    }
    private void OnDisable() {
        ItemManager.OnItemGained -= AddItemToUI;
        ItemManager.OnItemRemoved -= RemoveItemFromUI;
    }

    private void AddItemToUI(ScriptableItem item) {
        Image newItemIcon = itemIconPrefab.Spawn(transform);
        newItemIcon.sprite = item.GetSprite();

        iconDict.Add(item, newItemIcon);
    }

    private void RemoveItemFromUI(ScriptableItem item) {
        iconDict[item].gameObject.ReturnToPool();

        iconDict.Remove(item);
    }
}
