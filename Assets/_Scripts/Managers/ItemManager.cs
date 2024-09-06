using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemManager : StaticInstance<ItemManager> {

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
    }

    public void RemoveItem(ScriptableItem item) {
        ScriptableItem itemToRemove = items.FirstOrDefault(i => i.GetType().Equals(item.GetType()));

        if (itemToRemove == null) {
            Debug.LogWarning("Tried To Remove Item The Player Doesn't Have!");
            return;
        }

        itemToRemove.Deactivate();
        items.Remove(itemToRemove);
    }
}
