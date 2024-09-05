using UnityEngine;

public class AnimationRandomizer : MonoBehaviour {

    [SerializeField] private string clipName;

    // randomize delay on the animation
    void Start() {
        Animator animator = GetComponent<Animator>();
        animator.Play(clipName, 0, Random.value); 
    }
}
