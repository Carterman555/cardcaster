using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "FireSwordCard", menuName = "Cards/Fire Sword Card")]
public class ScriptableFireSwordCard : ScriptableCardBaseOld {

    // static for abilities that take on fire effect
    public static float BurnDuration { get; private set; } = 3f;

    [SerializeField] private Material fireSwordMaterial;
    [SerializeField] private Material normalSwordMaterial;

    [SerializeField] private TransformWithSwordSize fireEffectsPrefab;
    private TransformWithSwordSize fireEffects;

    public override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.OnAttack_Targets += InflictBurn;

        SpriteRenderer swordVisual = ReferenceSystem.Instance.PlayerSwordVisual;
        swordVisual.material = fireSwordMaterial;

        // create effects and increase size based on sword size
        fireEffects = fireEffectsPrefab.Spawn(swordVisual.transform);
        float swordSize = StatsManager.Instance.GetPlayerStats().SwordSize;

        Vector3 offset = new Vector3(0.31f, 0.27f);
        fireEffects.SetOriginalLocalPos(offset);
    }

    public override void Stop() {
        base.Stop();

        PlayerMeleeAttack.OnAttack_Targets -= InflictBurn;

        SpriteRenderer swordVisual = ReferenceSystem.Instance.PlayerSwordVisual;
        swordVisual.material = normalSwordMaterial;

        fireEffects.gameObject.ReturnToPool();
    }

    private void InflictBurn(Health[] healths) {
        foreach (Health health in healths) {
            health.GetComponent<Enemy>().AddEffect(new Burn(), true, BurnDuration);
        }
    }
}
