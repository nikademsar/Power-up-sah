using UnityEngine;

public class ThemeDropdown : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;

    private void Awake()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    public void ToggleDropdown()
    {
        if (optionsPanel == null) return;
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void CloseDropdown()
    {
        if (optionsPanel == null) return;
        optionsPanel.SetActive(false);
    }

    public void OpenDropdown()
    {
        if (optionsPanel == null) return;
        optionsPanel.SetActive(true);
    }
}