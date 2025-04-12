using MoreMountains.Tools;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BoomerangSwordCard", menuName = "Cards/Boomerang Sword Card")]
public class ScriptableBoomerangSwordCard : ScriptableAbilityCardBase {

    [SerializeField] private PlayerTouchDamage playerTouchDamagePrefab;
    private PlayerTouchDamage playerTouchDamage;

    [Header("Movement")]
    private BoomerangMovement boomerangMovement;
    [SerializeField] private float acceleration;
    [SerializeField] private float rotateSpeed;
    private MMAutoRotate autoRotate;

    private Vector3 attackPos;

    protected override void Play(Vector2 position) {

        base.Play(position);

        attackPos = position;

        PlayerMeleeAttack.Instance.DisableAttack();

        // need to setup boomerang movement before setting up sword touch damage. I think because when setting up boomerang
        // movement, it changes the parent and that effects the rotation of the sword touch damage. I don't know why
        SetupBoomerangMovement();

        //... make sword deal damage through touch
        SetupSwordTouchDamage();

        // make sword autorotate
        autoRotate = ReferenceSystem.Instance.PlayerSword.AddComponent<MMAutoRotate>();
        autoRotate.RotationSpeed = new Vector3(0f, 0f, -rotateSpeed);
    }

    private void SetupSwordTouchDamage() {
        Transform sword = ReferenceSystem.Instance.PlayerSword;
        Quaternion damageRotation = Quaternion.Euler(0, 0, sword.eulerAngles.z + 65f);
        playerTouchDamage = playerTouchDamagePrefab.Spawn(sword.position, damageRotation, sword);
        playerTouchDamage.SetDamageMult(StatsManager.PlayerStats.BaseProjectileDamageMult);

        BoxCollider2D prefabCol = playerTouchDamagePrefab.GetComponent<BoxCollider2D>();
        BoxCollider2D instanceCol = playerTouchDamage.GetComponent<BoxCollider2D>();

        //. update collider size to match sword size
        float swordSize = StatsManager.PlayerStats.SwordSize;

        instanceCol.size = new Vector2(prefabCol.size.x * swordSize, prefabCol.size.y);

        // increase/decrease the offset by half of what the size was changed by
        float sizeChange = instanceCol.size.x - prefabCol.size.x;
        instanceCol.offset = new Vector2(prefabCol.offset.x + sizeChange * 0.5f, prefabCol.offset.y);
    }

    private void SetupBoomerangMovement() {

        Transform sword = ReferenceSystem.Instance.PlayerSword;

        originalLocalPos = sword.localPosition;

        // change parent to move independently of player
        sword.SetParent(Containers.Instance.Projectiles, true);

        boomerangMovement = sword.AddComponent<BoomerangMovement>();

        Vector2 toAttackDirection = attackPos - sword.position;
        boomerangMovement.Setup(toAttackDirection, Stats.ProjectileSpeed, acceleration);

        boomerangMovement.OnReturn += OnSwordReturn;
    }

    private void OnSwordReturn() {
        base.Stop();

        boomerangMovement.OnReturn -= OnSwordReturn;

        // can attack normally again
        PlayerMeleeAttack.Instance.AllowAttack();

        Transform sword = ReferenceSystem.Instance.PlayerSword;
        Transform swordParent = ReferenceSystem.Instance.PlayerWeaponParent;
        sword.SetParent(swordParent);

        //... stop rotating
        Destroy(autoRotate);

        ResetSwordTransform();

        // stop dealing damage through touch
        playerTouchDamage.gameObject.ReturnToPool();

        // remove ability effects
        foreach (GameObject abilityEffect in abilityEffects) {
            abilityEffect.ReturnToPool();
        }
        abilityEffects.Clear();
    }

    private Vector2 originalLocalPos;

    private void ResetSwordTransform() {
        Transform sword = ReferenceSystem.Instance.PlayerSword;
        sword.SetLocalPositionAndRotation(originalLocalPos, Quaternion.identity);
    }

    private List<GameObject> abilityEffects = new();

    public override void ApplyModifier(AbilityStats statsModifier, AbilityAttribute abilityAttributesToModify, GameObject effectPrefab) {
        base.ApplyModifier(statsModifier, abilityAttributesToModify, effectPrefab);
        if (effectPrefab != null) {
            GameObject effect = effectPrefab.Spawn(ReferenceSystem.Instance.PlayerSword);
            abilityEffects.Add(effect);
        }
    }
}
