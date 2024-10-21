using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerMinion : Enemy {

    //private WallBounceMovement wallBounceMovement;
    private Rigidbody2D rb;

    protected override void Awake() {
        base.Awake();

        //wallBounceMovement = GetComponent<WallBounceMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        //wallBounceMovement.enabled = true;
        gettingSucked = false;
    }

    private Vector2 suckCenter;
    private bool gettingSucked;

    public void SuckToChonk(Vector2 suckCenter) {
        //wallBounceMovement.enabled = false;

        this.suckCenter = suckCenter;
        gettingSucked = true;
    }

    protected override void Update() {
        base.Update();

        if (gettingSucked) {
            //rb.velocity = Vector2.MoveTowards()
        }
    }

}
