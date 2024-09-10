using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Ghost Card")]
public class ScriptableGhostCard : ScriptableStatsModifierCard {


    public override void Play(Vector2 position) {
        base.Play(position);
        //throw new System.NotImplementedException();
    }


}
