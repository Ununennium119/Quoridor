using JetBrains.Annotations;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [UsedImplicitly]
    public void DeviceLost()
    {
        Debug.Log("Device Lost");
    }
        
    [UsedImplicitly]
    public void DeviceRegained()
    {
        Debug.Log("Device Regained");
    }
        
    [UsedImplicitly]
    public void ControlsChanged()
    {
        Debug.Log("Controls Changed");
    }
}