public interface IHasPlayerStats {
    public PlayerStats PlayerStats { get; }
}

public interface IHasEnemyStats {
    EnemyStats GetEnemyStats();
}
