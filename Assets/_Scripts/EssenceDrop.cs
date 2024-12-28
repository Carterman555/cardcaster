using DG.Tweening;
using QFSW.QC.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BobMovement))]
public class EssenceDrop : MonoBehaviour {

    private BobMovement bobMovement;

    private void Awake() {
        bobMovement = GetComponent<BobMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == GameLayers.PlayerLayer) {
            StartCoroutine(MoveToPlayer(collision.transform));
        }
    }

    private IEnumerator MoveToPlayer(Transform player) {
        bobMovement.StopBobbing();

        float velocity = 0;
        float acceleration = 0.1f;

        while (Vector2.Distance(player.position, transform.position) > 0.5f) {
            velocity += acceleration * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, player.position, velocity);
            yield return null;
        }

        DeckManager.Instance.ChangeEssenceAmount(1);
        transform.gameObject.ReturnToPool();

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.CollectEssence);
    }
}
