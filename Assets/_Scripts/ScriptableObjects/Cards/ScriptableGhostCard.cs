using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GhostCard", menuName = "Cards/Ghost Card")]
public class ScriptableGhostCard : ScriptableStatsModifierCard {

    public override void Play(Vector2 position) {
        base.Play(position);
        
        //... disable attack
        PlayerMovement.Instance.GetComponent<PlayerMeleeAttack>().enabled = false;

        // ghost visuals
        float fadeAmount = 0.5f;
        float duration = 0.5f;
        PlayerVisual.Instance.SetFadeEffect(1, fadeAmount, duration);
        ReferenceSystem.Instance.PlayerSwordVisual.enabled = false;

        //... set invincible
        PlayerMovement.Instance.GetComponent<Health>().SetInvincible(true);

        //... make it move through objects and enemies
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.RoomObjectLayer, true);
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.EnemyLayer, true);
    }

    public override void Stop() {
        base.Stop();

        //... enable attack
        PlayerMovement.Instance.GetComponent<PlayerMeleeAttack>().enabled = true;

        // revert ghost visuals
        float fadeAmount = 1f;
        float duration = 0.5f;
        PlayerVisual.Instance.SetFadeEffect(1, fadeAmount, duration);
        ReferenceSystem.Instance.PlayerSwordVisual.enabled = true;

        //... set not invincible
        PlayerMovement.Instance.GetComponent<Health>().SetInvincible(false);

        //... prevent from moving through objects and enemies
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.RoomObjectLayer, false);
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.EnemyLayer, false);
    }


    private void FadePlayer(float fadeAmount) {
        SpriteRenderer[] playerSprites = PlayerMovement.Instance.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer playerSprite in playerSprites) {
            playerSprite.Fade(fadeAmount);
        }
    }


}
