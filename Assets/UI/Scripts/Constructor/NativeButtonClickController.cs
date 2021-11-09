using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeButtonClickController : MonoBehaviour
{
    public void OnAcceptAllClick()
    {
        CmpLocalizationMapper.Consent(11);
    }

    public void OnSaveAndExitClick()
    {
        CmpLocalizationMapper.Consent(1);
    }

    public void OnRejectAllClick()
    {
        CmpLocalizationMapper.Consent(13);
    }
}
