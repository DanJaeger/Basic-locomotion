using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState, IRootState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
       : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    public override void CheckSwitchStates()
    {
        if (Context.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public override void EnterState()
    {
        Context.Anim.SetBool(Context.IsFallingHash, true);
        InitializeSubState();
    }

    public override void ExitState()
    {
        Context.Anim.SetBool(Context.IsFallingHash, false);
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public void HandleGravity()
    {
        float previousYVelocity = Context.CurrentMovementY;
        Context.CurrentMovementY = Context.CurrentMovementY + Context.Gravity * Time.deltaTime;
        Context.AppliedMovementY = Mathf.Max((previousYVelocity + Context.CurrentMovementY) * 0.5f, -20.0f);
    }
}
