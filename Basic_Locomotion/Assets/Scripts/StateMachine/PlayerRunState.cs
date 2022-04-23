using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    public override void CheckSwitchStates()
    {
        if (!Context.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }else if(Context.IsMovementPressed && !Context.IsRunPressed)
        {
            SwitchState(Factory.Walk());
        }
    }

    public override void EnterState()
    {
        Context.Anim.SetBool(Context.IsWalkingHash, false);
        Context.Anim.SetBool(Context.IsRunningHash, true);
        Context.CurrentSpeed = PlayerStateMachine.RunSpeed;
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
