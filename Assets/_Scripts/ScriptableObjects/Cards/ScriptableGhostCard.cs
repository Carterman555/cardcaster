using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GhostCard", menuName = "Cards/Ghost Card")]
public class ScriptableGhostCard : ScriptableStatsModifierCard {

    private PlayerFade ghostFadeEffect;
    private PlayerInvincibility playerInvincibility;

    protected override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.Instance.DisableAttack();

        float fadeAmount = 0.5f;
        ghostFadeEffect = PlayerFadeManager.Instance.AddFadeEffect(1, fadeAmount);
        ReferenceSystem.Instance.PlayerSwordVisual.enabled = false;

        playerInvincibility = PlayerMovement.Instance.AddComponent<PlayerInvincibility>();
    }

    public override void Stop() {
        base.Stop();

        PlayerMeleeAttack.Instance.AllowAttack();

        PlayerFadeManager.Instance.RemoveFadeEffect(ghostFadeEffect);
        ReferenceSystem.Instance.PlayerSwordVisual.enabled = true;

        Destroy(playerInvincibility);
    }
}
