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

    [SerializeField] private AudioClips dash;
    public AudioClips Dash => dash;

    [Header("Enemies")]

    [SerializeField] private AudioClips damageEnemy;
    public AudioClips DamageEnemy => damageEnemy;

    [Header("Cards")]

    [SerializeField] private AudioClips drawCard;
    public AudioClips DrawCard => drawCard;

    [SerializeField] private AudioClips hoverCard;
    public AudioClips HoverCard => hoverCard;

    [SerializeField] private AudioClips playCard;
    public AudioClips PlayCard => playCard;

    [SerializeField] private AudioClips cancelCard;
    public AudioClips CancelCard => cancelCard;
}
