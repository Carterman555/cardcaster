using MoreMountains.Feedbacks;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BobMovement))]
public class EssenceDrop : MonoBehaviour {

    private BobMovement bobMovement;
    private MMF_Player launchPlayer;

    [SerializeField] private TriggerEventInvoker wallTrigger;

    private void Awake() {
        bobMovement = GetComponent<BobMovement>();
        launchPlayer = GetComponent<MMF_Player>();
    }

    private void OnEnable() {
        launchPlayer.Events.OnComplete.AddListener(OnLaunchComplete);
        wallTrigger.OnTriggerEnter += OnHitWall;

        bobMovement.enabled = false;

        launchPlayer.PlayFeedbacks();
    }

    private void OnDisable() {
        wallTrigger.OnTriggerEnter -= OnHitWall;
    }

    private void OnHitWall() {
        launchPlayer.StopFeedbacks();
        OnLaunchComplete();
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        bool launching = launchPlayer.IsPlaying;
        bool nearPlayer = GameLayers.AllPlayerLayerMask.ContainsLayer(collision.gameObject.layer);

        if (nearPlayer && !launching) {
            StartCoroutine(MoveToPlayer(collision.transform));
        }
    }

    private void OnLaunchComplete() {
        launchPlayer.Events.OnComplete.RemoveListener(OnLaunchComplete);

        float radius = GetComponent<CircleCollider2D>().radius;
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius, GameLayers.AllPlayerLayerMask);
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
