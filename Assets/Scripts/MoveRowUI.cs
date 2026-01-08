using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveRowUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text noText;
    [SerializeField] private TMP_Text whiteText;
    [SerializeField] private TMP_Text blackText;

    [Header("Optional highlight backgrounds (Image)")]
    [SerializeField] private Image whiteHighlight;
    [SerializeField] private Image blackHighlight;

    public void SetNumber(int n)
    {
        if (noText) noText.text = $"{n}.";
    }

    public void SetWhite(string s)
    {
        if (whiteText) whiteText.text = s ?? "";
    }

    public void SetBlack(string s)
    {
        if (blackText) blackText.text = s ?? "";
    }

    public void ClearHighlight()
    {
        if (whiteHighlight) whiteHighlight.enabled = false;
        if (blackHighlight) blackHighlight.enabled = false;
    }

    public void HighlightWhite()
    {
        if (whiteHighlight) whiteHighlight.enabled = true;
    }

    public void HighlightBlack()
    {
        if (blackHighlight) blackHighlight.enabled = true;
    }
}