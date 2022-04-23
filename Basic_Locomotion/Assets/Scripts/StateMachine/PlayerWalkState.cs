using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    public override void CheckSwitchStates()
    {
        if (!Context.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }else if(Context.IsMovementPressed && Context.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
    }

    public override void EnterState()
    {
        Context.Anim.SetBool(Context.IsWalkingHash, true);
        Context.Anim.SetBool(Context.IsRunningHash, false);
        Context.CurrentSpeed = PlayerStateMachine.WalkSpeed;
    }

    public override void ExitState()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
