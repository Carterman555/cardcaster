using UnityEngine;

public class ReferenceSystem : StaticInstance<ReferenceSystem> {

    [Header("Player")]
    [SerializeField] private Transform playerWeaponParent;
    public Transform PlayerWeaponParent => playerWeaponParent;
}