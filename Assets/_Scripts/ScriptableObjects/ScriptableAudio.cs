using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClips", menuName = "AudioClips")]
public class ScriptableAudio : ScriptableObject {

    [Header("Player")]

    [SerializeField] private AudioClips playerStep;
    public AudioClips PlayerStep => playerStep;

    [SerializeField] private AudioClips swing;
    public AudioClips Swing => swing;
}
