using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class UIInputFieldController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    [UsedImplicitly]
    public void OnInputFieldSelect()
    {
        inputField.placeholder.gameObject.SetActive(false);
    }

    [UsedImplicitly]
    public void OnInputFieldDeselect()
    {
        inputField.placeholder.gameObject.SetActive(true);
    }
}