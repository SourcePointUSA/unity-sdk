using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CMP_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite defaultImage;
    [SerializeField] private Sprite activeImage;

    [SerializeField] private Vector3 defaultScale;
    [SerializeField] private Vector3 activeScale;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color activeColor;

    private void Awake()
    {
        SetIdleState();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetActiveState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetIdleState();
    }

    private void SetIdleState()
    {
        //image.color = defaultColor;
        //transform.localScale = defaultScale;
    }

    private void SetActiveState()
    {
        //image.color = activeColor;
        image.sprite = activeImage;
        //transform.localScale = activeScale;
    }
}
