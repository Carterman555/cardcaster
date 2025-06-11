using UnityEngine;

public class DeckOfDoomSplit : MonoBehaviour, IHasEnemyStats {

    [SerializeField] private ScriptableBoss scriptableBoss;
    public EnemyStats GetEnemyStats() {
        return scriptableBoss.Stats;
    }

    [SerializeField] private Animator anim;

    private BounceMoveBehaviour moveBehaviour;

    private void Awake() {
        moveBehaviour = GetComponent<BounceMoveBehaviour>();
    }

    private void OnEnable() {
        anim.SetBool("deckSplit", true);

        moveBehaviour.enabled = true;
    }
}
