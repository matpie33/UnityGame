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

    private bool isDisappearing = false;

    private List<Collider> colliderObjects;

    private List<MeshRenderer> meshRenderers;

    private Material material;

    private float timeElapsed = 0f;

    void Start()
    {
        colliderObjects = new List<Collider>();
        meshRenderers = new List<MeshRenderer>();
        colliderObjects.Add(GetComponent<Collider>());
        colliderObjects.AddRange(GetComponentsInChildren<Collider>());
        meshRenderers.Add(GetComponent<MeshRenderer>()); 
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());
        material = meshRenderers[0].material;
        StartCoroutine(DisappearAndAppear());
    }

    private void Update()
    {
        Color color = material.color;
        if (isDisappearing)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01((visibleTime - timeElapsed) / visibleTime);
            color.a = alpha;
        }
        material.color = color;
    }

    private IEnumerator DisappearAndAppear()
    {

        Color color = material.color;
        color.a = 0;
        material.color = color;
        yield return new WaitForSeconds(initialDelay);

        while (isRunning)
        {
            colliderObjects.ForEach(collider => collider.enabled = false);
            isDisappearing = false;
            timeElapsed = 0f;
            yield return new WaitForSeconds(invisibleTime);
            colliderObjects.ForEach(collider => collider.enabled = true);
            isDisappearing = true;
            timeElapsed = 0f;
            yield return new WaitForSeconds(visibleTime);
        }
    }
}
