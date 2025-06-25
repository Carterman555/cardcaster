using UnityEngine;

public class WraithAttack : MonoBehaviour {

    [SerializeField] private AudioClips attackSfx;

    [SerializeField] private Vector2 attackSize;
    [SerializeField] private Vector2 attackOffset;

    private Transform wraithTransform;
    private float damage;
    private float knockbackStrength;
    private Vector2 direction;

    public void Setup(Transform wraithTransform, float damage, float knockbackStrength, Vector2 direction) {
        this.wraithTransform = wraithTransform;
        this.damage = damage;
        this.knockbackStrength = knockbackStrength;
        this.direction = direction;

        if (direction == Vector2.right) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z));
        }
        else if (direction == Vector2.left) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
        }
        else {
            Debug.LogError($"Doesn't support direction {direction}!");
        }

        AudioManager.Instance.PlaySound(attackSfx);
    }

    // played by anim
    public void DealDamage() {
        if (wraithTransform == null) {
            Debug.LogError("Wraith attack not setup!");
            return;
        }

        Vector2 directionalOffset = attackOffset;
        directionalOffset.x *= direction.x;
        Vector2 attackCenter = (Vector2)transform.position + directionalOffset;
        DamageDealer.DealCapsuleDamage(
            GameLayers.PlayerLayerMask,
            wraithTransform.position,
            attackCenter,
            attackSize,
            angle: 90f,
            damage,
            knockbackStrength
        );
    }

    // played by anim
    public void Return() {
        gameObject.ReturnToPool();
    }

    private void OnDisable() {
        wraithTransform = null;
    }
}
