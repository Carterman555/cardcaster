using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelHole : MonoBehaviour {

    public static event Action OnFallInHole;

    [SerializeField] private SpriteMask spriteMask;

    [SerializeField] private bool tutorialHole;

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
        PlayerMovement.Instance.StopMovement();

        // fall movement
        float fallDistance = 3.5f;
        float fallYPos = PlayerMovement.Instance.CenterPos.y - fallDistance;
        PlayerMovement.Instance.transform.DOMoveY(fallYPos, duration: 0.5f).SetEase(Ease.InSine);

        OnFallInHole?.Invoke();

        if (!tutorialHole) {
            GameSceneManager.Instance.NextLevel();
        }
        else {
            //... go to first level if just finished tutorial
            GameSceneManager.Instance.StartGame();
        }
    }
}
