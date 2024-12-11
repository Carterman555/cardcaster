using AllIn1SpriteShader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMapIcons : MonoBehaviour {

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += AddOutlinesCor;
    }
    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= AddOutlinesCor;
    }

    private void AddOutlinesCor() {
        StartCoroutine(AddOutlines());
    }

    private IEnumerator AddOutlines() {

        //... delay adding the outline to make sure all the rooms have spawned their map icons as children of this object
        yield return null;

        //... this will create new room icon sprites with the outline
        GetComponent<All1CreateUnifiedOutline>().CreateUnifiedOutline();

        //... wait for icon sprites to spawn
        yield return null;

        // change all the newly create icon sprites to room layer so only visible on map
        foreach (Transform child in transform) {
            child.gameObject.layer = GameLayers.MapLayer;
        }
    }
}
