using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ElectricSwordCard", menuName = "Cards/Electric Sword Card")]
public class ScriptableElectricSwordCard : ScriptableCardBase {

    // static for abilities that take on electric effect
    public static int ElectricUnitAmount { get; private set; } = 3;
    public static float ElectricDamage { get; private set; } = 0.5f;

    [SerializeField] private Transform electricSwordPrefab;
    private Transform electricSword;

    public override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.OnAttack_Targets += CreateElectricity;

        SpriteRenderer swordVisual = PlayerMeleeAttack.Instance.GetSwordVisual();
        swordVisual.gameObject.SetActive(false);

        electricSword = electricSwordPrefab.Spawn(swordVisual.transform.parent);
        electricSword.localRotation = Quaternion.identity;
    }

    public override void Stop() {
        base.Stop();

        PlayerMeleeAttack.OnAttack_Targets -= CreateElectricity;

        SpriteRenderer swordVisual = PlayerMeleeAttack.Instance.GetSwordVisual();
        swordVisual.gameObject.SetActive(true);

        electricSword.gameObject.ReturnToPool();
    }

    private void CreateElectricity(Health[] healths) {
        foreach (Health health in healths) {
            ElectricChain electricChain = health.AddComponent<ElectricChain>();
            electricChain.Setup(ElectricUnitAmount);
        }
    }
}
