using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {

    [SerializeField] private float mapScaleFactor;
    private const float WORLD_TO_MAP_SCALE = 100f;

    [SerializeField] private MapIconContainerResizer mapIconContainerResizer;

    [Header("To Copy From Minimap")]
    [SerializeField] private Transform mapIconContainer;

    [SerializeField] private Transform miniMapIconContainer;
    [SerializeField] private Transform miniMapOutlineIconContainer;

    [SerializeField] private RectTransform playerMiniMapIcon;
    private RectTransform playerMapIcon;

    private List<Image> spawnedImages = new();

    private void OnEnable() {
        spawnedImages.Clear();

        float miniMapToMapScaleFactor = mapScaleFactor / MinimapManager.Instance.MinimapScaleFactor;

        Image[] iconImages = miniMapIconContainer.GetComponentsInChildren<Image>()
            .Where(i => i != miniMapIconContainer.GetComponent<Image>()).ToArray();

        foreach (Image miniMapImage in iconImages) {
            Image mapImage = miniMapImage.Spawn(mapIconContainer);

            RectTransform mapImageTransform = mapImage.GetComponent<RectTransform>();
            RectTransform minimapImageTransform = miniMapImage.GetComponent<RectTransform>();

            mapImageTransform.anchoredPosition = minimapImageTransform.anchoredPosition * miniMapToMapScaleFactor;
            mapImageTransform.sizeDelta = minimapImageTransform.sizeDelta * miniMapToMapScaleFactor;
            mapImage.Fade(miniMapImage.color.a);

            if (MinimapManager.Instance.RoomIconTransforms.ContainsKey(miniMapImage)) {

                if (mapImage.TryGetComponent(out Button _button)) {
                    _button.enabled = Room.GetCurrentRoom().IsRoomCleared;
                }
                else {
                    mapImage.AddComponent<Button>();
                    Button button = mapImage.GetComponent<Button>();

                    ColorBlock colorBlock = new ColorBlock() {
                        normalColor = Color.gray,
                        highlightedColor = new Color(0.75f, 0.75f, 0.75f),
                        pressedColor = new Color(0.75f, 0.75f, 0.75f),
                        selectedColor = Color.gray,
                        disabledColor = Color.gray,
                        colorMultiplier = 2
                    };

                    button.colors = colorBlock;

                    button.enabled = Room.GetCurrentRoom().IsRoomCleared;
                }

                if (mapImage.TryGetComponent(out RoomTeleportButton roomTeleport)) {
                    roomTeleport.SetRoom(MinimapManager.Instance.RoomIconTransforms[miniMapImage]);
                }
                else {
                    RoomTeleportButton roomTeleportButton = mapImage.AddComponent<RoomTeleportButton>();
                    roomTeleportButton.SetRoom(MinimapManager.Instance.RoomIconTransforms[miniMapImage]);
                }

            }

            spawnedImages.Add(mapImage);
        }

        playerMapIcon = playerMiniMapIcon.Spawn(mapIconContainer);
        playerMapIcon.anchoredPosition = WorldToIconPos(PlayerMovement.Instance.CenterPos);
        playerMapIcon.sizeDelta = playerMiniMapIcon.sizeDelta * miniMapToMapScaleFactor;
        playerMapIcon.SetSiblingIndex(playerMapIcon.parent.childCount - 1); // so appears above all other icons

        //... scale needs to be one or it will mess up map icon container resizing
        transform.localScale = Vector3.one;

        mapIconContainerResizer.ResizeAndPosition();
    }

    private void OnDisable() {
        foreach (Image mapImage in spawnedImages) {
            mapImage.gameObject.ReturnToPool();
        }
        playerMapIcon.gameObject.ReturnToPool();
    }

    private Vector2 WorldToIconPos(Vector2 worldPos) {
        return worldPos * mapScaleFactor * WORLD_TO_MAP_SCALE;
    }
}
