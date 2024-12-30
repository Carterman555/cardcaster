using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionManager : Singleton<SceneTransitionManager> {

    [SerializeField] private Animator crossFadeTransition;
    [SerializeField] private float transitionTime = 1f;

    private void Start() {
        //... so fade still works when time.scale = 0
        crossFadeTransition.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public IEnumerator PlayStartTransition() {
        crossFadeTransition.SetTrigger("start");
        yield return new WaitForSecondsRealtime(transitionTime);
    }

    public void PlayEndTransition() {
        crossFadeTransition.SetTrigger("end");
    }
}
