// useless
public class BlindedCyclops : Enemy {

    private CircleStraightShootBehavior shootBehavior;

    protected override void Awake() {
        base.Awake();

        shootBehavior = GetComponent<CircleStraightShootBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        shootBehavior.enabled = true;
    }
}
