using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnchantmentOrb : MonoBehaviour {

    [SerializeField] private float destinationObstacleDistance;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private EnchantmentDrop enchantmentDropPrefab;

    private float originalFade;

    [SerializeField] private AudioClips enchantmentSpawnSfx;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalFade = spriteRenderer.color.a;
    }

    private void OnEnable() {
        spriteRenderer.Fade(0f);
        spriteRenderer.DOFade(originalFade, duration: 1f);

        StartCoroutine(MoveToValidPos());
    }

    private IEnumerator MoveToValidPos() {

        //... wait for pos to get set when orb spawns
        yield return null;

        Vector2 destination = FindDestination();
        transform.DOMove(destination, duration: 1f).OnComplete(() => {
            SpawnEnchantmentDrops();

            AudioManager.Instance.PlaySound(enchantmentSpawnSfx);

            spriteRenderer.DOFade(0f, duration: 0.2f).OnComplete(() => {
                gameObject.ReturnToPool();
            });
        });
    }

    // move to a location away from obstacles where the player will be able to choose stat
    private Vector2 FindDestination() {

        if (IsValidPos(transform.position)) {
            return transform.position;
        }

        int maxIterations = 500;
        int iterationCounter = 0;

        float searchDistanceIncrement = 0.5f;
        float maxSearchDistance = 5f;
        float currentSearchDistance = 0.5f;

        while (currentSearchDistance <= maxSearchDistance) {

            float searchAngleIncrement = 20f;
            float currentSearchAngle = 0f;
            while (currentSearchAngle <= 360) {

                Vector2 currentDirection = Vector2.up.GetDirectionRotated(currentSearchAngle);
                Vector2 currentPos = (Vector2)transform.position + (currentDirection * currentSearchDistance);

                if (IsValidPos(currentPos)) {
                    return currentPos;
                }

                currentSearchAngle += searchAngleIncrement;

                iterationCounter++;
                if (iterationCounter >= maxIterations) {
                    Debug.LogError("Max iterations reached!");
                    return transform.position;
                }
            }

            currentSearchDistance += searchDistanceIncrement;
        }

        Debug.LogWarning("No valid positions found!");
        return transform.position;
    }

    private bool IsValidPos(Vector2 pos) {
        Collider2D obstacleCol = Physics2D.OverlapCircle(pos, destinationObstacleDistance, GameLayers.ObstacleLayerMask);
        bool nearObstacle = obstacleCol != null;

        PolygonCollider2D currentRoomCollider = Room.GetCurrentRoom().GetComponent<PolygonCollider2D>();
        bool inRoom = currentRoomCollider.OverlapPoint(pos);

        return !nearObstacle && inRoom;
    }

    private void SpawnEnchantmentDrops() {

        EnchantmentDrop[] enchantmentDrops = new EnchantmentDrop[3];

        List<ScriptableEnchantment> possibleEnchantments = new(ResourceSystem.Instance.Enchantments);

        if (StatsManager.PlayerStats.HandSize >= DeckManager.MaxHandSize) {
            ScriptableEnchantment sagesWisdom = possibleEnchantments.FirstOrDefault(e => e.EnchantmentType == EnchantmentType.SagesWisdom);
            possibleEnchantments.Remove(sagesWisdom);
        }

        for (int i = 0; i < 3; i++) {
            EnchantmentDrop enchantmentDrop = enchantmentDropPrefab.Spawn(transform.position, Containers.Instance.Drops);

            ScriptableEnchantment enchantment = possibleEnchantments.RandomItem();
            possibleEnchantments.Remove(enchantment);

            Vector2 pos = (Vector2)transform.position + GetDropPosition(i);
            enchantmentDrop.Setup(enchantment, pos);

            // the center drop creates a map icon
            if (i == 1) {
                enchantmentDrop.GetComponent<CreateMapIcon>().ShowMapIcon();
            }

            enchantmentDrops[i] = enchantmentDrop;
        }

        foreach (EnchantmentDrop enchantmentDrop in enchantmentDrops) {
            enchantmentDrop.EnchantmentDropsInGroup = enchantmentDrops;
        }
    }

    private Vector2 GetDropPosition(int itemIndex) {
        if (itemIndex == 0) return new Vector2(-1.3f, 0f);
        if (itemIndex == 1) return new Vector2(0f, 0.6f);
        if (itemIndex == 2) return new Vector2(1.3f, 0f);
        else {
            Debug.LogError("itemIndex position not set: " + itemIndex);
            return Vector2.zero;
        }
    }
}
