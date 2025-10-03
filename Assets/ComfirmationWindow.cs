using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmationWindow : MonoBehaviour
{
    public Button confirmButton;
    public Button cancelButton;

    public void Setup(Action onConfirm, Action onCancel)
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(() => onConfirm?.Invoke());

        if (cancelButton != null)
            cancelButton.onClick.AddListener(() => onCancel?.Invoke());
    }
}
