using QFSW.QC;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerFadeManager))]
public class IFrames : MonoBehaviour {

    [SerializeField] private float flashDelay;
    [SerializeField] private int flashAmount;

    [SerializeField] private PlayerHealth playerHealth;
    private PlayerFadeManager playerVisual;

    private Invincibility playerInvincibility;

    private void Awake() {
        playerVisual = GetComponent<PlayerFadeManager>();
    }

    [Command]
    public void OnDamaged() {
        StartCoroutine(Flash());
    }

    private IEnumerator Flash() {

        yield return null; // wait a frame to see if dead

        if (playerHealth.Dead) {
            yield break;
        }

        //... set player invincible
        playerInvincibility = PlayerMeleeAttack.Instance.AddComponent<Invincibility>();

        for (int i = 0; i < flashAmount; i++) {
            PlayerFade playerFade = playerVisual.AddFadeEffect(10, 0);

            yield return new WaitForSeconds(flashDelay);

            playerVisual.RemoveFadeEffect(playerFade);

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
