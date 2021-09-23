using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeButtonClickController : MonoBehaviour
{
    public void OnAcceptAllClick()
    {
        NetworkClient.Instance.ConsentGdpr(CmpLocalizationMapper.OnConsentGdprSuccessCallback , CmpLocalizationMapper.OnExceptionCallback, 3000);
    }
}
