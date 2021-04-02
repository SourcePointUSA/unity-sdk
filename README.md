# Unity-sdk
Your plug &amp; play CMP for Unity

# Usage - Instantiate Consent UI

Take a note that right now SDK is working with Android OS only. We will add other platforms in the nearest future!

To instantiate & trigger Consent Message Web View you must call function `ConsentWrapperV6.Instance.CallConsentAAR` along with accountId, propertyId, propertyName, pmId and privacyManagerTab.
```c#
ConsentWrapperV6.Instance.CallConsentAAR(22,                              //accountId
                                         10589,                           //propertyId
                                         "http://sid-multi-campaign.com", //propertyName
                                         "404472",                        //pmId
                                         PRIVACY_MANAGER_TAB.DEFAULT);    //privacyManagerTab
```
If you want to use it with authID (this is an optional feature) you should call it like this:
```c#
ConsentWrapperV6.Instance.CallConsentAAR(22,                              //accountId
                                         10589,                           //propertyId
                                         "http://sid-multi-campaign.com", //propertyName
                                         "404472",                        //pmId
                                         PRIVACY_MANAGER_TAB.DEFAULT,     //privacyManagerTab
                                         "Place_your_authID_here");       //authID
```
SDK will construct the Web View which is ready to interact with and will show it to user.

`PRIVACY_MANAGER_TAB` it's the tab which is selected to be shown first:
```c#
DEFAULT, 
PURPOSES,
VENDORS,
FEATURES
```

Once you did this, you may be interested in tracking progess and receiving updates of user interaction. We provide you such list of `IConsentEventHandler` interfaces to do this:
```c#
IOnConsentUIReadyEventHandler,    //Triggers when web view UI is ready and about to show 
IOnConsentActionEventHandler,     //Triggers when user made an action, provides you instance of enum CONSENT_ACTION_TYPE
IOnConsentErrorEventHandler,      //Triggers when something went wrong, provides you instance of Excetion
IOnConsentUIFinishedEventHandler, //Triggers when user interaction with web view UI is done and view is about to dissapear
IOnConsentReadyEventHandler       //Triggers when server succesfully reacted to user's consent, provides you jsonConsents string with consent info
```

CONSENT_ACTION_TYPE can be the following:
```c#
SAVE_AND_EXIT = 1,
PM_DISMISS = 2,
ACCEPT_ALL = 11,
SHOW_OPTIONS = 12,
REJECT_ALL = 13,
MSG_CANCEL = 15,
```

# Usage - Workflow of handling callbacks using interfaces:
Once you created your own `MonoBehaviour` script and attached this component to your `GameObject` you should:
* Inherit your script from any quantity of interfaces from `IConsentEventHandler` lsit you interested in and implement it's method(s)
For example, let's suppose you want to handle Exception callback via `IOnConsentErrorEventHandler`, and you already implemented `IOnConsentErrorEventHandler` inheritance and `OnConsentError` method in your script and attached this script to generic `GameObject` in hierarchy. What's next?
```c#
public class ConsentEventHandler : MonoBehaviour, IOnConsentErrorEventHandler
{
    public void OnConsentError(Exception exception)
    {
        Debug.LogError("Oh no, an error! " + exception.Message);
    }
}
```
* Register your `gameobject` (which implements any `IConsentEventHandler` interface obviously) as an event listener with `ConsentMessenger.AddListener` static method. It can be registered any time before you call the `CallConsentAAR` method (`Awake`, `Start` is enough, but you can adopt registration to your own logic)
```c#
void Awake()
{
    ConsentMessenger.AddListener<IOnConsentErrorEventHandler>(this.gameObject);
}
``` 
 ⤤ Adds current `gameobject` as listener for `IOnConsentErrorEventHandler` events ⤣ 

* Every interface from `IConsentEventHandler` list should be registered as a listener separately. Only registered `gameobject`'s methods are invoked. The according methods (such as `OnConsentError` in this example) will be invoked automatically when event triggers!

* Also you should unregister your listener when it becomes unnecessary due to garbage collection. `OnDestroy` is enough for our purposes:
```
private void OnDestroy()
{
    ConsentMessenger.RemoveListener<IOnConsentErrorEventHandler>(this.gameObject);
}
```
* BOOM! The solution is ready, configurate it and deploy directly to production!
 
Both calling & handling workflow are implemented in the `ConsentButtonCaller` and `ConsentEventHandler` scripts accordingly. Feel free to use this components! 
```c#
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
