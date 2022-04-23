using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    public override void CheckSwitchStates()
    {
        if(Context.IsMovementPressed && Context.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }else if (Context.IsMovementPressed)
        {
            SwitchState(Factory.Walk());
        }
    }

    public override void EnterState()
    {
        Context.Anim.SetBool(Context.IsWalkingHash, false);
        Context.Anim.SetBool(Context.IsRunningHash, false);
        Context.AppliedMovementX = 0; 
        Context.AppliedMovementZ = 0;
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
