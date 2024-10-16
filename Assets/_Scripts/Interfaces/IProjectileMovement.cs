using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetProjectileMovement {
    public GameObject GetObject();
    public void Setup(Transform target);
}
