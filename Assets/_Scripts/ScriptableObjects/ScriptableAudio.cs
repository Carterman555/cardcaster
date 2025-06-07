using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AudioClips", menuName = "AudioClips")]
public class ScriptableAudio : ScriptableObject {

    [Header("Music")]

    [SerializeField] private MusicClips[] music;
    public MusicClips[] Music => music;

    [Header("Player")]

    [SerializeField] private AudioClips swing;
    public AudioClips Swing => swing;

    [SerializeField] private AudioClips dash;
    public AudioClips Dash => dash;

    [SerializeField] private AudioClips collectEssence;
    public AudioClips CollectEssence => collectEssence;

    [SerializeField] private AudioClips playerDie;
    public AudioClips PlayerDie => playerDie;

    [SerializeField] private AudioClips playerHeal;
    public AudioClips PlayerHeal => playerHeal;

    [Header("Abilities")]

    [SerializeField] private AudioClips spinningFury;
    public AudioClips SpinningFury => spinningFury;

    [Header("Enemies")]

    [SerializeField] private AudioClips basicEnemyShoot;
    public AudioClips BasicEnemyShoot => basicEnemyShoot;

    [SerializeField] private AudioClips softEnemyShoot;
    public AudioClips SoftEnemyShoot => softEnemyShoot;

    [SerializeField] private AudioClips spawnEnemy;
    public AudioClips SpawnEnemy => spawnEnemy;

    [SerializeField] private AudioClips merge;
    public AudioClips Merge => merge;

    [Header("Bosses")]

    [SerializeField] private AudioClips deckOfDoomSplit;
    public AudioClips DeckOfDoomSplit => deckOfDoomSplit;

    [SerializeField] private AudioClips deckOfDoomSurge;
    public AudioClips DeckOfDoomSurge => deckOfDoomSurge;

    [SerializeField] private AudioClips drChonkEat;
    public AudioClips DrChonkEat => drChonkEat;

    [SerializeField] private AudioClips drChonkSmash;
    public AudioClips DrChonkSmash => drChonkSmash;

    [SerializeField] private AudioClips drChonkHeal;
    public AudioClips DrChonkHeal => drChonkHeal;

    [SerializeField] private AudioClips thunderGolemCharge;
    public AudioClips ThunderGolemCharge => thunderGolemCharge;

    [SerializeField] private AudioClips thunderGolemArea;
    public AudioClips ThunderGolemArea => thunderGolemArea;

    [Header("The Dealer")]
    [SerializeField] private AudioClips dealerSwordSpin;
    public AudioClips DealerSwordSpin => dealerSwordSpin;

    [SerializeField] private AudioClips laserShoot;
    public AudioClips LaserShoot => laserShoot;

    [SerializeField] private AudioClips spawningSmashers;
    public AudioClips SpawningSmashers => spawningSmashers;

    [SerializeField] private AudioClips spawnSword;
    public AudioClips SpawnSword => spawnSword;

    [SerializeField] private AudioClips spawningBouncers;
    public AudioClips SpawningBouncers => spawningBouncers;

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

    [SerializeField] private AudioClips upgradePersisent;
    public AudioClips UpgradePersisent => upgradePersisent;

    [SerializeField] private AudioClips maxPersisent;
    public AudioClips MaxPersisent => maxPersisent;

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

    [Header("UI")]

    [SerializeField] private AudioClips openPanel;
    public AudioClips OpenPanel => openPanel;

    [SerializeField] private AudioClips closePanel;
    public AudioClips ClosePanel => closePanel;

    [SerializeField] private AudioClips buttonClick;
    public AudioClips ButtonClick => buttonClick;

    [SerializeField] private AudioClips buttonEnter;
    public AudioClips ButtonEnter => buttonEnter;

    [SerializeField] private AudioClips buttonExit;
    public AudioClips ButtonExit => buttonExit;

    [SerializeField] private AudioClips unlockCard;
    public AudioClips UnlockCard => unlockCard;

    [Header("Misc")]

    [SerializeField] private AudioClips damaged;
    public AudioClips Damaged => damaged;

    [SerializeField] private AudioClips teleport;
    public AudioClips Teleport => teleport;

    [SerializeField] private AudioClips explode;
    public AudioClips Explode => explode;
}
