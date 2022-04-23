using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    #region Components
    CharacterController _characterController;
    Animator _anim;
    PlayerInput _playerInput;
    #endregion

    #region Movement Variables
    Vector2 _currentMovementInput = Vector2.zero;
    Vector3 _currentMovement = Vector3.zero;
    Vector3 _appliedMovement = Vector3.zero;

    float _currentSpeed;
    const float _walkSpeed = 4;
    const float _runSpeed = 8;
    float _rotationFactorPerFrame = 5;

    bool _isMovementPressed;
    bool _isRunPressed = false;
    #endregion

    #region Jump Variables
    [SerializeField] bool _holdJump = false;

    float _gravity = -9.8f;

    [SerializeField] float _maxJumpHeight = 2.0f;
    [SerializeField] float _maxJumpTime = 0.7f;
    float _initialJumpVelocity;

    bool _isJumping;
    bool _isJumpPressed;
    #endregion

    #region Animation Hash
    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpingHash; 
    int _isFallingHash;
    #endregion

    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    public PlayerBaseState CurrentState { get => _currentState; set => _currentState = value; }
    public bool IsJumpPressed { get => _isJumpPressed; set => _isJumpPressed = value; }
    public bool IsJumping { get => _isJumping; set => _isJumping = value; }
    public float CurrentMovementY { get => _currentMovement.y; set => _currentMovement.y = value; }
    public float AppliedMovementY { get => _appliedMovement.y; set => _appliedMovement.y = value; }
    public float AppliedMovementX { get => _appliedMovement.x; set => _appliedMovement.x = value; }
    public float AppliedMovementZ { get => _appliedMovement.z; set => _appliedMovement.z = value; }
    public float InitialJumpVelocity { get => _initialJumpVelocity; set => _initialJumpVelocity = value; }
    public Animator Anim { get => _anim; set => _anim = value; }
    public CharacterController CharacterController { get => _characterController; set => _characterController = value; }
    public float Gravity { get => _gravity; set => _gravity = value; }
    public bool HoldJump { get => _holdJump; set => _holdJump = value; }
    public bool IsMovementPressed { get => _isMovementPressed; set => _isMovementPressed = value; }
    public bool IsRunPressed { get => _isRunPressed; set => _isRunPressed = value; }
    public int IsJumpingHash { get => _isJumpingHash; set => _isJumpingHash = value; }
    public int IsWalkingHash { get => _isWalkingHash; set => _isWalkingHash = value; }
    public int IsRunningHash { get => _isRunningHash; set => _isRunningHash = value; }
    public Vector2 CurrentMovementInput { get => _currentMovementInput; set => _currentMovementInput = value; }
    public float CurrentSpeed { get => _currentSpeed; set => _currentSpeed = value; }

    public static float WalkSpeed => _walkSpeed;

    public static float RunSpeed => _runSpeed;

    public int IsFallingHash { get => _isFallingHash; set => _isFallingHash = value; }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        _playerInput = new PlayerInput();

        SetupInput();
        SetupJumpVariables();
        SetupAnimationHash();

        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState(); 
    }
    void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }
    void SetupAnimationHash()
    {
        _isWalkingHash = Animator.StringToHash("IsWalking");
        _isRunningHash = Animator.StringToHash("IsRunning");
        _isJumpingHash = Animator.StringToHash("IsJumping");
        _isFallingHash = Animator.StringToHash("IsFalling");
    }
    private void Start()
    {
        _characterController.Move(_appliedMovement * Time.deltaTime);
    }
    private void Update()
    {
        HandleRotation();

        _currentState.UpdateStates();

        _appliedMovement.x = _currentMovement.x * _currentSpeed;
        _appliedMovement.z = _currentMovement.z * _currentSpeed;
        _characterController.Move(_appliedMovement * Time.deltaTime);
    }
    void SetupInput()
    {
        _playerInput.CharacterControls.Move.started += OnMovementInput;
        _playerInput.CharacterControls.Move.canceled += OnMovementInput;
        _playerInput.CharacterControls.Move.performed += OnMovementInput;

        _playerInput.CharacterControls.Run.started += OnRunInput;
        _playerInput.CharacterControls.Run.canceled += OnRunInput;

        _playerInput.CharacterControls.Jump.started += OnJumpInput;
        _playerInput.CharacterControls.Jump.canceled += OnJumpInput;
    }
    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
        _currentMovement.x = _currentMovementInput.x; 
        _currentMovement.z = _currentMovementInput.y;
    }
    void OnJumpInput(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }
    void OnRunInput(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }
    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = _currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }
    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }
    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }
}
