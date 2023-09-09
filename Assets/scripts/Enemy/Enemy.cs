using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private HealthState healthState;

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

    [SerializeField]
    private TextMeshProUGUI healthText;

    public NavMeshAgent navMeshAgent { get; private set; }

    private UIUpdater uiUpdater;

    private void Start()
    {
        uiUpdater = FindObjectOfType<UIUpdater>();
        attackPower = 20;
        healthState = new HealthState(maxHealth);
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
        healthState.DecreaseHealth(byValue);
    }

    public bool IsAlive()
    {
        return healthState.IsAlive();
    }

    private void Update()
    {
        uiUpdater.UpdateHealthBar(healthState, healthText, healthBar);
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
