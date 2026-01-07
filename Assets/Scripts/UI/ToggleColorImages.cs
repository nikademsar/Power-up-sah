using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleColorImages : MonoBehaviour
{
    [SerializeField] private Image whiteImage;
    [SerializeField] private Image blackImage;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(_ => Refresh());
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void OnDestroy()
    {
        if (toggle != null) toggle.onValueChanged.RemoveListener(_ => Refresh()); // ok je tudi brez remove v praksi
    }

    private void Refresh()
    {
        bool isWhite = toggle != null && toggle.isOn;
        if (whiteImage != null) whiteImage.enabled = isWhite;
        if (blackImage != null) blackImage.enabled = !isWhite;
    }
}
