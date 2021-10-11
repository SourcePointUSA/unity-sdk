using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeButtonClickController : MonoBehaviour
{
    public void OnAcceptAllClick()
    {
        CmpLocalizationMapper.ConsentGdpr(11);
    }

    public void OnSaveAndExitClick()
    {
        CmpLocalizationMapper.ConsentGdpr(1);
    }

    public void OnRejectAllClick()
    {
        CmpLocalizationMapper.ConsentGdpr(13);
    }
}
