using UnityEngine;

public interface ITargetProjectileMovement {
    public GameObject GetObject();
    public void Setup(Transform target);
}
