using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatModifier", menuName = "Stat Modifier")]
public class ScriptableStatModifier : ScriptableObject {

    [SerializeField] private PlayerStatModifier[] statModifiers;
    public PlayerStatModifier[] StatModifiers => statModifiers;

    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;

}
