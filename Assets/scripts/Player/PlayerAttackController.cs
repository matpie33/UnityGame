using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private const float COOLDOWN_AFTER_COMBO_COMPLETED = 1f;

    [SerializeField]
    private List<AttackAnimation> attacksList;
    private int comboCounter;
    private Animator animator;
    private bool comboCompleted = false;
    private CharacterController characterController;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        ExitAttack();
        Attack();
    }

    void Attack()
    {
        if (comboCompleted)
        {
            return;
        }
        if (UnityEngine.Input.GetKeyDown(attacksList[comboCounter].key))
        {
            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (comboCounter == 0 || (animatorStateInfo.normalizedTime > 0.6f))
            {
                CancelInvoke(nameof(EndCombo));
                animator.runtimeAnimatorController = attacksList[comboCounter].animatorOverride;
                animator.speed = 0.9f;
                animator.Play("Attack", 0, 0);
                characterController.stateMachine.ChangeState(
                    characterController.stateMachine.doingAnimationState
                );

                comboCounter++;
                if (comboCounter >= attacksList.Count)
                {
                    comboCounter = 0;
                    Invoke(nameof(ResetCombo), COOLDOWN_AFTER_COMBO_COMPLETED);
                    comboCompleted = true;
                }
            }
        }
    }

    private void ResetCombo()
    {
        comboCompleted = false;
    }

    void ExitAttack()
    {
        if (
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f
            && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
        )
        {
            Invoke(nameof(EndCombo), 1f);
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
    }
}
