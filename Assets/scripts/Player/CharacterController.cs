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

    [SerializeField]
    private GameObject rightHandObject;

    private PickupObjectsController pickupObjectsController;

    private GameObject pickedObject;

    private PlayerState playerState;

    private UIUpdater uiUpdater;

    private void Awake()
    {
        cameraController = GetComponent<CameraController>();
        playerInputs = GetComponent<PlayerInputs>();
        rigidbody = GetComponent<Rigidbody>();
        animationsManager = GetComponent<PlayerAnimationsManager>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        pickupObjectsController = GetComponent<PickupObjectsController>();
    }

    private void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        initialHeight = capsuleCollider.height;
        healthState = new HealthState();

        healthBarForeground.fillAmount = 1;
        playerState = new PlayerState();
        uiUpdater = GetComponent<UIUpdater>();
    }

    public void AttachObjectToHand()
    {
        pickedObject = pickupObjectsController.objectInFront;
        if (pickedObject.GetComponent<Medkit>() != null)
        {
            playerState.increaseMedipacksAmount();
            uiUpdater.UpdateMedipackAmount(playerState.numberOfMedipacks);
        }
        pickedObject.GetComponent<BoxCollider>().enabled = false;
        pickedObject.transform.SetParent(rightHandObject.transform);
        pickedObject.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void DestroyPickedObject()
    {
        Destroy(pickedObject);
        pickedObject = null;
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
        stateMachine.OnTriggerType(TriggerType.ANIMATION_FINISHED);
    }

    public void PickupObjectsKeyPressed()
    {
        stateMachine.OnTriggerType(TriggerType.PICKUP_STARTED);
    }

    private void Update()
    {
        HealthBarUIUpdater.UpdateUI(healthBarForeground, healthState);
    }

    public void attackAnimationFinish()
    {
        notifyObservers(false);
        stateMachine.OnTriggerType(TriggerType.ANIMATION_FINISHED);
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
        stateMachine.OnTriggerType(TriggerType.GROUND_DETECTED);
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

    public void ledgeDetected()
    {
        stateMachine.OnTriggerType(TriggerType.LEDGE_DETECTED);
    }

    internal void pickupAnimationFinished()
    {
        stateMachine.OnTriggerType(TriggerType.ANIMATION_FINISHED);
    }
}
