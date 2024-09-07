using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : StaticInstance<ItemManager> {

    public static event Action<ScriptableItem> OnItemGained;
    public static event Action<ScriptableItem> OnItemRemoved;

    private List<ScriptableItem> items = new();

    [SerializeField] private ScriptableItem testItemToAdd;

    [ContextMenu("Add test")]
    private void AddTestItem() {
        GainItem(testItemToAdd);
    }

    [ContextMenu("Remove test")]
    private void RemoveTestItem() {
        RemoveItem(testItemToAdd);
    }

    public void GainItem(ScriptableItem item) {
        ScriptableItem itemInstance = Instantiate(item);
        itemInstance.Activate();
        items.Add(itemInstance);

        OnItemGained?.Invoke(itemInstance);
    }

    public void RemoveItem(ScriptableItem item) {
        ScriptableItem itemToRemove = items.FirstOrDefault(i => i.GetType().Equals(item.GetType()));

        if (itemToRemove == null) {
            Debug.LogWarning("Tried To Remove Item The Player Doesn't Have!");
            return;
        }

        itemToRemove.Deactivate();
        items.Remove(itemToRemove);

        OnItemRemoved?.Invoke(itemToRemove);
    }
}
