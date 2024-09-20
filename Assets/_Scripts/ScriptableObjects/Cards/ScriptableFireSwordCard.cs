using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "FireSwordCard", menuName = "Cards/Fire Sword Card")]
public class ScriptableFireSwordCard : ScriptableCardBase {

    // static for abilities that take on fire effect
    public static float BurnDuration { get; private set; } = 3f;

    [SerializeField] private Transform fireSwordPrefab;
    private Transform fireSword;

    public override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.OnAttack_Targets += InflictBurn;

        SpriteRenderer swordVisual = ReferenceSystem.Instance.PlayerSwordVisual;
        swordVisual.gameObject.SetActive(false);

        fireSword = fireSwordPrefab.Spawn(swordVisual.transform.parent);
        fireSword.localRotation = Quaternion.identity;
    }

    public override void Stop() {
        base.Stop();

        PlayerMeleeAttack.OnAttack_Targets -= InflictBurn;

        SpriteRenderer swordVisual = ReferenceSystem.Instance.PlayerSwordVisual;
        swordVisual.gameObject.SetActive(true);

        fireSword.gameObject.ReturnToPool();
    }

    private void InflictBurn(Health[] healths) {
        foreach (Health health in healths) {
            health.GetComponent<Enemy>().AddEffect(new Burn(), true, BurnDuration);
        }
    }
}
