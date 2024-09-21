using UnityEngine;

public class ReferenceSystem : StaticInstance<ReferenceSystem> {

    protected override void Awake() {
        base.Awake();
        SetPlayerSwordVisual(normalPlayerSwordVisual);
    }

    [Header("Player")]
    [SerializeField] private Transform playerWeaponParent;
    public Transform PlayerWeaponParent => playerWeaponParent;

    [SerializeField] private Transform playerSword;
    public Transform PlayerSword => playerSword;

    [SerializeField] private SpriteRenderer normalPlayerSwordVisual;
    public SpriteRenderer NormalPlayerSwordVisual => normalPlayerSwordVisual;

    private SpriteRenderer playerSwordVisual;
    public SpriteRenderer PlayerSwordVisual => playerSwordVisual;

    public void SetPlayerSwordVisual(SpriteRenderer newVisual) {
        playerSwordVisual = newVisual;
    }
}