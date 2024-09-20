using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordSwingCard", menuName = "Cards/Sword Swing Card")]
public class ScriptableSwordSwingCard : ScriptableCardBase {

    [SerializeField] private float swingSpeed = 1000f;
    private MMAutoRotate autoRotate;

    public override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.Instance.enabled = false;

        // make sword autorotate
        ReferenceSystem.Instance.PlayerWeaponParent.GetComponent<SlashingWeapon>().enabled = false;
        autoRotate = ReferenceSystem.Instance.PlayerWeaponParent.AddComponent<MMAutoRotate>();
        autoRotate.RotationSpeed = new Vector3(0f, 0f, -swingSpeed);
    }

    public override void Stop() {
        base.Stop();

        PlayerMeleeAttack.Instance.enabled = true;

        // stop autorotate
        ReferenceSystem.Instance.PlayerWeaponParent.GetComponent<SlashingWeapon>().enabled = true;
        Destroy(autoRotate);
    }
}
