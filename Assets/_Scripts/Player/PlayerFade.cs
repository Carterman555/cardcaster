using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFade {

    public float DesiredFade { get; private set; }
    public int Strength { get; private set; }

    public void Setup(int strength, float fadeAmount, float transitionDuration = 0f) {
        Strength = strength;
        DesiredFade = fadeAmount;

        //DOTween.To(() => DesiredFade, x => DesiredFade = x, fadeAmount, duration: transitionDuration);
    }

    //public void Remove(float transitionDuration) {
    //    DOTween.To(() => DesiredFade, x => DesiredFade = x, 1f, duration: transitionDuration).OnComplete(() => {
    //        Destroy(this);
    //    });
    //}
}
