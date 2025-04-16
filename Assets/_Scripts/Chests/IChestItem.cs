using UnityEngine;

public interface IChestItem {

    public void Setup(Chest chest, Vector2 position);
    public void ReturnToChest(float duration);
}
