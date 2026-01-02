using UnityEngine;
using UnityEngine.UI;

public class CapturedPiecesUI : MonoBehaviour
{
    [SerializeField] private Transform whiteCapturedParent;
    [SerializeField] private Transform blackCapturedParent;
    [SerializeField] private Image capturedIconPrefab;

    public void AddCaptured(bool capturedWasWhite, Sprite pieceSprite)
    {
        // če je bila ujeta bela figura → gre v BlackCapturedRow
        Transform parent = capturedWasWhite ? blackCapturedParent : whiteCapturedParent;

        Image icon = Instantiate(capturedIconPrefab, parent);
        icon.sprite = pieceSprite;
    }

    public void Clear()
    {
        foreach (Transform t in whiteCapturedParent) Destroy(t.gameObject);
        foreach (Transform t in blackCapturedParent) Destroy(t.gameObject);
    }
}