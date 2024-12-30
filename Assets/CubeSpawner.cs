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
            clone.transform.position = initialPosition.position;
            yield return new WaitForSeconds(interval);
        }
    }
}
