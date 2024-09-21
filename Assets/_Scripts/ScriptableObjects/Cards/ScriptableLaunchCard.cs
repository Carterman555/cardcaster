using DG.Tweening;
using Mono.CSharp;
using System;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// this needs to check if the position the player is trying to teleport to is in the room and not in a collider.
/// if one of those is the case, then it needs to send a raycast from the mouse position to the player position until
/// there is an open spot to teleport. Then teleport there
/// </summary>
[CreateAssetMenu(fileName = "LaunchCard", menuName = "Cards/Launch Card")]
public class ScriptableLaunchCard : ScriptableCardBase {
    public float raycastStep = 0.1f;
    public LayerMask obstacleLayer;

    [Header("Path Visual")]
    [SerializeField] private float pathWidth = 3;
    [SerializeField] private Transform pathVisualPrefab;
    private Transform pathVisual;

    [Header("Launch")]
    [SerializeField] private PlayerTouchDamage damageDealerPrefab;
    private PlayerTouchDamage damageDealer;

    [SerializeField] private TriggerContactTracker wallTriggerPrefab;
    private TriggerContactTracker wallTrigger;

    [SerializeField] private float damageMult;
    [SerializeField] private float launchSpeed;
    private Tween launchTween;

    [Header("Effects")]
    [SerializeField] private ParticleSystem launchEffectsPrefab;
    private ParticleSystem launchEffects;

    public override void OnStartDraggingCard() {
        base.OnStartDraggingCard();

        pathVisual = pathVisualPrefab.Spawn(PlayerMovement.Instance.transform.position, PlayerVisual.Instance.transform);
    }

    protected override void DraggingUpdate() {
        base.DraggingUpdate();

        Vector2 toMouseDirection = MouseTracker.Instance.ToMouseDirection(pathVisual.transform.position);

        //... point path towards mouse
        pathVisual.up = toMouseDirection;

        // scale path towards end of room
        float checkDistance = 100f;
        RaycastHit2D hit = Physics2D.BoxCast(pathVisual.position, new Vector2(pathWidth, 1f), pathVisual.eulerAngles.z, toMouseDirection, checkDistance, obstacleLayer);

        if (hit.collider == null) {
            Debug.LogError("Could Not Find Wall!");
        }
        else {
            pathVisual.localScale = new Vector3(pathWidth, hit.distance);
        }
    }

    public override void Play(Vector2 position) {
        base.Play(position);

        pathVisual.gameObject.ReturnToPool();

        Transform playerTransform = PlayerMovement.Instance.transform;

        PlayerMovement.Instance.enabled = false;
        PlayerMeleeAttack.Instance.enabled = false;

        // launch player
        Rigidbody2D playerRb = playerTransform.GetComponent<Rigidbody2D>();
        float launchFullSpeedTime = 0.5f;
        Vector2 toMouseDirection = MouseTracker.Instance.ToMouseDirection(pathVisual.transform.position);
        launchTween = DOTween.To(() => playerRb.velocity, x => playerRb.velocity = x, toMouseDirection * launchSpeed, launchFullSpeedTime);

        // make deal damage
        damageDealer = damageDealerPrefab.Spawn(playerTransform.position, playerTransform);
        damageDealer.SetDamageMult(damageMult);

        //... make player move through objects
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.RoomObjectLayer, true);
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.EnemyLayer, true);

        // make it stop when hit wall
        wallTrigger = wallTriggerPrefab.Spawn(playerTransform.position, playerTransform);
        wallTrigger.OnEnterContact += StopLaunch;

        // move sword to point forward
        ReferenceSystem.Instance.PlayerWeaponParent.GetComponent<SlashingWeapon>().enabled = false;

        float rotOffset = -45f;
        Vector3 rot = toMouseDirection.DirectionToRotation().eulerAngles + new Vector3(0, 0, rotOffset);
        ReferenceSystem.Instance.PlayerWeaponParent.transform.DORotate(rot, duration: 0.3f);

        //... make invincible
        playerTransform.GetComponent<Health>().SetInvincible(true);

        launchEffects = launchEffectsPrefab.Spawn(playerTransform.position, playerTransform);
        launchEffects.transform.up = toMouseDirection;
    }

    private void StopLaunch(GameObject wall) {
        wallTrigger.OnEnterContact -= StopLaunch;

        PlayerMovement.Instance.enabled = true;
        PlayerMeleeAttack.Instance.enabled = true;

        //... make player not move through objects and enemies
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.RoomObjectLayer, false);
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.EnemyLayer, false);

        //... stop launch
        launchTween.Kill();

        //... stop dealing damage
        damageDealer.gameObject.ReturnToPool();

        // move sword back to normal pos
        ReferenceSystem.Instance.PlayerWeaponParent.GetComponent<SlashingWeapon>().enabled = true;

        //... make not invincible
        PlayerMeleeAttack.Instance.GetComponent<Health>().SetInvincible(false);

        launchEffects.gameObject.ReturnToPool();
    }
}