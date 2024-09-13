using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DaggerShootCard", menuName = "Cards/Dagger Shoot Card")]
public class ScriptableDaggerShootCard : ScriptableCardBase {

    [SerializeField] private StraightMovement daggerPrefab;
    [SerializeField] private float spawnOffsetValue;

    [Header("Stats")]
    [SerializeField] private float shootCooldown;
    [SerializeField] private float damage;
    [SerializeField] private float knockbackStrength;

    private Coroutine shootCoroutine;

    public override void Play(Vector2 position) {
        base.Play(position);

        shootCoroutine = DeckManager.Instance.StartCoroutine(ShootDaggers());
    }

    public override void Stop() {
        base.Stop();

        DeckManager.Instance.StopCoroutine(shootCoroutine);
    }

    private IEnumerator ShootDaggers() {
        while (true) {
            yield return new WaitForSeconds(shootCooldown);

            Vector2 toMouseDirection = (MouseTracker.Instance.transform.position - PlayerMovement.Instance.transform.position).normalized;
            Vector2 offset = spawnOffsetValue * toMouseDirection;
            Vector2 spawnPos = (Vector2)PlayerMovement.Instance.transform.position + offset;
            StraightMovement straightMovement = daggerPrefab.Spawn(spawnPos, Containers.Instance.Projectiles);
            straightMovement.Setup(toMouseDirection);
            straightMovement.GetComponent<DamageOnContact>().Setup(damage, knockbackStrength);
        }
    }
}
