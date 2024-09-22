using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ElectricSwordCard", menuName = "Cards/Electric Sword Card")]
public class ScriptableElectricSwordCard : ScriptableCardBase {

    // static for abilities that take on electric effect
    public static int ElectricUnitAmount { get; private set; } = 3;
    public static float ElectricDamage { get; private set; } = 0.5f;

    [SerializeField] private Material electricSwordMaterial;
    [SerializeField] private Material normalSwordMaterial;

    [SerializeField] private TransformWithSwordSize electricEffectsPrefab;
    private TransformWithSwordSize electricEffects;

    public override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.OnAttack_Targets += CreateElectricity;

        SpriteRenderer swordVisual = ReferenceSystem.Instance.PlayerSwordVisual;
        swordVisual.material = electricSwordMaterial;

        // create effects and increase size based on sword size
        electricEffects = electricEffectsPrefab.Spawn(swordVisual.transform);
        float swordSize = StatsManager.Instance.GetPlayerStats().SwordSize;

        Vector3 offset = new Vector3(0.31f, 0.27f);
        electricEffects.SetOriginalLocalPos(offset);
    }

    public override void Stop() {
        base.Stop();

        PlayerMeleeAttack.OnAttack_Targets -= CreateElectricity;

        SpriteRenderer swordVisual = ReferenceSystem.Instance.PlayerSwordVisual;
        swordVisual.material = normalSwordMaterial;

        electricEffects.gameObject.ReturnToPool();
    }

    private void CreateElectricity(Health[] healths) {
        foreach (Health health in healths) {
            ElectricChain electricChain = health.AddComponent<ElectricChain>();
            electricChain.Setup(ElectricUnitAmount);
        }
    }
}
