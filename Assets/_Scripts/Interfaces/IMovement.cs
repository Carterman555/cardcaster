using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetMovement {
    public GameObject GetObject();
    public void Setup(Transform target);
}
