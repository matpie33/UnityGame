using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private HealthState enemyState;

    [SerializeField]
    private int maxHealth;

    [SerializeField]
    private int attackPower;

    private CharacterController characterController;
    private readonly float minimumDistanceToChase = 10;
    private Animator animator;
    private float minimumDistanceToAttack = 2;
    private bool isAttacking;
    private Image healthBar;

    public NavMeshAgent navMeshAgent { get; private set; }

    private void Start()
    {
        attackPower = 100;
        enemyState = new HealthState(maxHealth);
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterController = FindObjectOfType<CharacterController>();
        animator = GetComponent<Animator>();
        GetHealthbarForeground();
        healthBar.fillAmount = 1;
    }

    public int getAttackPower()
    {
        return attackPower;
    }

    private void GetHealthbarForeground()
    {
        Image[] images = GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].name.Equals("Foreground"))
            {
                healthBar = images[i];
                break;
            }
        }
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

    public void DecreaseHealth(int byValue)
    {
        enemyState.DecreaseHealth(byValue);
    }

    public bool IsAlive()
    {
        return enemyState.IsAlive();
    }

    private void Update()
    {
        HealthBarUIUpdater.UpdateUI(healthBar, enemyState);
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
