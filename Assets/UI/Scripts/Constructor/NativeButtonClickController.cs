using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeButtonClickController : MonoBehaviour
{
    public void OnAcceptAllClick()
    {
        NetworkClient.Instance.ConsentGdpr(11 ,CmpLocalizationMapper.OnConsentGdprSuccessCallback , CmpLocalizationMapper.OnExceptionCallback, 3000);
        //TODO: destroy popup
    }

    public void OnSaveAndExitClick()
    {
        var saveAndExitVariables = new ConsentGdprSaveAndExitVariables("EN",
            privacyManagerId: "16879", 
            categories: CmpPmSaveAndExitVariablesContext.GetAcceptedCategories(), 
            vendors: CmpPmSaveAndExitVariablesContext.GetAcceptedVendors()); 
        NetworkClient.Instance.ConsentGdpr(1 ,CmpLocalizationMapper.OnConsentGdprSuccessCallback , CmpLocalizationMapper.OnExceptionCallback, 3000, saveAndExitVariables);
        //TODO: destroy popup
    }
}
