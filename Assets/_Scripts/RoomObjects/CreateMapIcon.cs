using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMapIcon : MonoBehaviour {

    [SerializeField] private Sprite iconSprite;
    [SerializeField] private Material iconMaterial;
    [SerializeField] private float size = 8f;

    private GameObject mapIcon;

    private void OnEnable() {

        mapIcon = new GameObject().Spawn(transform.position, Containers.Instance.MapIcons);
        mapIcon.name = "ChestIcon";
        mapIcon.layer = GameLayers.MapLayer;
        mapIcon.transform.localScale = Vector3.one * size;

        SpriteRenderer iconSpriteRenderer = mapIcon.AddComponent<SpriteRenderer>();
        iconSpriteRenderer.sprite = iconSprite;
        iconSpriteRenderer.material = iconMaterial;
        iconSpriteRenderer.sortingOrder = 1;
    }

}
