using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private const float HorizontalRotateModifier = 50.0f;
    private const float VerticalRotateModifier = 50.0f;
    private const float ZoomModifier = 0.4f;

    private const float VerticalRotationLowerBound = 30.0f;
    private const float VerticalRotationUpperBound = 90.0f;

    private const float DistanceLowerBound = 15.0f;
    private const float DistanceUpperBound = 35.0f;

    private float _horizontalRotateValue = 0.0f;
    private float _verticalRotateValue = 0.0f;
    private float _zoomValue = 0.0f;

    private float _distance;
    private float _rotationX;
    private float _rotationY;


    private void Start()
    {
        _distance = Vector3.Magnitude(transform.position);
        _rotationX = transform.eulerAngles.x;
        _rotationY = transform.eulerAngles.y;
    }

    private void Update()
    {
        Zoom();
        Rotate();
    }
    
    [UsedImplicitly]
    private void OnRotateCameraHorizontally(InputValue value)
    {
        _horizontalRotateValue = value.Get<float>();
    }

    [UsedImplicitly]
    private void OnRotateCameraVertically(InputValue value)
    {
        _verticalRotateValue = value.Get<float>();
    }

    [UsedImplicitly]
    private void OnZoom(InputValue value)
    {
        _zoomValue = value.Get<float>();
    }
    

    private void Zoom()
    {
        Vector3 position = transform.position +
                           Vector3.Normalize(transform.position) * (_zoomValue * ZoomModifier * Time.deltaTime);
        transform.position = ClampVector(position, DistanceLowerBound, DistanceUpperBound);
        _distance = Vector3.Magnitude(transform.position);
    }

    private void Rotate()
    {
        _rotationX += _verticalRotateValue * VerticalRotateModifier * Time.deltaTime;
        _rotationY += -_horizontalRotateValue * HorizontalRotateModifier * Time.deltaTime;
        _rotationX = Mathf.Clamp(_rotationX, VerticalRotationLowerBound, VerticalRotationUpperBound);

        Quaternion rotation = Quaternion.Euler(_rotationX, _rotationY, 0);

        Vector3 negDistance = new Vector3(0, 0, -_distance);
        Vector3 position = rotation * negDistance;

        transform.rotation = rotation;
        transform.position = position;
    }


    private static Vector3 ClampVector(Vector3 vector, float minMagnitude, float maxMagnitude)
    {
        float squareMagnitude = vector.sqrMagnitude;
        if (squareMagnitude < minMagnitude * minMagnitude)
        {
            return Vector3.Normalize(vector) * minMagnitude;
        }

        if (squareMagnitude > maxMagnitude * maxMagnitude)
        {
            return Vector3.Normalize(vector) * maxMagnitude;
        }

        return new Vector3(vector.x, vector.y, vector.z);
    }
}