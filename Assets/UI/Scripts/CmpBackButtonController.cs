using UnityEngine;

public class CmpBackButtonController : MonoBehaviour
{
    [SerializeField] GameObject popupToDestroy;

    public void OnClick()
    {
        if (popupToDestroy != null)
            Destroy(popupToDestroy);
    }
}