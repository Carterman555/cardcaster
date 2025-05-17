public class GameStatsTracker : Singleton<GameStatsTracker> {

    public int Kills { get; private set; }

    private void OnEnable() {
        GameSceneManager.OnStartGameLoadingCompleted += OnGameStart;
        EnemyHealth.OnAnyDeath += IncrementKills;
    }

    private void OnDisable() {
        GameSceneManager.OnStartGameLoadingCompleted -= OnGameStart;
        EnemyHealth.OnAnyDeath -= IncrementKills;
    }

    private void OnGameStart() {
        Kills = 0;
    }

    private void IncrementKills(EnemyHealth health) {
        Kills++;
    }
}
