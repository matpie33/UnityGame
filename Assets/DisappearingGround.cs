using System.Collections;
using UnityEngine;

public class DisappearingGround : MonoBehaviour
{
    [SerializeField]
    private float invisibleTime;

    [SerializeField]
    private float visibleTime;

    [SerializeField]
    private float initialDelay;

    private bool isRunning = true;

    private Collider colliderObject;

    private MeshRenderer meshRenderer;

    void Start()
    {
        colliderObject = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
        StartCoroutine(DisappearAndAppear());
    }

    private IEnumerator DisappearAndAppear()
    {
        Debug.Log(isRunning);
        yield return new WaitForSeconds(initialDelay);
        while (isRunning)
        {
            colliderObject.enabled = false;
            meshRenderer.enabled = false;
            yield return new WaitForSeconds(invisibleTime);
            colliderObject.enabled = true;
            meshRenderer.enabled = true;
            yield return new WaitForSeconds(visibleTime);
        }
    }
}
