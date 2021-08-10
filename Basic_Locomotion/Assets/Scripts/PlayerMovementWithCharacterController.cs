using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementWithCharacterController : MonoBehaviour
{
    CharacterController characterController;
    Animator anim;

    //Character movement variables
    Vector2 currentMovementInput = Vector2.zero;
    Vector3 currentMovement = Vector3.zero;
    float currentSpeed;
    const float walkSpeed = 4;
    const float runSpeed = 8;
    float rotationFactorPerFrame = 5;
    bool isMovementPressed;
    bool isWalking = false;
    bool isRunning = false;

    //Character jump variables
    [SerializeField] bool holdJump = false;
    float gravity = -9.8f;
    float groundedGravity = -0.05f;
    const float maxJumpHeight = 5.0f;
    const float maxJumpTime = 0.75f;
    float initialJumpVelocity;
    bool isJumping;
    bool isJumpPressed;

    int isWalkingHash;
    int isRunningHash; 
    int isJumpingHash;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        SetupJumpVariables();
    }

    void Start()
    {
        currentSpeed = walkSpeed;

        isWalkingHash = Animator.StringToHash("IsWalking");
        isRunningHash = Animator.StringToHash("IsRunning");
        isJumpingHash = Animator.StringToHash("IsJumping");
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
        else //Is not running
        {
            isRunning = false;
            currentSpeed = walkSpeed;
        }
        //Is Walking
        if (!isRunning && isMovementPressed)
        {
            isWalking = true;
            currentSpeed = walkSpeed;
        }
        characterController.Move(new Vector3(currentMovement.x * currentSpeed, currentMovement.y, currentMovement.z * currentSpeed) * Time.deltaTime);
        #endregion

        HandleGravity();
        HandleJump();
        HandleAnimations();
    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
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
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;
            currentMovement.y = nextYVelocity;
        }
        else
        {
            //Verlet integration
            //http://lolengine.net/blog/2011/12/14/understanding-motion-in-games
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;
            currentMovement.y = nextYVelocity;
        }
    }

    void HandleJump()
    {
        if(!isJumping && characterController.isGrounded && isJumpPressed)
        {
            isJumping = true;
            currentMovement.y = initialJumpVelocity * 0.5f;
        }else if(isJumping && characterController.isGrounded && !isJumpPressed)
        {
            isJumping = false;
        }
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
