using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewEnchantment", menuName = "Enchantment")]
public class ScriptableEnchantment : ScriptableObject {

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
}
