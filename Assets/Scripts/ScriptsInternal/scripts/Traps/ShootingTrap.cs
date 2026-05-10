using System.Collections;
using UnityEngine;

public class ShootingTrap : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToShoot;

    [SerializeField]
    private float speed;

    private bool isRunning;

    [SerializeField]
    private float interval;

    [SerializeField]
    private float initialDelay;

    void Start()
    {
        isRunning = true;
        StartCoroutine(SpawnTrap());
    }


    private IEnumerator SpawnTrap()
    {
        yield return new WaitForSeconds(initialDelay);
        while (isRunning)
        {
            GameObject clone = Instantiate(objectToShoot, transform);
            Rigidbody rigidbody = clone.GetComponent<Rigidbody>();
            rigidbody.AddForce(transform.forward * speed, ForceMode.Force);
            yield return new WaitForSeconds(interval);
        }
    }
}
