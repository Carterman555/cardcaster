using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {

    [SerializeField] private float mapScaleFactor;
    private const float WORLD_TO_MAP_SCALE = 100f;

    [SerializeField] private MapIconContainerResizer mapIconContainerResizer;

    [Header("To Copy From Minimap")]
    [SerializeField] private Transform mapIconContainer;
    [SerializeField] private Transform mapOutlineIconContainer;

    [SerializeField] private Transform miniMapIconContainer;
    [SerializeField] private Transform miniMapOutlineIconContainer;

    private List<Image> spawnedImages = new();

    private void OnEnable() {
        StartCoroutine(SetupMap());
    }

    private IEnumerator SetupMap() {
        spawnedImages.Clear();

        float miniMapToMapScaleFactor = mapScaleFactor / MinimapManager.Instance.MinimapScaleFactor;

        Image[] iconImages = miniMapIconContainer.GetComponentsInChildren<Image>()
            .Where(i => i != miniMapIconContainer.GetComponent<Image>()).ToArray();

        foreach (Image miniMapImage in iconImages) {
            Image mapImage = miniMapImage.Spawn(mapOutlineIconContainer);

            RectTransform mapImageTransform = mapImage.GetComponent<RectTransform>();
            RectTransform minimapImageTransform = miniMapImage.GetComponent<RectTransform>();

            mapImageTransform.anchoredPosition = minimapImageTransform.anchoredPosition * miniMapToMapScaleFactor;
            mapImageTransform.sizeDelta = minimapImageTransform.sizeDelta * miniMapToMapScaleFactor;

            spawnedImages.Add(mapImage);

            mapImage.Fade(1f); // debug 
        }

        mapIconContainerResizer.ResizeAndPosition(transform.localScale.x);

        yield return new WaitForSeconds(1f);
    }

    private void OnDisable() {
        foreach (Image mapImage in spawnedImages) {
            mapImage.gameObject.ReturnToPool();
        }
    }

    private Vector2 WorldToIconPos(Vector2 worldPos) {
        return worldPos * mapScaleFactor * WORLD_TO_MAP_SCALE;
    }
}
