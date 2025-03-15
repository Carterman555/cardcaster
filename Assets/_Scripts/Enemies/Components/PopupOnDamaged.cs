using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDamagable))]
public class PopupOnDamaged : MonoBehaviour {

    [SerializeField] private DamagePopup damagePopupPrefab;
    [SerializeField] private float yOffset;

    private IDamagable damagable;

    private void Awake() {
        damagable = GetComponent<IDamagable>();
    }

    private void OnEnable() {
        damagable.OnDamaged_Damage_Shared += CreateDamagePopup;
    }
    private void OnDisable() {
        damagable.OnDamaged_Damage_Shared -= CreateDamagePopup;
    }

    public void CreateDamagePopup(float damage, bool fromShared) {

        if (fromShared) {
            return;
        }

        float xVariation = 0.25f;
        float yVariation = 0.25f;

        Vector2 position = (Vector2)transform.position + new Vector2(0, yOffset);
        position.x += Random.Range(-xVariation, xVariation);
        position.y += Random.Range(-yVariation, yVariation);

        DamagePopup damagePopup = damagePopupPrefab.Spawn(position, Containers.Instance.Effects);
        damagePopup.Setup(damage);
    }
}
