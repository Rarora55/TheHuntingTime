using UnityEngine;

public class Player : MonoBehaviour
{
    #region States
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerInAirState inAirState { get; private set; }
    public PlayerLandedState landedState { get; private set; }

    [SerializeField] private PlayerData playerData;
    #endregion

    #region Components
    public Animator Anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    #endregion

    #region Check Transforms
    [SerializeField] private Transform groundCheck;
    #endregion

    #region Others Variables
    private Vector2 workSpace;
    public Vector2 currentVelocity { get; private set; }
    public int FacingDirection { get; private set; }

    #endregion


    #region Unity Function CallBacks
    private void Awake()
    {
        StateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        moveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        jumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        inAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        landedState = new PlayerLandedState(this, StateMachine, playerData, "land");
    }

    private void Start()
    {

        Anim = GetComponent<Animator>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();
        FacingDirection = 1;
        StateMachine.Initialize(idleState);
    }

    private void Update()
    {
        currentVelocity = RB.linearVelocity;
        StateMachine.currentState.LogicUpdate();

    }

    private void FixedUpdate()
    {
        StateMachine.currentState.PhysicsUpdate();
    }

    #endregion

    #region Set Functions
    public void SetVelocityX(float velocity)
    {
        workSpace.Set(velocity, currentVelocity.y);
        RB.linearVelocity = workSpace;
        currentVelocity = workSpace;
    }

    public void SetVelocityY(float velocity)
    {
        workSpace.Set(currentVelocity.x, velocity);
        RB.linearVelocity = workSpace;
        currentVelocity = workSpace;
    }
    #endregion

    #region Checks
    public void CheckIfShouldFlip(int xInput)
    {
        if (xInput != 0 && xInput != FacingDirection)
        {
            Flip();
        }
    }

    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, playerData.checkIfGrounded, playerData.whatIsGround);
    }
    #endregion

    #region Others Functions
    private void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    #endregion

}
