using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Boss", menuName = "Unit/Boss")]
public class ScriptableBoss : ScriptableObject {

    [field: SerializeField] public LocalizedString LocName { get; private set; }

    [field: SerializeField] public GameObject Prefab { get; private set; }

    [field: SerializeField] public EnemyStats Stats { get; private set; }

    [field: SerializeField] public int[] PossibleLevels { get; private set; }
}