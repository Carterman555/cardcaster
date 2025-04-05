using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SpawnEffect : MonoBehaviour {

    private Enemy enemyPrefab;

    private void OnEnable() {
        transform.localScale = Vector3.zero;
    }

    public void Setup(Enemy enemyToSpawn) {
        enemyPrefab = enemyToSpawn;
        Grow();

        StartCoroutine(SpawnEnemyDelayed());

        AudioManager.Instance.PlaySingleSound(AudioManager.Instance.AudioClips.SpawnEnemy);
    }
    
    private void Grow() {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, duration: 0.3f);
    }

    private IEnumerator SpawnEnemyDelayed() {

        float spawnDelay = 2.5f;
        yield return new WaitForSeconds(spawnDelay);

        SpawnEnemy();
        FadeOut();
    } 

    private void SpawnEnemy() {
        enemyPrefab.Spawn(transform.position, Containers.Instance.Enemies);
    }

    private void FadeOut() {
        FadeSprite[] fadeSprites = GetComponentsInChildren<FadeSprite>();
        foreach (FadeSprite fadeSprite in fadeSprites) {
            fadeSprite.FadeOut(duration: 0.5f);
        }
    }
}
