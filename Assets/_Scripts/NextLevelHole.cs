using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelHole : MonoBehaviour {
    [SerializeField] private SpriteMask spriteMask;

    private bool fallTriggered;

    private void OnEnable() {
        spriteMask.enabled = false;
        fallTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent(out HoleTrigger holeTrigger) && !fallTriggered) {
            MakePlayerFall();
        }
    }

    private void MakePlayerFall() {

        fallTriggered = true;

        //... make player masked behind sprite so it looks like the player is falling into the hole.
        spriteMask.enabled = true;

        //... disable movement
        PlayerMovement.Instance.enabled = false;

        // fall movement
        float fallDistance = 3.5f;
        float fallYPos = PlayerMovement.Instance.transform.position.y - fallDistance;
        PlayerMovement.Instance.transform.DOMoveY(fallYPos, duration: 0.5f).SetEase(Ease.InSine);

        // load next level
        LevelManager.Instance.NextLevel();
    }
}
