using UnityEngine;

public class DropEssenceOnDeath : MonoBehaviour {

    public float DropMult = 1f;

    private EnemyHealth health;

    [SerializeField] private EssenceDrop essencePrefab;

    //... not really the avg especially when value is under 1 because it turns negatives to 0
    [SerializeField] private float avgDropAmount;
    [SerializeField] private float dropAmountVariation;

    [SerializeField] private float yVariation = 1f;

    //... can't use enabled because it's false when the gameobject is not active which happens
    //... right as the enemy dies and would drop the essence, so it never would
    public bool IsEnabled;

    [SerializeField] private bool debugAlwaysDrop;

    private void Awake() {
        health = GetComponent<EnemyHealth>();

        health.DeathEventTrigger.AddListener(DropEssence);
    }

    private void OnDestroy() {
        health.DeathEventTrigger.RemoveListener(DropEssence);
    }

    private void DropEssence() {

        if (!IsEnabled) {
            return;
        }

        int amount = GenerateRandomDropAmount();

        if (debugAlwaysDrop) {
            amount = 1;
        }

        for (int i = 0; i < amount; i++) {
            float yOffset = Random.Range(-yVariation, yVariation);
            Vector3 essencePos = transform.position + Vector3.up * yOffset;
            essencePrefab.Spawn(essencePos, Containers.Instance.Drops);
        }
    }

    // generate random amount based on normal distribution
    private int GenerateRandomDropAmount() {
        float mean = avgDropAmount * DropMult;
        float stdDev = dropAmountVariation;

        // Box-Muller transform
        float u1 = 1.0f - Random.value; // Avoid zero
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

        float floatAmount = mean + stdDev * randStdNormal;
        return Mathf.Max(Mathf.RoundToInt(floatAmount), 0);
    }

    [ContextMenu("PrintSampleAmount")]
    private void PrintSampleAmount() {
        for (int i = 0; i < 1000; i++) {
            print(GenerateRandomDropAmount());
        }
    }

}
