using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {

    [SerializeField] private float mapScaleFactor;
    private const float WORLD_TO_MAP_SCALE = 100f;

    [Header("To Copy From Minimap")]
    [SerializeField] private Transform mapIconContainer;
    [SerializeField] private Transform mapOutlineIconContainer;

    [SerializeField] private Transform miniMapIconContainer;
    [SerializeField] private Transform miniMapOutlineIconContainer;

    private void OnEnable() {
        
        foreach (Transform miniMapIcon in miniMapIconContainer) {
            if (miniMapIcon.TryGetComponent(out Image miniMapImage)) {
                Image mapImage = miniMapImage.Spawn(mapIconContainer);
                mapImage.GetComponent<RectTransform>().anchoredPosition = miniMapImage.GetComponent<RectTransform>().anchoredPosition;
            }
        }

        foreach (Transform miniMapOutlineIcon in miniMapOutlineIconContainer) {
            if (miniMapOutlineIcon.TryGetComponent(out Image miniMapImage)) {
                Image mapImage = miniMapImage.Spawn(mapOutlineIconContainer);
                mapImage.GetComponent<RectTransform>().anchoredPosition = miniMapImage.GetComponent<RectTransform>().anchoredPosition;
            }
        }

    }

    private Vector2 WorldToIconPos(Vector2 worldPos) {
        return worldPos * mapScaleFactor * WORLD_TO_MAP_SCALE;
    }
}
