using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowTextOnHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private Text hoverText;

    void Start()
    {
        hoverText.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverText.gameObject.SetActive(false);
    }
}
