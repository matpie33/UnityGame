using System.Collections;
using System.Collections.Generic;
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

    private List<Collider> colliderObjects;

    private List<MeshRenderer> meshRenderers;

    void Start()
    {
        colliderObjects = new List<Collider>();
        meshRenderers = new List<MeshRenderer>();
        colliderObjects.Add(GetComponent<Collider>());
        colliderObjects.AddRange(GetComponentsInChildren<Collider>());
        meshRenderers.Add(GetComponent<MeshRenderer>()); 
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());
        StartCoroutine(DisappearAndAppear());
    }

    private IEnumerator DisappearAndAppear()
    {
        yield return new WaitForSeconds(initialDelay);
        while (isRunning)
        {
            colliderObjects.ForEach(collider => collider.enabled = false);
            meshRenderers.ForEach(renderer => renderer.enabled = false);
            yield return new WaitForSeconds(invisibleTime);
            colliderObjects.ForEach(collider => collider.enabled = true);
            meshRenderers.ForEach(renderer => renderer.enabled = true);
            yield return new WaitForSeconds(visibleTime);
        }
    }
}
