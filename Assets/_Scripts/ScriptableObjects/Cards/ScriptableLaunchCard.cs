using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "LaunchCard", menuName = "Cards/Launch Card")]
public class ScriptableLaunchCard : ScriptableAbilityCardBase {
    public float raycastStep = 0.1f;

    private PlayerInvincibility playerInvincibility;

    [Header("Path Visual")]
    [SerializeField] private SpriteRenderer pathVisualPrefab;
    private SpriteRenderer pathVisual;

    [Header("Launch")]
    [SerializeField] private PlayerTouchDamage damageDealerPrefab;
    private PlayerTouchDamage damageDealer;

    [SerializeField] private TriggerContactTracker wallTriggerPrefab;
    private TriggerContactTracker wallTrigger;
    private float checkFactor = 0.75f;

    [SerializeField] private float launchSpeed;
    private Tween launchTween;

    private List<GameObject> effectModifierObjects = new();

    private Vector2 launchDirection;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem launchEffectsPrefab;
    private ParticleSystem launchEffects;

    private float positioningAreaSize;

    public override void OnStartPositioningCard(Transform cardTransform) {
        base.OnStartPositioningCard(cardTransform);

        pathVisual = pathVisualPrefab.Spawn(PlayerMovement.Instance.CenterPos, PlayerMovement.Instance.transform);

        float fade = pathVisual.color.a;
        pathVisual.Fade(0f);
        pathVisual.DOKill();
        pathVisual.DOFade(fade, duration: 0.2f).SetDelay(0.1f); // delay to make sure path rotation is set towards the card position

        // the modifier stats apply when card is played, but I still want the modifiers to effect the path visual size
        positioningAreaSize = Stats.AreaSize;
        foreach (ScriptableModifierCardBase modifier in AbilityManager.Instance.ActiveModifiers) {
            positioningAreaSize *= modifier.StatsModifier.AreaSize.PercentToMult();
        }
    }

    protected override void PositioningUpdate(Vector2 cardPosition) {
        base.PositioningUpdate(cardPosition);

        launchDirection = (cardPosition - (Vector2)pathVisual.transform.position).normalized;
        pathVisual.transform.up = launchDirection;

        // scale path towards end of room
        float pathWidth = positioningAreaSize * 2f;

        float checkDistance = 100f;
        float distanceFromPlayer = 1.5f;
        Vector2 origin = (Vector2)pathVisual.transform.position + (launchDirection * distanceFromPlayer);
        RaycastHit2D hit = Physics2D.BoxCast(origin, new Vector2(pathWidth * checkFactor, 1f), pathVisual.transform.eulerAngles.z, launchDirection, checkDistance, GameLayers.WallLayerMask);

        if (hit.collider == null) {
            Debug.LogError("Could Not Find Wall!");
        }
        else if (hit.distance > 1f) {
            pathVisual.size = new Vector3(pathWidth, hit.distance + distanceFromPlayer);
        }
    }

    public override void OnStopPositioningCard() {
        base.OnStopPositioningCard();

        pathVisual.gameObject.ReturnToPool();
    }

    protected override void Play(Vector2 position) {
        Transform playerTransform = PlayerMovement.Instance.transform;
        Vector2 playerCenterPos = PlayerMovement.Instance.CenterPos;

        //... need to spawn damageDealer before base.Play(position); because it invokes ApplyModifier which uses damageDealer
        damageDealer = damageDealerPrefab.Spawn(playerCenterPos, playerTransform);

        base.Play(position);

        PlayerMovement.Instance.DisableMoveInput();
        PlayerMeleeAttack.Instance.DisableAttack();

        // launch player
        Rigidbody2D playerRb = playerTransform.GetComponent<Rigidbody2D>();
        float launchFullSpeedTime = 0.5f;
        launchTween = DOTween.To(() => playerRb.velocity, x => playerRb.velocity = x, launchDirection * launchSpeed, launchFullSpeedTime);

        // make deal damage
        damageDealer.SetDamageMult(Stats.Damage);
        damageDealer.GetComponent<CircleCollider2D>().radius = Stats.AreaSize;
        

        //... make player move through objects
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.RoomObjectLayer, true);
        Physics2D.IgnoreLayerCollision(GameLayers.InvinciblePlayerLayer, GameLayers.RoomObjectLayer, true);

        // make it stop when hit wall
        wallTrigger = wallTriggerPrefab.Spawn(playerCenterPos, playerTransform);
        wallTrigger.GetComponent<CircleCollider2D>().radius = Stats.AreaSize * checkFactor;
        wallTrigger.OnEnterContact_GO += TryStopLaunch;

        // move sword to point forward
        ReferenceSystem.Instance.PlayerWeaponParent.GetComponent<SlashingWeapon>().enabled = false;

        float rotOffset = -45f;
        Vector3 rot = launchDirection.DirectionToRotation().eulerAngles + new Vector3(0, 0, rotOffset);
        ReferenceSystem.Instance.PlayerWeaponParent.transform.DORotate(rot, duration: 0.3f);

        //... make invincible
        playerInvincibility = playerTransform.AddComponent<PlayerInvincibility>();

        launchEffects = launchEffectsPrefab.Spawn(playerCenterPos, playerTransform);
        launchEffects.transform.up = launchDirection;
    }

    // only stop the launch if the wall is in front of the player. This is to prevent the player from stopping
    // immediately after launching if backed up into wall
    private void TryStopLaunch(GameObject wall) {
        Vector2 playerPosition = PlayerMovement.Instance.CenterPos;
        Vector2 contactPoint = wall.GetComponent<Collider2D>().ClosestPoint(playerPosition);

        Vector2 toContactPoint = contactPoint - playerPosition;
        float dotProduct = Vector2.Dot(toContactPoint, launchDirection);

        bool contactPointInFrontOfPlayer = dotProduct > 0f;
        if (contactPointInFrontOfPlayer) {
            StopLaunch(wall);
        }
    }

    private void StopLaunch(GameObject wall) {
        base.Stop();

        wallTrigger.OnEnterContact_GO -= TryStopLaunch;

        PlayerMovement.Instance.AllowMoveInput();
        PlayerMeleeAttack.Instance.AllowAttack();

        //... make player not move through objects and enemies
        Physics2D.IgnoreLayerCollision(GameLayers.PlayerLayer, GameLayers.RoomObjectLayer, false);
        Physics2D.IgnoreLayerCollision(GameLayers.InvinciblePlayerLayer, GameLayers.RoomObjectLayer, false);

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
        foreach (GameObject effectModifier in effectModifierObjects) {
            effectModifier.ReturnToPool();
        }
        effectModifierObjects.Clear();

        launchEffects.gameObject.ReturnToPool();

        CameraShaker.Instance.ShakeCamera(0.4f);
    }

    public override void ApplyModifier(ScriptableModifierCardBase modifierCard) {
        base.ApplyModifier(modifierCard);
        if (modifierCard.AppliesEffect) {
            GameObject effectLogicObject = modifierCard.EffectModifier.EffectLogicPrefab.Spawn(damageDealer.transform);
            effectModifierObjects.Add(effectLogicObject);

            if (modifierCard.EffectModifier.HasVisual) {
                GameObject effectVisualObject = modifierCard.EffectModifier.EffectVisualPrefab.Spawn(damageDealer.transform);
                effectModifierObjects.Add(effectVisualObject);
            }
        }
    }
}