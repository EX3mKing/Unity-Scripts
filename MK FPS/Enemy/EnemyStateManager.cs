using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EnemyWakeUpType
{
    Passive, Threatening, NotAwakeable
}

public enum EnemyFollowType
{
    Endless, Zone
}

public class EnemyStateManager : MonoBehaviour
{
    public EnemyBaseState currentState;
    public EnemyPassiveState PassiveState = new EnemyPassiveState();
    public EnemyAggroState AggroState = new EnemyAggroState();
    public EnemyReturnState ReturnState = new EnemyReturnState();
    
    [HideInInspector] public NavMeshAgent navigation;
    [HideInInspector] public PlayerStateManager player;
    
    public EnemyWakeUpType enemyWakeUpType;
    public EnemyFollowType enemyFollowType;
    public ColliderTriggerEvent StartAggroAreaEvent;
    public ColliderTriggerEvent StopAggroAreaEvent;
    public bool playerInStartAggroArea;
    public bool playerInStopAggroArea;

    [Tooltip("Enemy field of view")]
    [Range(0f, 180f)]
    public float FOV;
    public bool playerSeen;
    public float timeBetweenSeeUpdates;
    public Transform head;

    public Animator animator;
    public float animationRunBlendSpeed = 1f;

    [HideInInspector] public Vector3 startingPosition;
    [HideInInspector] public float animationRunBlendCur;
    [HideInInspector] public int animIDSpeed = Animator.StringToHash("Speed");
    [HideInInspector] public int animIDGrounded = Animator.StringToHash("Grounded");
    [HideInInspector] public int animIDJump = Animator.StringToHash("Jump");
    [HideInInspector] public int animIDFreeFall = Animator.StringToHash("FreeFall");
    [HideInInspector] public int animIDMotionSpeed = Animator.StringToHash("MotionSpeed");


    private void Awake()
    {
        navigation = GetComponent<NavMeshAgent>();
        startingPosition = transform.position;
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerStateManager>();
        currentState = PassiveState;
        currentState.EnterState(this);
        StartAggroAreaEvent.entered.AddListener(PlayerEnteredStartAggroArea);
        StartAggroAreaEvent.exited.AddListener(PlayerExitedStartAggroArea);
        StopAggroAreaEvent.entered.AddListener(PlayerEnteredStopAggroArea);
        StopAggroAreaEvent.exited.AddListener(PlayerExitedStopAggroArea);
        StartCoroutine(PlayerSeenCheck());
    }

    private void Update()
    {
        currentState.UpdateState();
        animationRunBlendCur = Mathf.Lerp(animationRunBlendCur, navigation.velocity.magnitude * navigation.velocity.magnitude,
            Time.deltaTime * animationRunBlendSpeed);
        animator.SetFloat(animIDSpeed, animationRunBlendCur);
        animator.SetFloat(animIDMotionSpeed, navigation.velocity.normalized.magnitude);
    }

    public void SwitchState(EnemyBaseState state)
    {
        currentState.ExitState();
        currentState = state;
        currentState.EnterState(this);
    }

    private void PlayerEnteredStopAggroArea()
    {
        playerInStopAggroArea = true;
    }

    private void PlayerExitedStopAggroArea()
    {
        playerInStopAggroArea = false;
    }

    private void PlayerEnteredStartAggroArea()
    {
        playerInStartAggroArea = true;
    }

    private void PlayerExitedStartAggroArea()
    {
        playerInStartAggroArea = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(head.position, Quaternion.AngleAxis(-FOV, Vector3.up) * head.forward * 2f + head.position);
        Gizmos.DrawLine(head.position, Quaternion.AngleAxis(FOV, Vector3.up) * head.forward * 2f + head.position);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(head.position, head.forward + head.position);
    }

    private IEnumerator PlayerSeenCheck()
    {
        while (true)
        {
            playerSeen = (Vector3.Dot(head.forward, (head.position - player.transform.position).normalized) + 1f) / 2f <= FOV / 180f;
            yield return new WaitForSeconds(timeBetweenSeeUpdates);
        }
    }
}
