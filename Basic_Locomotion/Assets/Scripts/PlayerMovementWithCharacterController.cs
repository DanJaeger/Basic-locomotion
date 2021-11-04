using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementWithCharacterController : MonoBehaviour
{
    #region Components
    CharacterController characterController;
    Animator anim;
    #endregion

    #region Movement Variables
    Vector2 currentMovementInput = Vector2.zero;
    Vector3 currentMovement = Vector3.zero;
    Vector3 appliedMovement = Vector3.zero;

    float currentSpeed;
    const float walkSpeed = 4;
    const float runSpeed = 8;
    float rotationFactorPerFrame = 5;

    bool isMovementPressed;
    bool isWalking = false;
    bool isRunning = false;
    #endregion

    #region Jump Variables
    [SerializeField] bool holdJump = false;

    float gravity = -9.8f;
    float groundedGravity = -0.05f;

    [SerializeField] float maxJumpHeight = 2.0f;
    [SerializeField] float maxJumpTime = 0.7f;
    float initialJumpVelocity;

    bool isJumping;
    bool isJumpPressed;
    #endregion

    #region Animation Hash
    int isWalkingHash;
    int isRunningHash; 
    int isJumpingHash;
    #endregion

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        currentSpeed = walkSpeed;

        SetupAnimationHash();
        SetupJumpVariables();
    }
    void SetupAnimationHash()
    {
        isWalkingHash = Animator.StringToHash("IsWalking");
        isRunningHash = Animator.StringToHash("IsRunning");
        isJumpingHash = Animator.StringToHash("IsJumping");
    }
    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void Update()
    {
        HandleRotation();

        #region Move Character
        currentMovementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isJumpPressed = Input.GetKey(KeyCode.Space);
        currentMovementInput.Normalize();
        currentMovement = new Vector3(currentMovementInput.x, currentMovement.y, currentMovementInput.y);
        if (currentMovement.x != 0 || currentMovement.z != 0)
        {
            isMovementPressed = true;
        }
        else
        {
            isMovementPressed = false;
            isWalking = false;
            isRunning = false;
        }

        if (Input.GetKey(KeyCode.LeftShift) && isMovementPressed) //Is running
        {
            isRunning = true;
            currentSpeed = runSpeed;
            isWalking = false;
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && isMovementPressed)//Is not running
        {
            isRunning = false;
            currentSpeed = walkSpeed;
            isWalking = true;
        }
        appliedMovement.x = currentMovement.x * currentSpeed;
        appliedMovement.z = currentMovement.z * currentSpeed;
        characterController.Move(appliedMovement * Time.deltaTime);
        #endregion

        HandleGravity();
        HandleJump();
        HandleAnimations();
    }
    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void HandleGravity()
    {
        bool isFalling;
        float fallMultiplier = 2.0f;
        if (holdJump)
            isFalling = currentMovement.y <= 0 || !isJumpPressed;
        else
            isFalling = currentMovement.y <= 0;

        if (characterController.isGrounded)
        {
            currentMovement.y = groundedGravity;
        }else if (isFalling)
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * 0.5f, -10.0f);
        }
        else
        {
            //Verlet integration
            //http://lolengine.net/blog/2011/12/14/understanding-motion-in-games
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * Time.deltaTime);
            appliedMovement.y = (previousYVelocity + currentMovement.y) * 0.5f;
        }
    }

    void HandleJump()
    {
        if(!isJumping && characterController.isGrounded && isJumpPressed)
        {
            isJumping = true;
            currentMovement.y = initialJumpVelocity;
            appliedMovement.y = initialJumpVelocity;
        }else if(isJumping && characterController.isGrounded && !isJumpPressed)
        {
            isJumping = false;
        }
    }

    void HandleAnimations()
    {
        anim.SetBool(isWalkingHash, isWalking);
        anim.SetBool(isRunningHash, isRunning);
        anim.SetBool(isJumpingHash, isJumping);
    }

    /*bool IsGrounded() In case you need it
    {
        Vector3 capsulebottom = new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z);
        bool grounded = Physics.CheckCapsule(col.bounds.center, capsulebottom, distanceToGround, groundLayer,
            QueryTriggerInteraction.Ignore);
        return grounded;
    }*/

}
