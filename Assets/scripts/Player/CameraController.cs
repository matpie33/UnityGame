using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Framing")]
    [SerializeField]
    private Camera _camera = null;

    [SerializeField]
    private Transform _followTransform = null; //

    private Vector3 planarDirection; //cameras forward on xz plane
    private Quaternion _targetRotation;

    [Header("Rotation")]
    [SerializeField]
    [Range(-90, 90)]
    private float _minVerticalAngle = -90;

    [Range(-90, 90)]
    [SerializeField]
    private float _maxVerticalAngle = 90;

    [Header("Distance")]
    [SerializeField]
    private float _zoomSpeed = 10f;

    [SerializeField]
    private float _rotationSharpness = 25;

    [SerializeField]
    private float _minDistance = 0;

    [SerializeField]
    private float _maxDistance = 10f;

    [SerializeField]
    private float _defaultDistance = 2f;

    [Header("Obstructions")]
    [SerializeField]
    private float _checkRadius = 0.2f;

    [SerializeField]
    private LayerMask _obstructionLayers = -1;

    private List<Collider> _ignoreColliders = new List<Collider>();

    private float _targetVerticalAngle;

    private Vector3 _targetPosition;

    private float _targetDistance;

    public Vector3 cameraPlanarDirection
    {
        get => planarDirection;
    }

    [SerializeField]
    private Vector3 _framing;

    [SerializeField]
    private bool _invertX;

    [SerializeField]
    private bool _invertY;

    [SerializeField]
    private float _defaultVerticalAngle = 20f;

    private Vector3 _newPosition;

    private Quaternion _newRotation;

    private void OnValidate()
    {
        _defaultDistance = Mathf.Clamp(_defaultDistance, _minDistance, _maxDistance);
        _defaultVerticalAngle = Mathf.Clamp(
            _defaultVerticalAngle,
            _minVerticalAngle,
            _maxVerticalAngle
        );
    }

    public void adjustCameraForCrouch()
    {
        _framing.x = -0.02f;
        _framing.y = -0.69f;
    }

    public void adjustCameraForStanding()
    {
        _framing.x = 0;
        _framing.y = 0;
    }

    private void Start()
    {
        _framing = new Vector3(0, 0, 0);
        _ignoreColliders.AddRange(GetComponentsInChildren<Collider>());
        planarDirection = _followTransform.forward;

        Cursor.lockState = CursorLockMode.Locked;
        _targetDistance = _defaultDistance;
        _targetVerticalAngle = _defaultVerticalAngle;
        _targetRotation =
            Quaternion.LookRotation(planarDirection) * Quaternion.Euler(_targetVerticalAngle, 0, 0);
        _targetPosition =
            _followTransform.position - (_targetRotation * Vector3.forward) * _targetDistance;
    }

    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        float _zoom = -PlayerInputs.MouseScrollInput * _zoomSpeed; // -0.5 lub 0.5
        float mouseX = PlayerInputs.MouseXInput; //wartosc od okolo -40 do 40 - 40 bardzo szybki ruch, 10 srednio szybki,
        float mouseY = -PlayerInputs.MouseYInput;
        if (_invertX)
        {
            mouseX *= -1;
        }
        if (_invertY)
        {
            mouseY *= -1;
        }

        Vector3 _focusPosition = _followTransform.position + new Vector3(_framing.x, _framing.y, 0);

        planarDirection = Quaternion.Euler(0, mouseX, 0) * planarDirection; //rotate planar direction (mouseX degrees) around y axis
        _targetDistance = Mathf.Clamp(_targetDistance + _zoom, _minDistance, _maxDistance); //zwiekszamy/zmniejszamy dystans o wartosc _zoom
        _targetVerticalAngle = Mathf.Clamp(
            _targetVerticalAngle + mouseY,
            _minVerticalAngle,
            _maxVerticalAngle
        );

        float _smallestDistance = _targetDistance;
        RaycastHit[] _hits = Physics.SphereCastAll(
            _focusPosition,
            _checkRadius,
            _targetRotation * -Vector3.forward,
            _targetDistance,
            _obstructionLayers
        ); //cast ray from player model, ray is a sphere, ray toward camera, sphere is moving "_targetDistance" towards destination

        // and has radius = checkRadius

        if (_hits.Length != 0)
        {
            foreach (RaycastHit hit in _hits)
            {
                if (!_ignoreColliders.Contains(hit.collider))
                {
                    if (hit.distance < _smallestDistance)
                    {
                        _smallestDistance = hit.distance; // find closest distance from player to obstacle and use this distance
                    }
                }
            }
        }

        _targetRotation =
            Quaternion.LookRotation(planarDirection) * Quaternion.Euler(_targetVerticalAngle, 0, 0);
        _targetPosition = _focusPosition - (_targetRotation * Vector3.forward) * _smallestDistance;
        _newRotation = Quaternion.Slerp(
            _camera.transform.rotation,
            _targetRotation,
            Time.deltaTime * _rotationSharpness
        );
        _newPosition = Vector3.Lerp(
            _camera.transform.position,
            _targetPosition,
            Time.deltaTime * _rotationSharpness
        );

        _camera.transform.position = _newPosition;
        _camera.transform.rotation = _newRotation;
    }
}
