using UnityEngine;

public class HealerMinion : Enemy {

    private BounceMoveBehaviour bounceMoveBehaviour;
    private Rigidbody2D rb;

    protected override void Awake() {
        base.Awake();

        bounceMoveBehaviour = GetComponent<BounceMoveBehaviour>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        bounceMoveBehaviour.enabled = true;
        gettingSucked = false;
    }

    [Header("Getting Sucked")]
    private Vector2 suckCenter;
    private bool gettingSucked;
    [SerializeField] private float suckAcceleration;
    [SerializeField] private float maxSuckSpeed;

    public void SuckToChonk(Vector2 suckCenter) {
        bounceMoveBehaviour.enabled = false;

        this.suckCenter = suckCenter;
        gettingSucked = true;

        GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
    }

    public void StopSuck() {
        bounceMoveBehaviour.enabled = true;
        gettingSucked = false;

        GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
    }

    private void FixedUpdate() {
        if (gettingSucked) {
            Vector2 suckDirection = (suckCenter - (Vector2)transform.position).normalized;
            rb.velocity = Vector2.MoveTowards(rb.velocity, suckDirection * maxSuckSpeed, suckAcceleration * Time.fixedDeltaTime);
        }
    }

}
