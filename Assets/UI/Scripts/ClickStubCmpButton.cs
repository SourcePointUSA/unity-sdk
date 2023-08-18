using UnityEngine;
using ConsentManagementProviderLib;

public class ClickStubCmpButton : MonoBehaviour
{
    public void LogStubOnClick()
    {
        CmpDebugUtil.Log($"{this.name} element has been CKICKED");
    }
}
