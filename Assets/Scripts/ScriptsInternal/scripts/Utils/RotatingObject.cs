using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    [SerializeField]
    public float rotationSpeed;

    public Vector3 rotationDirection = new Vector3(0, 0, 1);

    public bool ignoreTimescale;

    private void Update()
    {
        float value = rotationSpeed * (ignoreTimescale ? 0.5f : Time.deltaTime);
        transform.Rotate(rotationDirection * value);
    }
}
