using DG.Tweening;
using QFSW.QC;
using UnityEngine;

public class HealerMinion : Enemy {

    private BounceMoveBehaviour bounceMoveBehaviour;
    private Rigidbody2D rb;

    [SerializeField] private float jumpPower;
    [SerializeField] private float jumpDuration;

    private bool jumping;

    protected override void Awake() {
        base.Awake();

        bounceMoveBehaviour = GetComponent<BounceMoveBehaviour>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        jumping = false;
        bounceMoveBehaviour.enabled = true;

        GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!jumping && collision.name == "HealerMinionTrigger") {
            jumping = true;
            bounceMoveBehaviour.enabled = false;

            GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;

            transform.DOJump(collision.transform.position, jumpPower, numJumps: 1, jumpDuration).SetEase(Ease.Linear).OnComplete(() => {
                // TODO - heal drchonk (drChonk.Heal())
                gameObject.ReturnToPool();
            });
        }
    }
}
