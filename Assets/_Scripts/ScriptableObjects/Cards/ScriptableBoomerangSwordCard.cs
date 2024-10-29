using MoreMountains.Tools;
using System.Collections;
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

    protected override void Play(Vector2 position) {

        base.Play(position);

        PlayerMeleeAttack.Instance.enabled = false;

        //... make sword deal damage through touch
        SetupSwordTouchDamage();

        SetupBoomerangMovement();

        // make sword autorotate
        autoRotate = ReferenceSystem.Instance.PlayerSword.AddComponent<MMAutoRotate>();
        autoRotate.RotationSpeed = new Vector3(0f, 0f, -rotateSpeed);
    }

    private void SetupSwordTouchDamage() {
        Transform sword = ReferenceSystem.Instance.PlayerSword;
        Quaternion damageRotation = Quaternion.Euler(0, 0, sword.eulerAngles.z + 65f);
        playerTouchDamage = playerTouchDamagePrefab.Spawn(sword.position, damageRotation, sword);

        BoxCollider2D prefabCol = playerTouchDamagePrefab.GetComponent<BoxCollider2D>();
        BoxCollider2D instanceCol = playerTouchDamage.GetComponent<BoxCollider2D>();

        //. update collider size to match sword size
        float swordSize = StatsManager.Instance.GetPlayerStats().SwordSize;

        instanceCol.size = new Vector2(prefabCol.size.x * swordSize, prefabCol.size.y);

        //... only increase the offset by half
        float offsetMult = ((swordSize - 1) * 0.5f) + 1;
        instanceCol.offset = new Vector2(prefabCol.offset.x * offsetMult, prefabCol.offset.y);
    }

    private void SetupBoomerangMovement() {

        Transform sword = ReferenceSystem.Instance.PlayerSword;

        originalLocalPos = sword.localPosition;

        // change parent to move independently of player
        sword.SetParent(Containers.Instance.Projectiles);

        boomerangMovement = sword.AddComponent<BoomerangMovement>();

        Vector2 toMouseDirection = MouseTracker.Instance.ToMouseDirection(sword.position);
        boomerangMovement.Setup(toMouseDirection, Stats.ProjectileSpeed, acceleration);

        boomerangMovement.OnReturn += OnSwordReturn;
    }

    private void OnSwordReturn() {
        base.Stop();

        boomerangMovement.OnReturn -= OnSwordReturn;

        // can attack normally again
        PlayerMeleeAttack.Instance.enabled = true;

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
        sword.localPosition = originalLocalPos;
        sword.localRotation = Quaternion.identity;
    }

    private List<GameObject> abilityEffects = new();

    public override void AddEffect(GameObject effectPrefab) {
        base.AddEffect(effectPrefab);
        GameObject effect = effectPrefab.Spawn(ReferenceSystem.Instance.PlayerSword);
        abilityEffects.Add(effect);
    }
}
