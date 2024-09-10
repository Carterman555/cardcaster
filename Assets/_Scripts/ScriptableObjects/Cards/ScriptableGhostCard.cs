using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Ghost Card")]
public class ScriptableGhostCard : ScriptableStatsModifierCard {


    public override void Play(Vector2 position) {
        base.Play(position);
        
        // disable attack
        PlayerMovement.Instance.GetComponent<PlayerMeleeAttack>().enabled = false;

        // make transparent - TODO

        // set invincible
        PlayerMovement.Instance.GetComponent<Health>().SetInvincible(true);

        // make it move through objects
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.RoomObjectLayer, true);
    }

    public override void Stop() {
        base.Stop();

        // enable attack
        PlayerMovement.Instance.GetComponent<PlayerMeleeAttack>().enabled = true;

        // set not invincible
        PlayerMovement.Instance.GetComponent<Health>().SetInvincible(false);

        // prevent from moving through objects
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.RoomObjectLayer, false);
    }


}
