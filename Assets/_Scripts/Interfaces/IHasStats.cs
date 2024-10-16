using UnityEngine;

public interface IHasStats {
    public Stats GetStats();
}

public interface IHasEnemyStats : IHasStats {
    public EnemyStats GetEnemyStats();
}
