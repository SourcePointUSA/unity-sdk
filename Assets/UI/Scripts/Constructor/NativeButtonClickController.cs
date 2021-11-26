using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NativeButtonClickController : MonoBehaviour
{
    private static bool dnsClicked = false;

    private void Start()
    {
        CmpNativeButtonUiController uiController = gameObject.GetComponent<CmpNativeButtonUiController>();
        Button butt = gameObject.GetComponent<Button>();
        if (CmpCampaignPopupQueue.CurrentCampaignToShow() == 2 && uiController.ID.Equals("DoNotSellButton"))
            butt.onClick.AddListener(OnDoNotSellClick);
        else if (uiController.ID.Equals("RejectAllButton"))
            butt.onClick.AddListener(OnRejectAllClick);
    }

    public void OnAcceptAllClick()
    {
        CmpLocalizationMapper.Consent(11);
    }

    public void OnSaveAndExitClick()
    {
        if (CmpCampaignPopupQueue.CurrentCampaignToShow() == 2 && dnsClicked)
            OnRejectAllClick();
        else
            CmpLocalizationMapper.Consent(1);
    }

    public void OnRejectAllClick()
    {
        CmpLocalizationMapper.Consent(13);
    }

    private void OnDoNotSellClick()
    {
        dnsClicked = true;
    }

    private void OnDestroy()
    {
        dnsClicked = false;
    }
}
