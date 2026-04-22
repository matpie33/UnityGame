using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Material[] materials;

    private float timeElapsed = 0f;

    void Start()
    {
        colliderObjects = new List<Collider>();
        meshRenderers = new List<MeshRenderer>();
        colliderObjects.Add(GetComponent<Collider>());
        colliderObjects.AddRange(GetComponentsInChildren<Collider>());
        meshRenderers.Add(GetComponent<MeshRenderer>()); 
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());
        materials = meshRenderers.Select(meshRenderer => meshRenderer.material).ToArray();
        StartCoroutine(DisappearAndAppear());
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        foreach (Material material in materials)
        {
            if (isDisappearing)
            {
                Color color = material.color;
                
                float alpha = Mathf.Clamp01((visibleTime - timeElapsed) / visibleTime);
                color.a = alpha;
                material.color = color;
            }
        }
        
    }

    private IEnumerator DisappearAndAppear()
    {

        foreach (Material material in materials)
        {
            Color color = material.color;
            color.a = 0;
            material.color = color;
        }
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
