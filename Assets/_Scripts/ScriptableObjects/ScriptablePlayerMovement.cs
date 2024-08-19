using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMoveSettings", menuName = "Settings/Player Move Settings")]
public class ScriptablePlayerMovement : ScriptableObject {

    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float DashSpeed { get; private set; }
    [field: SerializeField] public float DashTime { get; private set; }

}
