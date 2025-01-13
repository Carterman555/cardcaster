using AllIn1SpriteShader;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelMapIcons : StaticInstance<LevelMapIcons> {

    private bool setup;

    private List<SpriteRenderer> iconsToShow = new();

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += SetupMap;

        setup = false;
    }
    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= SetupMap;
    }

    // if room or hallway fade in the icon and the icon outline
    public void ShowMapIcon(SpriteRenderer mapIcon) {

        // if not setup then wait until then icons are setup before showing the map icon
        if (!setup) {
            iconsToShow.Add(mapIcon);
            return;
        }

        mapIcon.gameObject.SetActive(true);
        mapIcon.DOFade(1f, duration: 0.5f);

        string outlineName = mapIcon.name + "Outline";
        foreach (Transform child in transform) {
            if (child.name == outlineName) {
                child.gameObject.SetActive(true);
                child.GetComponent<SpriteRenderer>().DOFade(1f, duration: 0.5f);
            }
        }
    }

    private void SetupMap() {
        StartCoroutine(SetupMapCor());
    }

    private IEnumerator SetupMapCor() {

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

        FadeOutChildren();

        setup = true;

        // show all the icons that were invoked to show (through ShowMapIcon()) before the icons were setup
        foreach (SpriteRenderer icon in iconsToShow) {
            ShowMapIcon(icon);
        }
        iconsToShow.Clear();
    }

    private void FadeOutChildren() {
        foreach (Transform child in transform) {
            child.GetComponent<SpriteRenderer>().Fade(0);
        }

        //... also set all children inactive because for some reason the outlines have a black outline even when
        //... alpha is 0
        transform.SetActiveChildren(false);
    }
}
