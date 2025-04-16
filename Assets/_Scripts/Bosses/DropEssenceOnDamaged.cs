using UnityEngine;

public class DropEssenceOnDamaged : MonoBehaviour {

    [SerializeField] private EssenceDrop essencePrefab;

    [SerializeField] private RandomInt dropAmount;
    [SerializeField][Range(0f, 1f)] private float dropChancePerDmg;

    [SerializeField] private float yVariation = 1f;

    [SerializeField] private bool hasMax;
    [ConditionalHide("hasMax")][SerializeField] private float maxDropAmount;
    private float dropCount;

    private EnemyHealth health;

    private void Awake() {
        health = GetComponent<EnemyHealth>();

        health.OnDamagedDetailed += TryDropEssence;
    }

    private void OnEnable() {
        dropCount = 0;
    }

    private void OnDestroy() {
        health.OnDamagedDetailed -= TryDropEssence;
    }

    private void TryDropEssence(float damage, bool shared, bool crit) {

        if (hasMax && dropCount >= maxDropAmount) {
            return;
        }

        float dropChance = dropChancePerDmg * damage;
        dropChance = Mathf.Clamp01(dropChance);

        if (Random.value < dropChance) {
            int amount = dropAmount.Randomize();
            for (int i = 0; i < amount; i++) {

                float yOffset = Random.Range(-yVariation, yVariation);
                Vector3 essencePos = transform.position + Vector3.up * yOffset;
                essencePrefab.Spawn(essencePos, Containers.Instance.Drops);

                dropCount++;
            }
        }
    }
}
