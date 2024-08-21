using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStraightProjectile {
    public void Shoot(Vector2 direction, float damage);
}

public interface ITargetProjectile {
    public void Shoot(Transform target, float damage);
}
