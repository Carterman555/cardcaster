using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerFadeManager))]
public class IFrames : MonoBehaviour {

    [SerializeField] private float flashDelay;
    [SerializeField] private int flashAmount;

    [SerializeField] private PlayerHealth playerHealth;
    private PlayerFadeManager playerVisual;

    private PlayerInvincibility playerInvincibility;

    private void Awake() {
        playerVisual = GetComponent<PlayerFadeManager>();
    }

    private void OnEnable() {
        playerHealth.OnDamaged += OnDamaged;
    }

    private void OnDisable() {
        playerHealth.OnDamaged -= OnDamaged;

        if (playerInvincibility != null) {
            Destroy(playerInvincibility);
        }
    }

    private void OnDamaged() {
        StartCoroutine(Flash());
    }

    private IEnumerator Flash() {

        if (playerHealth.Dead) {
            yield break;
        }

        //... set player invincible
        playerInvincibility = PlayerMeleeAttack.Instance.AddComponent<PlayerInvincibility>();

        for (int i = 0; i < flashAmount; i++) {
            PlayerFade playerFade = playerVisual.AddFadeEffect(10, 0);

            yield return new WaitForSeconds(flashDelay);

            playerVisual.RemoveFadeEffect(playerFade);

            yield return new WaitForSeconds(flashDelay);
        }

        //... remove player invincibility
        Destroy(playerInvincibility);
    }
}
