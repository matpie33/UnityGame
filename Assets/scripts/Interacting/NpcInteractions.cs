using System.Collections;
using UnityEngine;

public class NpcInteractions : Interactable
{
    [SerializeField]
    private AudioSource helloMessage;

    [SerializeField]
    private AudioSource letsMoveMessage;

    [SerializeField]
    private AudioSource underAttackMessage;

    [SerializeField]
    private AudioSource weReSafe;

    [SerializeField]
    private GameObject wolves;

    private EventQueue eventQueue;

    private void Start()
    {
        eventQueue = FindObjectOfType<EventQueue>();
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.NPC_MOVE:
                letsMoveMessage.Play();
                break;
            case EventType.NPC_ATTACKED:
                underAttackMessage.Play();
                break;
            case EventType.OBJECT_DESTROYED:
                GameObject gameObject = (GameObject)eventDTO.eventData;
                if (gameObject == wolves)
                {
                    weReSafe.Play();
                }
                break;
        }
    }

    public override void Interact(Object data)
    {
        helloMessage.Play();
        float clipLength = helloMessage.clip.length;
        Invoke("SendEvent", clipLength);
        Npc npc = GetComponent<Npc>();
        npc.SetLookAtTarget((GameObject)data);
        enabled = false;
    }

    private void SendEvent()
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_CONFIRMATION_NEEDED, null));
        Cursor.lockState = CursorLockMode.None;
    }
}
