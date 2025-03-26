using DG.Tweening;
using MoreMountains.Feedbacks;
using QFSW.QC.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BobMovement))]
public class EssenceDrop : MonoBehaviour {

    private BobMovement bobMovement;
    private MMF_Player launchPlayer;

    private void Awake() {
        bobMovement = GetComponent<BobMovement>();
        launchPlayer = GetComponent<MMF_Player>();
    }

    private void OnEnable() {
        bobMovement.enabled = false;

        launchPlayer.PlayFeedbacks();
        launchPlayer.Events.OnComplete.AddListener(OnLaunchComplete);
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        bool launching = launchPlayer.IsPlaying;
        bool nearPlayer = GameLayers.PlayerLayerMask.ContainsLayer(collision.gameObject.layer);

        if (nearPlayer && !launching) {
            StartCoroutine(MoveToPlayer(collision.transform));
        }
    }

    private void OnLaunchComplete() {
        launchPlayer.Events.OnComplete.RemoveListener(OnLaunchComplete);

        float radius = GetComponent<CircleCollider2D>().radius;
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius, GameLayers.PlayerLayerMask);
        bool touchingPlayer = cols.Length > 0;
        if (touchingPlayer) {
            StartCoroutine(MoveToPlayer(cols[0].transform));
        }
        else {
            bobMovement.enabled = true;
        }
    }

    private IEnumerator MoveToPlayer(Transform player) {
        bobMovement.enabled = false;

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
