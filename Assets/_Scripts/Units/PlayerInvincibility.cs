public class PlayerInvincibility : Invincibility {

    private void Start() {
        gameObject.layer = GameLayers.InvinciblePlayerLayer; // set player to different layer, so doesn't trigger projectiles
    }

    private void OnDestroy() {
        PlayerInvincibility[] playerInvincibilities = GetComponents<PlayerInvincibility>();
        if (playerInvincibilities.Length == 1) {
            gameObject.layer = GameLayers.PlayerLayer;
        }
    }
}
