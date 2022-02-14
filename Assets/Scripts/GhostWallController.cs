using System;
using UnityEngine;

public class GhostWallController : MonoBehaviour
{
    private bool _isHorizontal = false;
    private bool _isValid = false;
    private Material _ghostWallDefaultMaterial;
    [SerializeField] private Material ghostWallInvalidMaterial;

    public bool IsHorizontal => _isHorizontal;

    public bool IsValid => _isValid;


    private void Awake()
    {
        gameObject.SetActive(false);
        _ghostWallDefaultMaterial = GetComponent<Renderer>().material;
    }

    public void Rotate()
    {
        transform.Rotate(0, 90, 0);
        _isHorizontal = !_isHorizontal;
    }

    public void Move(Vector3 position)
    {
        transform.position = new Vector3(position.x, transform.position.y, position.z);
        gameObject.SetActive(true);
    }


    public void SetValid(bool isValid)
    {
        GetComponent<Renderer>().material = isValid ? _ghostWallDefaultMaterial : ghostWallInvalidMaterial;
        _isValid = isValid;
    }
}