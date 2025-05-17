using UnityEngine;
using UnityEngine.AI;

public class WanderMovementBehavior : MonoBehaviour, IEffectable, IEnemyMovement {

    [SerializeField] private float scrollSpeed = 0.5f;

    //... when destination is near obstacle change direction faster to increase chances of moving away from obstacle
    [SerializeField] private float obstacleScrollSpeedMult = 5f;

    //... for outside scripts to dictate where the unit shouldn't wander
    public bool ForceObstacleScrollSpeed { get; set; }

    private float noiseXOffset;
    private float noiseYOffset;
    private float noisePos;

    [SerializeField] private float maxDestinationDistance = 2f;

    //... don't set for bees because it takes the target destination and uses it to control the whole swarm
    [SerializeField] private bool setAgentDestination = true;
    public Vector2 TargetDestination { get; private set; }

    private NavMeshAgent agent;
    private Knockback knockback;

    private IHasEnemyStats hasStats;

    [SerializeField] private bool showGizmosLine;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        hasStats = GetComponent<IHasEnemyStats>();
        agent.speed = hasStats.EnemyStats.MoveSpeed;

        knockback = GetComponent<Knockback>();
    }

    private void OnEnable() {
        noiseXOffset = Random.Range(-100f, 100f);
        noiseYOffset = Random.Range(-100f, 100f);
        noisePos = Random.Range(-100f, 100f);
    }

    private void Update() {

        if (!IsMoving()) {
            return;
        }
        else if (IsMoving() && agent.isStopped) {
            agent.isStopped = false;
            return;
        }

        // x and y are both between -1 and 1
        float x = Mathf.PerlinNoise(noiseXOffset, noisePos) * 2f - 1f;
        float y = Mathf.PerlinNoise(noiseYOffset, noisePos) * 2f - 1f;

        Vector2 moveDirection = new Vector2(x, y).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, maxDestinationDistance, GameLayers.ObstacleLayerMask);
        if (hit.collider == null) {
            TargetDestination = (Vector2)transform.position + moveDirection * maxDestinationDistance;

            if (ForceObstacleScrollSpeed) {
                noisePos += scrollSpeed * obstacleScrollSpeedMult * Time.deltaTime;
            }
            else {
                noisePos += scrollSpeed * Time.deltaTime;
            }
        }
        else {
            TargetDestination = (Vector2)transform.position + moveDirection * hit.distance;

            //... when destination is near obstacle change direction faster to increase chances of moving away from obstacle
            noisePos += scrollSpeed * obstacleScrollSpeedMult * Time.deltaTime;
        }

        //... don't set for bees because it takes the target destination and uses it to control the whole swarm
        if (setAgentDestination) {
            agent.SetDestination(TargetDestination);
        }
    }

    private void OnDrawGizmos() {

        if (!Application.isPlaying || !showGizmosLine) {
            return;
        }

        Gizmos.color = Color.white;

        // x and y are both between -1 and 1
        float x = Mathf.PerlinNoise(noiseXOffset, noisePos) * 2f - 1f;
        float y = Mathf.PerlinNoise(noiseYOffset, noisePos) * 2f - 1f;

        Vector2 moveDirection = new Vector2(x, y).normalized;

        float minDepth = 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, maxDestinationDistance, GameLayers.ObstacleLayerMask, minDepth);
        if (hit.collider == null) {
            Vector2 destination = (Vector2)transform.position + moveDirection * maxDestinationDistance;
            Gizmos.DrawLine(transform.position, destination);
        }
        else {
            Vector2 destination = (Vector2)transform.position + moveDirection * hit.distance;
            Gizmos.DrawLine(transform.position, destination);
        }
    }

    public void OnAddEffect(UnitEffect unitEffect) {
        if (unitEffect is StopMovement) {
            agent.isStopped = true;
        }
    }

    public bool IsMoving() {
        bool hasStopEffect = TryGetComponent(out StopMovement stopMovement);
        return !hasStopEffect && !knockback.IsApplyingKnockback() && enabled && agent.enabled;
    }
}
