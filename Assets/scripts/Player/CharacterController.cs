using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    public PlayerAnimationsManager animationsManager { get; private set; }
    public CapsuleCollider capsuleCollider { get; private set; }
    public CameraController cameraController { get; private set; }
    public PlayerInputs playerInputs { get; private set; }

    public float initialHeight { get; private set; }

    public Rigidbody rigidbody { get; private set; }

    private List<Observer> observers = new List<Observer>();

    public Vector3 currentVelocity { get; set; }

    private HealthState healthState;

    [SerializeField]
    private Image healthBarForeground;

    private StateMachine stateMachine;

    public Animator animator;

    public RunState runState { get; private set; }
    public JumpState jumpState { get; private set; }
    public SprintState sprintState { get; private set; }
    public CrouchState crouchState { get; private set; }
    public LedgeGrabState ledgeGrabState { get; private set; }
    public AttackState attackState { get; private set; }
    public FallingState fallingState { get; private set; }

    private void Start()
    {
        cameraController = GetComponent<CameraController>();
        playerInputs = GetComponent<PlayerInputs>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        animationsManager = GetComponent<PlayerAnimationsManager>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        initialHeight = capsuleCollider.height;
        healthState = new HealthState();

        stateMachine = new StateMachine();
        runState = new RunState(this, stateMachine);
        ledgeGrabState = new LedgeGrabState(this, stateMachine);
        sprintState = new SprintState(this, stateMachine);
        crouchState = new CrouchState(this, stateMachine);
        jumpState = new JumpState(this, stateMachine);
        attackState = new AttackState(this);
        fallingState = new FallingState(this, stateMachine);
        stateMachine.Initialize(runState);

        healthBarForeground.fillAmount = 1;
    }

    public void DecreaseHealth(int percent)
    {
        healthState.DecreaseHealth(percent);
    }

    public void AddObserver(Observer observer)
    {
        observers.Add(observer);
    }

    public void ClimbingFinished()
    {
        stateMachine.currentState.OnTrigger(TriggerType.CLIMBING_FINISHED);
    }

    private void FixedUpdate()
    {
        if (rigidbody.velocity.y < -0.5)
        {
            stateMachine.ChangeState(fallingState);
        }
        stateMachine.currentState.PhysicsUpdate();
    }

    private void Update()
    {
        stateMachine.currentState.FrameUpdate();
        HealthBarUIUpdater.UpdateUI(healthBarForeground, healthState);
    }

    public void attackAnimationFinish()
    {
        notifyObservers(false);
        stateMachine.ChangeState(runState);
    }

    public void attackAnimationStart()
    {
        notifyObservers(true);
    }

    private void notifyObservers(bool isAttacking)
    {
        EventDTO eventDTO = EventDTO.createEventPlayerAttack(isAttacking);
        foreach (Observer observer in observers)
        {
            observer.Notify(eventDTO);
        }
    }

    public void GroundDetected()
    {
        stateMachine.currentState.OnTrigger(TriggerType.GROUND_DETECTED);
    }

    public void changeHeight(bool toStanding)
    {
        if (toStanding)
        {
            capsuleCollider.height = initialHeight;
            capsuleCollider.center = new Vector3(0, 0.9f, 0);
        }
        else
        {
            capsuleCollider.height = initialHeight / 2;
            capsuleCollider.center = new Vector3(0, 0.47f, 0);
        }
    }

    public void grabLedge()
    {
        stateMachine.currentState.OnTrigger(TriggerType.LEDGE_DETECTED);
    }

    internal void switchToIdleAnimation()
    {
        stateMachine.ChangeState(runState);
    }
}
