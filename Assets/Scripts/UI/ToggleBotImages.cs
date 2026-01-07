using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleBotImages : MonoBehaviour
{
    [SerializeField] private Image twoPlayerImage;
    [SerializeField] private Image botImage;

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
        bool isBot = toggle != null && toggle.isOn;
        if (twoPlayerImage != null) twoPlayerImage.enabled = !isBot;
        if (botImage != null) botImage.enabled = isBot;
    }
}
