using System.Collections;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject cubePrefab;

    [SerializeField]
    private float interval;

    private bool isRunning;

    [SerializeField]
    private Transform initialPosition;

    [SerializeField]
    private float startingForce;

    void Start()
    {
        isRunning = true;
        StartCoroutine(SpawnCubes());
    }

    private void OnDestroy()
    {
        StopCoroutine(nameof(SpawnCubes));
    }

    private IEnumerator SpawnCubes()
    {
        while (isRunning)
        {
            GameObject clone = Instantiate(cubePrefab);
            Rigidbody rb = clone.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * rb.mass * startingForce, ForceMode.Impulse);
            StartCoroutine(DestroyObject(clone));
            cubePrefab.transform.rotation = transform.rotation;
            clone.transform.position = initialPosition.position;
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator DestroyObject(GameObject gameObject)
    {
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }
}
