using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewEnchantment", menuName = "Enchantment")]
public class ScriptableEnchantment : ScriptableObject {

    [SerializeField] private EnchantmentType enchantmentType;
    public EnchantmentType EnchantmentType => enchantmentType;

    [SerializeField] private LocalizedString enchantmentName;
    public string Name => enchantmentName.GetLocalizedString();

    [SerializeField] private bool hasDescription;
    public bool HasDescription => hasDescription;

    [SerializeField, ConditionalHide("hasDescription")] private LocalizedString description;
    public string Description => description.GetLocalizedString();

    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;

    [SerializeField] private PlayerStatModifier[] statModifiers;
    public PlayerStatModifier[] StatModifiers => statModifiers;

    public virtual void OnGain() {
    }
}

public enum EnchantmentType {
    BerserkersMight,
    EssenceWell,
    FleetFoot,
    GlassCannon,
    KeenEye,
    LethalPrecision,
    SagesWisdom,
    SpellBolt,
    SwiftRecovery,
    ThunderousImpact,
    VitalityWard,
    WhirlwindStance
}
