using UnityEditor;
using UnityEngine;

public class SprintState : MovementState
{
    private float sprintSpeed = 8f;

    public SprintState(CharacterController characterController, StateMachine stateMachine)
        : base(characterController, stateMachine) { }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (UnityEngine.Input.GetKeyUp(KeyCode.LeftShift))
        {
            stateMachine.ChangeState(stateMachine.runState);
        }
    }

    public override float getTargetSpeed()
    {
        return sprintSpeed;
    }
}
