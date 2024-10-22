using UnityEngine;

public class StraightMovement : MonoBehaviour {

    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;
    private IDelayedReturn[] delayedReturns;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        delayedReturns = GetComponents<IDelayedReturn>();
    }

    public void Setup(Vector2 direction) {
        transform.up = direction;
        rb.velocity = direction * moveSpeed;
    }

    public void Setup(Vector2 direction, float moveSpeed) {
        transform.up = direction;
        this.moveSpeed = moveSpeed;
        rb.velocity = direction * moveSpeed;
    }

    // whenever a behavior starts to return this object, stop moving (this would be when the death animation starts)
    private void OnEnable() {
        foreach (var delayedReturn in delayedReturns) {
            delayedReturn.OnStartReturn += Stop;
        }
    }
    private void OnDisable() {
        foreach (var delayedReturn in delayedReturns) {
            delayedReturn.OnStartReturn -= Stop;
        }
    }

    public void Stop() {
        rb.velocity = Vector2.zero;
    }
}
