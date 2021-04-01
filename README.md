# Unity-sdk
Your plug &amp; play CMP for Unity

# Usage

Take a note that right now SDK is working with Android OS only. We will add other platforms in the nearest future!

To instantiate & trigger Consent Message Web View you must call function `ConsentWrapperV6.Instance.CallConsentAAR` along with accountId, propertyId, propertyName, pmId and privacyManagerTab.
```
  ConsentWrapperV6.Instance.CallConsentAAR(22,                              //accountId
                                           10589,                           //propertyId
                                           "http://sid-multi-campaign.com", //propertyName
                                           "404472",                        //pmId
                                           PRIVACY_MANAGER_TAB.DEFAULT);    //privacyManagerTab
```
If you want to use it with authID (this is an optional feature) you should call it like this:
```
  ConsentWrapperV6.Instance.CallConsentAAR(22,                              //accountId
                                           10589,                           //propertyId
                                           "http://sid-multi-campaign.com", //propertyName
                                           "404472",                        //pmId
                                           PRIVACY_MANAGER_TAB.DEFAULT,     //privacyManagerTab
                                           "Place_your_authID_here");       //authID
```
SDK will construct the Web View which is ready to interact with and will show it to user.

Once you did this, you may be interested in tracking progess and receiving updates of user interaction. We provide you such interfaces to do this:
```
IOnConsentUIReadyEventHandler,    //Triggers when user web view ui is ready and about to show
IOnConsentActionEventHandler,     //Triggers when user made an action, provides you instance of enum CONSENT_ACTION_TYPE
IOnConsentErrorEventHandler,      //
IOnConsentUIFinishedEventHandler, //
IOnConsentReadyEventHandler       //
```

General workflow:
```
using GdprConsentLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsentEventHandler : MonoBehaviour, IOnConsentUIReadyEventHandler, IOnConsentActionEventHandler, IOnConsentErrorEventHandler, IOnConsentUIFinishedEventHandler, IOnConsentReadyEventHandler
{
    void Awake()
    {
        ConsentMessenger.AddListener<IOnConsentUIReadyEventHandler>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentActionEventHandler>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentErrorEventHandler>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentUIFinishedEventHandler>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentReadyEventHandler>(this.gameObject);
    }

    public void OnConsentUIReady()
    {
        Debug.LogWarning("User will be shown the web view with series of consent messages!");
    }

    public void OnConsentAction(CONSENT_ACTION_TYPE action)
    {
        Debug.LogWarning($"User made {action} action with consent view!");
    }

    public void OnConsentError(Exception exception)
    {
        Debug.LogError("Oh no, an error! " + exception.Message);
    }

    public void OnConsentUIFinished()
    {
        Debug.LogWarning("User has interacted with the web view consent message and it is disappeared!");
    }

    public void OnConsentReady(SpConsents consents)
    {
        Debug.Log("The user interaction on series of consent messages is done. You can continue user's gaming experience!");
    }

    private void OnDestroy()
    {
        ConsentMessenger.RemoveListener<IOnConsentUIReadyEventHandler>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentActionEventHandler>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentErrorEventHandler>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentUIFinishedEventHandler>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentReadyEventHandler>(this.gameObject);
    }
}
```

