using Febucci.UI;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ReferenceSystem : StaticInstance<ReferenceSystem> {

    [Header("Player")]
    [SerializeField] private Transform playerWeaponParent;
    public Transform PlayerWeaponParent => playerWeaponParent;

    [SerializeField] private Transform playerSword;
    public Transform PlayerSword => playerSword;

    [SerializeField] private SpriteRenderer playerSwordVisual;
    public SpriteRenderer PlayerSwordVisual => playerSwordVisual;

    [Header("For Tutorial")]
    [SerializeField] private TypewriterByCharacter dialogTypewriter;
    public TypewriterByCharacter DialogTypewriter => dialogTypewriter;

    [SerializeField] private MMF_Player cardsPanelFeedbacks;
    public MMF_Player CardsPanelFeedbacks => cardsPanelFeedbacks;

    [SerializeField] private MMF_Player enchantmentsPanelFeedbacks;
    public MMF_Player EnchantmentsPanelFeedbacks => enchantmentsPanelFeedbacks;

    [SerializeField] private MMF_Player mapPanelFeedbacks;
    public MMF_Player MapPanelFeedbacks => mapPanelFeedbacks;
}