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

    private BoxCollider2D prefabCol;
    private BoxCollider2D instanceCol;

    [SerializeField] private ParticleSystem swingSwordEffectsPrefab;
    private ParticleSystem swingSwordEffects;

    private Coroutine updateCor;

    protected override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.Instance.DisableAttack();

        // make sword autorotate
        ReferenceSystem.Instance.PlayerWeaponParent.GetComponent<SlashingWeapon>().enabled = false;
        autoRotate = ReferenceSystem.Instance.PlayerWeaponParent.AddComponent<MMAutoRotate>();
        autoRotate.RotationSpeed = new Vector3(0f, 0f, -swingSpeed);

        // make sword deal damage through touch
        Transform sword = ReferenceSystem.Instance.PlayerSword;
        Quaternion damageRotation = Quaternion.Euler(0, 0, sword.eulerAngles.z + 65f);
        playerTouchDamage = playerTouchDamagePrefab.Spawn(sword.position, damageRotation, sword);

        prefabCol = playerTouchDamagePrefab.GetComponent<BoxCollider2D>();
        instanceCol = playerTouchDamage.GetComponent<BoxCollider2D>();

        updateCor = AbilityManager.Instance.StartCoroutine(UpdateCor());

        // effects
        swingSwordEffects = swingSwordEffectsPrefab.Spawn(sword.position, sword);
    }

    // use update cor because sword size could change while sword is swinging
    private IEnumerator UpdateCor() {
        while (true) {
            yield return null;

            // update collider size to match sword size
            float swordSize = StatsManager.Instance.GetPlayerStats().SwordSize;
            instanceCol.size = new Vector2(prefabCol.size.x * swordSize, prefabCol.size.y);
            instanceCol.offset = new Vector2(prefabCol.offset.x * swordSize, prefabCol.offset.y);
        }
    }

    public override void Stop() {
        base.Stop();

        PlayerMeleeAttack.Instance.AllowAttack();

        // stop autorotate
        ReferenceSystem.Instance.PlayerWeaponParent.GetComponent<SlashingWeapon>().enabled = true;
        Destroy(autoRotate);

        // stop dealing damage through touch
        playerTouchDamage.gameObject.ReturnToPool();
        AbilityManager.Instance.StopCoroutine(updateCor);

        // remove visual effects
        swingSwordEffects.gameObject.ReturnToPool();

        // remove ability effects
        foreach (GameObject abilityEffect in abilityEffects) {
            abilityEffect.ReturnToPool();
        }
        abilityEffects.Clear();
    }

    private List<GameObject> abilityEffects = new();

    public override void AddEffect(GameObject effectPrefab) {
        base.AddEffect(effectPrefab);
        GameObject effect = effectPrefab.Spawn(ReferenceSystem.Instance.PlayerSword);
        abilityEffects.Add(effect);
    }
}
