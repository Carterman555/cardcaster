using UnityEngine;

public interface IChestItem {

    public void Setup(Chest chest, int collectableIndex, Vector2 position);
    public void ReturnToChest(float duration);
}
