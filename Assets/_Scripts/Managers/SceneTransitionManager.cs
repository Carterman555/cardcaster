using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionManager : StaticInstance<SceneTransitionManager> {

    [SerializeField] private Animator crossFadeTransition;
    [SerializeField] private float transitionTime = 1f;

    public IEnumerator PlayStartTransition() {
        crossFadeTransition.SetTrigger("start");

        yield return new WaitForSeconds(transitionTime);
    }
}
