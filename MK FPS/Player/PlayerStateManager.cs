using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController), typeof(PlayerRigManager))]
public class PlayerStateManager : MonoBehaviour
{
    public PlayerStateBase currentState;
    [HideInInspector] public PlayerMovingState MovingState = new PlayerMovingState();
    [HideInInspector] public PlayerClimbingState ClimbingState = new PlayerClimbingState();
    [HideInInspector] public PlayerSlidingState SlidingState = new PlayerSlidingState();

    [Header("Health")]
    public float healthMax;
    public float healthCurrent;
    public float healthRegenRate;


    [Header("Movement")]
    public float speed = 1f;
    public float crouchMult = 0.25f;
    public float sprintMult = 2.0f;
    [Tooltip("Speed used for falling down slopes")]
    public float slideSpeed = 1f;
    public float timeToGetUpAfterSliding = 0.1f;

    [Header("Jump")]
    [Tooltip("Gravity should be negative (- = go down)")]
    public float gravity = -10f;
    [Tooltip("To what height in meters can player jump")]
    public float jumpHeight = 1f;
    public float terminalVelocity = 45.0f;
    [HideInInspector] 
    public float verticalSpeed = 0f;

    [Header("Camera")]
    public float sensitivity = 1f;
    [Tooltip("How far can player look up (degrees)")]
    public float upClamp = 90;
    [Tooltip("How far can player look down (degrees)")]
    public float downClamp = -90;
    [HideInInspector]
    public float cameraTargetPitch;
    public Camera targetCamera;

    [Header("Stamina")]
    [Tooltip("Maximum stamina the player has")]
    public float staminaMaxAmount = 100f;
    [Tooltip("Also used for starting stamina amount")]
    public float staminaCurrentAmount = 100f;
    public float staminaSprintDepletionRate = 10f;
    public float staminaNormalRecoveryRate = 5f;
    public float staminaMinAmountToStartSprint = 15f;
    public float staminaJumpCost = 30f;
    public UnityEvent UseStamina;

    [Header("Climbing")]
    public bool climb;
    public float climbNormalSpeed;
    public float climbSprintSpeedMult;
    public float staminaFastClimbDepletionRate;
    public float firemanLadderSlideSpeedMult;

    [Header("Animation")]
    public Animator animator;
    public float animationRunBlendSpeed = 1f;
    public float climbHandInterpolationSpeed;

    public float timeBetweenHandPlacementUpdates = 0.1f;
    public Transform climbLHandIK;
    public Transform climbRHandIK;
    public Transform climbLHandTarget;
    public Transform climbRHandTarget;

    [HideInInspector] public float animationRunBlendCur;
    [HideInInspector] public int animIDSpeed = Animator.StringToHash("Speed");
    [HideInInspector] public int animIDGrounded = Animator.StringToHash("Grounded");
    [HideInInspector] public int animIDJump = Animator.StringToHash("Jump");
    [HideInInspector] public int animIDFreeFall = Animator.StringToHash("FreeFall");
    [HideInInspector] public int animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    

    [Header("Extras")]
    public LayerMask slopeCheckLayerMask;
    public LayerMask interactableLayerMask;
    
    [HideInInspector]
    public InputManager input;
    [HideInInspector]
    public CharacterController cc;
    [HideInInspector]
    public InteractionManager interaction;
    [HideInInspector]
    public PlayerRigManager rm;

    public const string KeyboardAndMouseControlScheme = "Keyboard&Mouse";
    public const string GamepadControlScheme = "Gamepad";

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        rm = GetComponent<PlayerRigManager>();
    }

    private void Start()
    {
        try
        {
            input = GameObject.FindAnyObjectByType<InputManager>().GetComponent<InputManager>();
        }
        catch
        {
            Debug.LogError("Input Manager not found");
        }
        interaction = FindFirstObjectByType<InteractionManager>();
        currentState = MovingState;
        MovingState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(PlayerStateBase state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }
}
