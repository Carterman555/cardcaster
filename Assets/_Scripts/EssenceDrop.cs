using DG.Tweening;
using MoreMountains.Feedbacks;
using QFSW.QC.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BobMovement))]
public class EssenceDrop : MonoBehaviour {

    private BobMovement bobMovement;

    private void Awake() {
        bobMovement = GetComponent<BobMovement>();
        launchPlayer = GetComponent<MMF_Player>();
    }

    private void Start() {
        float radius = GetComponent<CircleCollider2D>().radius;
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius, GameLayers.PlayerLayerMask);
        bool touchingPlayer = cols.Length > 0;
        if (touchingPlayer) {
            StartCoroutine(MoveToPlayer(cols[0].transform));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (GameLayers.PlayerLayerMask.ContainsLayer(collision.gameObject.layer)) {
            StartCoroutine(MoveToPlayer(collision.transform));
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

    #region Launch

    private MMF_Player launchPlayer;

    public void Launch() {

        bobMovement.enabled = false;

        launchPlayer.PlayFeedbacks();

        launchPlayer.Events.OnComplete.AddListener(StartBobbing);

    }

    private void StartBobbing() {
        bobMovement.enabled = true;

        launchPlayer.Events.OnComplete.RemoveListener(StartBobbing);
    }

    #endregion
}
