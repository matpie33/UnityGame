using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private EnemyState enemyState = new EnemyState();

    private CharacterController characterController;
    private readonly float minimumDistanceToChase = 10;
    private Animator animator;
    private float minimumDistanceToAttack = 2;

    public NavMeshAgent navMeshAgent { get; private set; }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterController = FindObjectOfType<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void DecreaseHealth(int byValue)
    {
        enemyState.DecreaseHealth(byValue);
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
