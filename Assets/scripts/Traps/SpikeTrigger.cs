using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
    private Animator spikeAnimator;
    public GameObject spikeToTrigger;

    void Start()
    {
        spikeAnimator = spikeToTrigger.transform.Find("spike").GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(Tags.PLAYER))
        {
            spikeAnimator.Play("SpikeUp");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(Tags.PLAYER))
        {
            spikeAnimator.Play("SpikeDown");
        }
    }
}
