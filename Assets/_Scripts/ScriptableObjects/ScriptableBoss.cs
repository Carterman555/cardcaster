using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boss", menuName = "Unit/Boss")]
public class ScriptableBoss : ScriptableObject {

    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }

    [field: SerializeField] public GameObject Prefab { get; private set; }

    [field: SerializeField] public Stats Stats { get; private set; }

    [field: SerializeField] public Level PossibleLevels { get; private set; }
}

[Flags]
public enum Level {
    None = 0,
    Level1 = 1 << 0,
    Level2 = 1 << 1,
    Level3 = 1 << 2,
    Level4 = 1 << 3,
    Level5 = 1 << 4,
    Level6 = 1 << 5,
    Level7 = 1 << 6
}