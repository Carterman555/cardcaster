using DG.Tweening;
using Mono.CSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// this needs to check if the position the player is trying to teleport to is in the room and not in a collider.
/// if one of those is the case, then it needs to send a raycast from the mouse position to the player position until
/// there is an open spot to teleport. Then teleport there
/// </summary>
[CreateAssetMenu(fileName = "LaunchCard", menuName = "Cards/Launch Card")]
public class ScriptableLaunchCard : ScriptableAbilityCardBase {
    public float raycastStep = 0.1f;
    public LayerMask obstacleLayer;

    private Invincibility playerInvincibility;

    [Header("Path Visual")]
    [SerializeField] private SpriteRenderer pathVisualPrefab;
    private SpriteRenderer pathVisual;

    [Header("Launch")]
    [SerializeField] private PlayerTouchDamage damageDealerPrefab;
    private PlayerTouchDamage damageDealer;

    [SerializeField] private TriggerContactTracker wallTriggerPrefab;
    private TriggerContactTracker wallTrigger;

    [SerializeField] private float launchSpeed;
    private Tween launchTween;

    private List<GameObject> abilityEffects = new();

    [Header("Visuals")]
    [SerializeField] private ParticleSystem launchEffectsPrefab;
    private ParticleSystem launchEffects;

    public override void OnStartDraggingCard(Transform cardTransform) {
        base.OnStartDraggingCard(cardTransform);

        pathVisual = pathVisualPrefab.Spawn(PlayerMovement.Instance.transform.position, PlayerMovement.Instance.transform);
    }

    protected override void DraggingUpdate(Vector2 cardposition) {
        base.DraggingUpdate(cardposition);

        Vector2 toMouseDirection = MouseTracker.Instance.ToMouseDirection(pathVisual.transform.position);

        //... point path towards mouse
        pathVisual.transform.up = toMouseDirection;

        // scale path towards end of room
        float pathWidth = Stats.AreaSize * 2f;

        float checkDistance = 100f;
        RaycastHit2D hit = Physics2D.BoxCast(pathVisual.transform.position, new Vector2(pathWidth, 1f), pathVisual.transform.eulerAngles.z, toMouseDirection, checkDistance, obstacleLayer);

        if (hit.collider == null) {
            Debug.LogError("Could Not Find Wall!");
        }
        else {
            pathVisual.size = new Vector3(pathWidth, hit.distance);
        }
    }

    protected override void Play(Vector2 position) {

        //... disable the path visual before base.play because base.play adds visual effects
        //... that will try to parent to path visual
        pathVisual.gameObject.ReturnToPool();

        base.Play(position);

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
        damageDealer.SetDamageMult(Stats.Damage);
        damageDealer.GetComponent<CircleCollider2D>().radius = Stats.AreaSize;

        //... make player move through objects
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.RoomObjectLayer, true);
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.EnemyLayer, true);

        // make it stop when hit wall
        wallTrigger = wallTriggerPrefab.Spawn(playerTransform.position, playerTransform);
        wallTrigger.GetComponent<CircleCollider2D>().radius = Stats.AreaSize;
        wallTrigger.OnEnterContact += StopLaunch;

        // move sword to point forward
        ReferenceSystem.Instance.PlayerWeaponParent.GetComponent<SlashingWeapon>().enabled = false;

        float rotOffset = -45f;
        Vector3 rot = toMouseDirection.DirectionToRotation().eulerAngles + new Vector3(0, 0, rotOffset);
        ReferenceSystem.Instance.PlayerWeaponParent.transform.DORotate(rot, duration: 0.3f);

        //... make invincible
        playerInvincibility = playerTransform.AddComponent<Invincibility>();

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

        wallTrigger.gameObject.ReturnToPool();

        // move sword back to normal pos
        ReferenceSystem.Instance.PlayerWeaponParent.GetComponent<SlashingWeapon>().enabled = true;

        //... make not invincible
        Destroy(playerInvincibility);

        // take off effects
        foreach (GameObject abilityEffect in abilityEffects) {
            abilityEffect.ReturnToPool();
        }
        abilityEffects.Clear();

        launchEffects.gameObject.ReturnToPool();
    }

    public override void AddEffect(GameObject effectPrefab) {
        base.AddEffect(effectPrefab);
        GameObject effect = effectPrefab.Spawn(PlayerMovement.Instance.transform);
        abilityEffects.Add(effect);
    }
}