using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : StaticInstance<ItemManager> {

    public static event Action<ScriptableItemBase> OnItemGained;
    public static event Action<ScriptableItemBase> OnItemRemoved;

    private List<ScriptableItemBase> items = new();

    [SerializeField] private ScriptableItemBase testItemToAdd;

    [ContextMenu("Add test")]
    private void AddTestItem() {
        GainItem(testItemToAdd);
    }

    [ContextMenu("Remove test")]
    private void RemoveTestItem() {
        RemoveItem(testItemToAdd);
    }

    public void GainItem(ScriptableItemBase item) {
        ScriptableItemBase itemInstance = Instantiate(item);
        itemInstance.Activate();
        items.Add(itemInstance);

        OnItemGained?.Invoke(itemInstance);
    }

    public void RemoveItem(ScriptableItemBase item) {
        ScriptableItemBase itemToRemove = items.FirstOrDefault(i => i.GetType().Equals(item.GetType()));

        if (itemToRemove == null) {
            Debug.LogWarning("Tried To Remove Item The Player Doesn't Have!");
            return;
        }

        itemToRemove.Deactivate();
        items.Remove(itemToRemove);

        OnItemRemoved?.Invoke(itemToRemove);
    }
}
