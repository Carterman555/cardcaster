using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptablePermCard : ScriptableCardBase {

    public override void TryPlay(Vector2 position) {
        base.TryPlay(position);
        Play(position);
    }

}
