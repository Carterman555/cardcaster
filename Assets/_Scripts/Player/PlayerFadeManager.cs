using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class PlayerFadeManager : StaticInstance<PlayerFadeManager> {

    private SpriteRenderer[] playerSprites;

    private List<PlayerFade> playerFades = new();

    private float currentFade;

    protected override void Awake() {
        base.Awake();
        playerSprites = PlayerMovement.Instance.GetComponentsInChildren<SpriteRenderer>();
    }

    private void Update() {

        float desiredFade = 1f;
        if (playerFades.Count > 0) {
            desiredFade = playerFades.OrderBy(f => f.Strength).Last().DesiredFade;
        }

        if (desiredFade != currentFade) {
            FadePlayer(desiredFade);
            currentFade = desiredFade;
        }
    }

    public PlayerFade AddFadeEffect(int overrideStrength, float fadeAmount) {
        PlayerFade playerFade = new PlayerFade();
        playerFade.Setup(overrideStrength, fadeAmount);
        playerFades.Add(playerFade);
        return playerFade;
    }

    public void RemoveFadeEffect(PlayerFade playerFade) {
        if (!playerFades.Contains(playerFade)) {
            Debug.LogError("Trying to remove playerfade that is not is list!");
            return;
        }

        playerFades.Remove(playerFade);
    }

    public void RemoveAllFadeEffects() {
        playerFades.Clear();
        FadePlayer(1f);
    }

    private void FadePlayer(float fadeAmount) {
        foreach (SpriteRenderer playerSprite in playerSprites) {
            playerSprite.Fade(fadeAmount);
        }
    }
}