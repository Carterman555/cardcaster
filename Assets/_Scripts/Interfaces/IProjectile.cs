using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStraightProjectile {
    public GameObject GetObject();
    public void Shoot(Vector2 direction, float damage, float knockBackStrength);
}

public interface ITargetProjectile {
    public GameObject GetObject();
    public void Shoot(Transform target, float damage, float knockBackStrength);
}
