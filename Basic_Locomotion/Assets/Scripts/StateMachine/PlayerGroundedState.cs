using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState, IRootState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory) {
        IsRootState = true;
    }
    public override void CheckSwitchStates()
    {
        if (Context.IsJumpPressed)
        {
            SwitchState(Factory.Jump());
        }else if (!Context.CharacterController.isGrounded)
        {
            SwitchState(Factory.Fall());
        }
    }

    public override void EnterState()
    {
        HandleGravity();
        InitializeSubState();
    }

    public override void ExitState()
    {
        
    }

    public void HandleGravity()
    {
        Context.CurrentMovementY = Context.Gravity;
    }

    public override void InitializeSubState()
    {
        if(!Context.IsMovementPressed && !Context.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }else if(Context.IsMovementPressed && !Context.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Run());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
