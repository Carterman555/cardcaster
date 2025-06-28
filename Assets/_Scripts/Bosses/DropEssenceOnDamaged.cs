using UnityEngine;

public class DropEssenceOnDamaged : MonoBehaviour {

    [SerializeField] private EssenceDrop essencePrefab;

    [SerializeField] private RandomInt dropAmount;
    [SerializeField][Range(0f, 1f)] private float dropChancePerDmg;

    [SerializeField] private float yOffset = 0f;
    [SerializeField] private float yVariation = 1f;

    [SerializeField] private bool hasMax;
    [ConditionalHide("hasMax")][SerializeField] private float maxDropAmount;
    private float dropCount;

    private IDamagable damagable;

    private void Awake() {
        damagable = GetComponent<IDamagable>();

        damagable.OnDamagedDetailed += TryDropEssence;
    }

    private void OnEnable() {
        dropCount = 0;
    }

    private void OnDestroy() {
        damagable.OnDamagedDetailed -= TryDropEssence;
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

                float yVariationOffset = Random.Range(-yVariation, yVariation);
                Vector3 essencePos = transform.position + Vector3.up * (yVariationOffset + yOffset);
                essencePrefab.Spawn(essencePos, Containers.Instance.Drops);

                dropCount++;
            }
        }
    }
}
