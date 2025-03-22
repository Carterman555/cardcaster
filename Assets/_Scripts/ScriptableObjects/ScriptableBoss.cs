using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boss", menuName = "Unit/Boss")]
public class ScriptableBoss : ScriptableObject {

    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }

    [field: SerializeField] public GameObject Prefab { get; private set; }

    [field: SerializeField] public CommonStats Stats { get; private set; }

    [field: SerializeField] public int[] PossibleLevels { get; private set; }
}