using DG.Tweening;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRing : MonoBehaviour {

    [SerializeField] private DamageOnContact cardPrefab;
    [SerializeField] private ParticleSystem cardParticlesPrefab;
    [SerializeField] private Color cardParticlesColor;

    private Rigidbody2D rb;

    private float radius;
    private int amount;
    private float damage;
    private float speed;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        rb.velocity = Vector3.zero;
    }

    public void Setup(float radius, int amount, float damage, float speed, float circularSpeed) {
        this.radius = radius;
        this.amount = amount;
        this.damage = damage;
        this.speed = speed;

        rb.angularVelocity = -circularSpeed;

        StartCoroutine(SpawnCards());
    }

    private IEnumerator SpawnCards() {

        GameObject[] cards = new GameObject[amount];

        float angleBetweenCards = 360f / amount;
        Vector2 cardDirection = Vector2.up;

        // spawn cards
        for (int i = 0; i < amount; i++) {
            Vector2 pos = (Vector2)transform.position + cardDirection * radius;
            cardDirection.RotateDirection(angleBetweenCards);

            DamageOnContact card = cardPrefab.Spawn(pos, transform);
            card.Setup(damage, knockbackStrength: 1f);

            card.gameObject.SetActive(false);
            card.GetComponent<Collider2D>().enabled = false;

            card.GetComponent<SpriteRenderer>().Fade(0.5f);

            cards[i] = card.gameObject;
        }

        // enable cards one at a time
        foreach (GameObject card in cards) {

            card.transform.up = Vector2.right;
            card.SetActive(true);
            cardParticlesPrefab.CreateColoredParticles(card.transform.position, cardParticlesColor);

            // wait until spawned card has moved the correct amount for even spacing
            float cardAngle = 0f;
            while (cardAngle < angleBetweenCards) {
                Vector2 toCardDirection = (card.transform.position - transform.position).normalized;
                cardAngle = Vector2.Angle(Vector2.up, toCardDirection);
                yield return null;
            }
        }

        float fadeDuration = 0.5f;

        // activate cards to deal damage and move towards player
        foreach (GameObject card in cards) {
            card.GetComponent<SpriteRenderer>().DOFade(1f, fadeDuration).OnComplete(() => {
                card.GetComponent<Collider2D>().enabled = true;
            });
        }

        yield return new WaitForSeconds(fadeDuration);

        Vector2 toPlayerDirection = (PlayerMovement.Instance.CenterPos - transform.position).normalized;
        rb.velocity = toPlayerDirection * speed;
    }
}
