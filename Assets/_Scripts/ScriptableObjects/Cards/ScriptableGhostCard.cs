using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GhostCard", menuName = "Cards/Ghost Card")]
public class ScriptableGhostCard : ScriptableStatsModifierCard {

    private FadeEffect ghostFadeEffect;

    private PlayerInvincibility playerInvincibility;

    protected override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.Instance.DisableAttack();

        // ghost visuals
        float fadeAmount = 0.5f;
        float duration = 0.5f;
        ghostFadeEffect = PlayerVisual.Instance.AddFadeEffect(1, fadeAmount, duration);
        ReferenceSystem.Instance.PlayerSwordVisual.enabled = false;

        //... set invincible
        playerInvincibility = PlayerMovement.Instance.AddComponent<PlayerInvincibility>();

        //... make it move through objects
        Physics2D.IgnoreLayerCollision(GameLayers.InvinciblePlayerLayer, GameLayers.RoomObjectLayer, true);
    }

    public override void Stop() {
        base.Stop();

        PlayerMeleeAttack.Instance.AllowAttack();

        // revert ghost visuals
        float duration = 0.5f;
        PlayerVisual.Instance.RemoveFadeEffect(ghostFadeEffect, duration);
        ReferenceSystem.Instance.PlayerSwordVisual.enabled = true;

        //... set not invincible
        Destroy(playerInvincibility);

        //... prevent from moving through objects
        Physics2D.IgnoreLayerCollision(GameLayers.InvinciblePlayerLayer, GameLayers.RoomObjectLayer, false);
    }


    private void FadePlayer(float fadeAmount) {
        SpriteRenderer[] playerSprites = PlayerMovement.Instance.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer playerSprite in playerSprites) {
            playerSprite.Fade(fadeAmount);
        }
    }


}
