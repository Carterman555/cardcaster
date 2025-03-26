using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropEssenceOnDeath : MonoBehaviour {

    public float DropMult = 1f;

    private EnemyHealth health;

    [SerializeField] private EssenceDrop essencePrefab;

    //... not really the avg especially when value is under 1 because it turns negatives to 0
    [SerializeField] private float avgDropAmount;
    [SerializeField] private float dropAmountVariation;

    [SerializeField] private float yVariation = 1f;

    private void Awake() {
        health = GetComponent<EnemyHealth>();

        health.DeathEventTrigger.AddListener(DropEssence);
    }

    private void OnDestroy() {
        health.DeathEventTrigger.RemoveListener(DropEssence);
    }

    private void DropEssence() {
        int amount = GenerateRandomDropAmount();
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
