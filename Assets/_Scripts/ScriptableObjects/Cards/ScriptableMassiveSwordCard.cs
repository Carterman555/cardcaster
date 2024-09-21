using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MassiveSwordCard", menuName = "Cards/Massive Sword Card")]
public class ScriptableMassiveSwordCard : ScriptableCardBase {

    [Header("Stats")]
    [SerializeField] private float damageMult;
    [SerializeField] private float knockbackStrengthMult;

    public override void Play(Vector2 position) {
        base.Play(position);

        PlayerMovement.Instance.enabled = false;

    }

    public override void Stop() {
        base.Stop();

        PlayerMovement.Instance.enabled = true;

    }
}
