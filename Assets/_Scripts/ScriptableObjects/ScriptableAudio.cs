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

    [SerializeField] private AudioClips burnCard;
    public AudioClips BurnCard => burnCard;

    [Header("Environment")]

    [SerializeField] private AudioClips openDoor;
    public AudioClips OpenDoor => openDoor;

    [SerializeField] private AudioClips closeDoor;
    public AudioClips CloseDoor => closeDoor;

    [SerializeField] private AudioClips openDoorBlocker;
    public AudioClips OpenDoorBlocker => openDoorBlocker;

    [SerializeField] private AudioClips closeDoorBlocker;
    public AudioClips CloseDoorBlocker => closeDoorBlocker;

    [SerializeField] private AudioClips openChest;
    public AudioClips OpenChest => openChest;

    [SerializeField] private AudioClips gainChestCard;
    public AudioClips GainChestCard => gainChestCard;

    [SerializeField] private AudioClips breakBarrel;
    public AudioClips BreakBarrel => breakBarrel;

    [Header("UI")]

    [SerializeField] private AudioClips openPanel;
    public AudioClips OpenPanel => openPanel;

    [SerializeField] private AudioClips closePanel;
    public AudioClips ClosePanel => closePanel;

    [SerializeField] private AudioClips buttonClick;
    public AudioClips ButtonClick => buttonClick;
}
