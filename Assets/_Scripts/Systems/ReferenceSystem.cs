using UnityEngine;

public class ReferenceSystem : StaticInstance<ReferenceSystem> {

    [Header("Player")]
    [SerializeField] private Transform playerWeaponParent;
    public Transform PlayerWeaponParent => playerWeaponParent;

    [SerializeField] private Transform playerSword;
    public Transform PlayerSword => playerSword;

    [SerializeField] private SpriteRenderer playerSwordVisual;
    public SpriteRenderer PlayerSwordVisual => playerSwordVisual;

    [Header("Misc")]
    [SerializeField] private GameObject cameraConfiner;
    public GameObject CameraConfiner => cameraConfiner;
}