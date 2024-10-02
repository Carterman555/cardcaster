using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PopupOnDamaged : MonoBehaviour {

    [SerializeField] private DamagePopup damagePopupPrefab;
    [SerializeField] private float yOffset;

    private Health health;

    private void Awake() {
        health = GetComponent<Health>();
    }

    private void OnEnable() {
        health.OnDamaged_Damage += CreateDamagePopup;
    }
    private void OnDisable() {
        health.OnDamaged_Damage -= CreateDamagePopup;
    }

    public void CreateDamagePopup(float damage) {
        Vector2 position = (Vector2)transform.position + new Vector2(0, yOffset);
        DamagePopup damagePopup = damagePopupPrefab.Spawn(position, Containers.Instance.Effects);
        damagePopup.Setup(Mathf.RoundToInt(damage));
    }
}
