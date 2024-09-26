using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordSwingCard", menuName = "Cards/Sword Swing Card")]
public class ScriptableSwordSwingCard : ScriptableAbilityCardBase {

    [SerializeField] private float swingSpeed = 1000f;
    private MMAutoRotate autoRotate;

    [SerializeField] private PlayerTouchDamage playerTouchDamagePrefab;
    private PlayerTouchDamage playerTouchDamage;

    [SerializeField] private ParticleSystem swingSwordEffectsPrefab;
    private ParticleSystem swingSwordEffects;

    public override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.Instance.enabled = false;

        // make sword autorotate
        ReferenceSystem.Instance.PlayerWeaponParent.GetComponent<SlashingWeapon>().enabled = false;
        autoRotate = ReferenceSystem.Instance.PlayerWeaponParent.AddComponent<MMAutoRotate>();
        autoRotate.RotationSpeed = new Vector3(0f, 0f, -swingSpeed);

        // make sword deal damage through touch
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

        // effects
        swingSwordEffects = swingSwordEffectsPrefab.Spawn(sword.position, sword);
    }

    public override void Stop() {
        base.Stop();

        PlayerMeleeAttack.Instance.enabled = true;

        // stop autorotate
        ReferenceSystem.Instance.PlayerWeaponParent.GetComponent<SlashingWeapon>().enabled = true;
        Destroy(autoRotate);

        // stop dealing damage through touch
        playerTouchDamage.gameObject.ReturnToPool();

        // remove visual effects
        swingSwordEffects.gameObject.ReturnToPool();

        // remove ability effects
        foreach (GameObject abilityEffect in abilityEffects) {
            abilityEffect.ReturnToPool();
        }
    }

    private List<GameObject> abilityEffects = new();

    public override void AddEffect(GameObject effectPrefab) {
        base.AddEffect(effectPrefab);
        GameObject effect = effectPrefab.Spawn(PlayerMeleeAttack.Instance.transform);
        abilityEffects.Add(effect);
    }
}
