using System;
using UnityEngine;

public class ConstructEffect : MonoBehaviour {

    public event Action OnConstructed;

    public bool Constructing { get; private set; }

    [SerializeField] private Material constructionMat;
    [SerializeField] private float constructionTime;
    private float progress;

    private void OnEnable() {
        Constructing = true;
        progress = 0;
    }

    private void Update() {
        if (Constructing) {
            progress += (Time.deltaTime / constructionTime);
            if (progress > 1f) {
                Constructing = false;
                OnConstructed?.Invoke();
            }

            constructionMat.SetFloat("_Progress", progress);
        }
    }
}
