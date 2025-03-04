using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerVisual))]
public class IFrames : MonoBehaviour {

    [SerializeField] private float flashDelay;
    [SerializeField] private int flashAmount;

    private PlayerVisual playerVisual;

    private Invincibility playerInvincibility;

    private void Awake() {
        playerVisual = GetComponent<PlayerVisual>();
    }

    [Command]
    public void OnDamaged() {
        StartCoroutine(Flash());
    }

    private IEnumerator Flash() {

        //... set player invincible
        playerInvincibility = PlayerMeleeAttack.Instance.AddComponent<Invincibility>();

        for (int i = 0; i < flashAmount; i++) {
            FadeEffect fadeEffect = playerVisual.AddFadeEffect(10, 0);

            yield return new WaitForSeconds(flashDelay);

            playerVisual.RemoveFadeEffect(fadeEffect);

            yield return new WaitForSeconds(flashDelay);
        }

        //... remove player invincibility
        Destroy(playerInvincibility);
    }

    private void OnDisable() {
        if (playerInvincibility != null) {
            Destroy(playerInvincibility);
        }
    }
}
