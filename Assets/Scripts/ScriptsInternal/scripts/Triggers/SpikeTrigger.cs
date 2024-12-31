using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
    private Animator spikeAnimator;
    public GameObject spikeToTrigger;

    void Start()
    {
        spikeAnimator = spikeToTrigger.transform.Find("spike").GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
        {
            spikeAnimator.Play("SpikeUp");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
        {
            Invoke(nameof(SpikeDown), .01f);
        }
    }

    private void SpikeDown()
    {
        spikeAnimator.Play("SpikeDown");
    }
}
