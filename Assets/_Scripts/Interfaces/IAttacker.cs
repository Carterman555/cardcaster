using System;
using UnityEngine;

public interface IAttacker {
    public event Action OnAttack;
}

public interface ISpecialAttacker {
    public event Action OnSpecialAttack;
}

public interface ITargetAttacker : IAttacker {
    public event Action<GameObject> OnDamage_Target;
}
