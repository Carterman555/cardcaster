using UnityEngine;

public interface IHasStats {
    public Stats Stats { get; }
}

public interface IHasEnemyStats : IHasStats {
    public EnemyStats GetEnemyStats();
}
