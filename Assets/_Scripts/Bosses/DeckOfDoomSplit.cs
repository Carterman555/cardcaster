using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckOfDoomSplit : MonoBehaviour, IHasCommonStats {

    [SerializeField] private ScriptableBoss scriptableBoss;
    public CommonStats CommonStats => scriptableBoss.Stats;

    [SerializeField] private Animator anim;

    private BounceMoveBehaviour moveBehaviour;

    private void Awake() {
        moveBehaviour = GetComponent<BounceMoveBehaviour>();
    }

    private void OnEnable() {
        anim.SetBool("deckSplit", true);

        moveBehaviour.enabled = true;
    }
}
