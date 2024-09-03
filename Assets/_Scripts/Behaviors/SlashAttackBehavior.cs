using UnityEngine;

public class SlashAttackBehavior : UnitBehavior {

    private SlashingWeapon weapon;
    private LayerMask targetLayerMask;

    public void Setup(SlashingWeapon weapon, LayerMask targetLayerMask) {
        this.weapon = weapon;
        this.targetLayerMask = targetLayerMask;
    }

    public void Swing(Vector2 attackDirection, float slashSize) {

        weapon.Swing();

        // deal damage
        Vector2 attackCenter = (Vector2)gameObject.transform.position + (attackDirection.normalized * slashSize);
        CircleDamage.DealDamage(targetLayerMask, attackCenter, slashSize, hasStats.GetStats().Damage, hasStats.GetStats().KnockbackStrength);
    }
}
