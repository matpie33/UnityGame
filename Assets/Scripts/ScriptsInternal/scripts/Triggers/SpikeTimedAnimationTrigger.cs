using System.Collections;
using UnityEngine;

public class SpikeTimedAnimationTrigger : MonoBehaviour
{
    private Animator spikeAnimator;
    public float timeUp;
    public float timeDown;
    public int initialDelay;

    private bool isRunning = true;

    void Start()
    {
        
        spikeAnimator = GetComponent<Animator>();
    }

    public void SetActive (bool active)
    {
        if (active) {
            StartCoroutine(MoveSpikes());
        }
        else
        {
            GetComponent<Animator>().Play("SpikeUp");
        }
        

    }

    private IEnumerator MoveSpikes()
    {
        yield return new WaitForSeconds(initialDelay);
        spikeAnimator.Play("SpikeUp");
        while (isRunning)
        {
            yield return new WaitForSeconds(timeUp);
            SpikeDown();
            yield return new WaitForSeconds(timeDown);
            spikeAnimator.Play("SpikeUp");
            
            
        }
        
    }


    private void SpikeDown()
    {
        spikeAnimator.Play("SpikeDown");
    }
}
