using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "FireSwordCard", menuName = "Cards/Fire Sword Card")]
public class ScriptableFireSwordShockwave : ScriptableCardBase {

    [SerializeField] private float burnDuration = 3f;

    public override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.OnAttack_Targets += InflictBurn;
    }

    public override void Stop() {
        base.Stop();

        PlayerMeleeAttack.OnAttack_Targets -= InflictBurn;
    }

    private void InflictBurn(Health[] healths) {
        foreach (Health health in healths) {
            health.GetComponent<Enemy>().AddEffect(new Burn(), true, burnDuration);
        }
    }
}
