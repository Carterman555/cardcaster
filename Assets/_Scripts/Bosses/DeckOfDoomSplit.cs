using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckOfDoomSplit : MonoBehaviour, IHasStats {

    [SerializeField] private ScriptableBoss scriptableBoss;
    public Stats GetStats() {
        return scriptableBoss.Stats;
    }

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
