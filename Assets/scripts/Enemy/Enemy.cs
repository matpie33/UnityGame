using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int attackPower;

    private CharacterController characterController;
    private readonly float minimumDistanceToChase = 10;
    private Animator animator;
    private float minimumDistanceToAttack = 2;
    private bool isAttacking;
    public bool isInRange { get; set; }
    public NavMeshAgent navMeshAgent { get; private set; }

    public int experienceValue { get; private set; }

    private void Start()
    {
        experienceValue = 400;
        attackPower = 10;

        navMeshAgent = GetComponent<NavMeshAgent>();
        characterController = FindObjectOfType<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public int getAttackPower()
    {
        return attackPower;
    }

    public bool GetIsAttacking()
    {
        if (isAttacking)
        {
            isAttacking = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AttackStarts()
    {
        isAttacking = true;
    }

    public void FinishedAttack()
    {
        isAttacking = false;
    }

    private void Update()
    {
        Vector3 playerPosition = characterController.transform.position;
        float distance = Vector3.Distance(navMeshAgent.transform.position, playerPosition);
        if (distance < minimumDistanceToChase)
        {
            navMeshAgent.SetDestination(playerPosition);

            if (distance < minimumDistanceToAttack)
            {
                animator.SetBool(WolfAnimationVariables.IS_ATTACKING, true);
                animator.SetBool(WolfAnimationVariables.IS_CHASING, false);

                gameObject.transform.LookAt(playerPosition, Vector3.up);
            }
            else
            {
                animator.SetBool(WolfAnimationVariables.IS_CHASING, true);
                animator.SetBool(WolfAnimationVariables.IS_ATTACKING, false);
            }
        }
        else
        {
            animator.SetBool(WolfAnimationVariables.IS_CHASING, false);
            navMeshAgent.ResetPath();
        }
    }
}
