using UnityEngine;

public class GhostWallController : MonoBehaviour
{
    private Material _ghostWallDefaultMaterial;
    [SerializeField] private Material ghostWallInvalidMaterial;

    /// <value>Specifies whether the ghost wall is horizontal or vertical.</value>
    public bool IsHorizontal { get; private set; } = false;

    /// <value>Specifies whether the ghost wall is valid or not.</value>
    public bool IsValid { get; private set; } = false;


    private void Awake()
    {
        gameObject.SetActive(false);
        _ghostWallDefaultMaterial = GetComponent<Renderer>().material;
    }


    /// <summary>
    /// Rotates the ghost wall 90 degrees.
    /// </summary>
    public void Rotate()
    {
        transform.Rotate(0, 90, 0);
        IsHorizontal = !IsHorizontal;
    }

    /// <summary>
    /// Moves the ghost wall to the given <paramref name="position"/>.
    /// </summary>
    /// <param name="position"></param>
    public void Move(Vector3 position)
    {
        transform.position = new Vector3(position.x, transform.position.y, position.z);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Sets the ghost wall validation as <paramref name="isValid"/>.
    /// </summary>
    /// <param name="isValid"></param>
    public void SetValid(bool isValid)
    {
        GetComponent<Renderer>().material = isValid ? _ghostWallDefaultMaterial : ghostWallInvalidMaterial;
        IsValid = isValid;
    }
}