using UnityEngine;

public class ClickStubCmpButton : MonoBehaviour
{
    public void LogStubOnClick()
    {
        Debug.Log($"{this.name} element has been CLICKED");
    }
}
