using System.Collections;
using UnityEngine;

public class ShinyGoblin : Enemy {

    [SerializeField] private RandomFloat fleeDelay;
    private float fleeTimer;

    private WanderMovementBehavior wanderMovement;

    [SerializeField] private EnchantmentOrb enchantmentOrbPrefab;

    [SerializeField] private AudioClips quietTeleportSfx;

    protected override void Awake() {
        base.Awake();

        wanderMovement = GetComponent<WanderMovementBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        health.DeathEventTrigger.AddListener(SpawnEnchantmentOrb);

        fleeTimer = 0;
        fleeDelay.Randomize();

        wanderMovement.enabled = true;
    }

    protected override void OnDisable() {
        base.OnDisable();
        health.DeathEventTrigger.RemoveListener(SpawnEnchantmentOrb);
    }

    protected override void Update() {
        base.Update();

        fleeTimer += Time.deltaTime;
        if (fleeTimer > fleeDelay.Value) {
            fleeTimer = 0;

            wanderMovement.enabled = false;

            anim.SetTrigger("flee");

            StartCoroutine(FleeSfxCor());
        }
    }

    private void SpawnEnchantmentOrb() {
        enchantmentOrbPrefab.Spawn(transform.position, Containers.Instance.Drops);
    }

    private IEnumerator FleeSfxCor() {

        yield return new WaitForSeconds(0.5f);

        AudioManager.Instance.PlaySound(quietTeleportSfx);
        yield return new WaitForSeconds(0.66f);

        AudioManager.Instance.PlaySound(quietTeleportSfx);
        yield return new WaitForSeconds(0.66f);

        AudioManager.Instance.PlaySound(quietTeleportSfx);
        yield return new WaitForSeconds(0.66f);

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Teleport);
    }

    // played by flee anim
    public void ReturnToPool() {
        gameObject.ReturnToPool();
    }
}
