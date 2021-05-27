# Unity-SDK
Your plug &amp; play CMP for Unity

# Usage - Instantiate Consent UI

Take a note that right now SDK is working with Android OS only. We will add other platforms in the nearest future!

First of all, you need to construct `List<SpCampaign>` which contains the SpCampaign objects which consist of `CAMPAIGN_TYPE` along with `TargetingParams` you need.
```c#
List<SpCampaign> spCampaigns = new List<SpCampaign>();
List<TargetingParam> gdprParams = new List<TargetingParam> { new TargetingParam("location", "EU") };
SpCampaign gdpr = new SpCampaign(CAMPAIGN_TYPE.GDPR, gdprParams);
spCampaigns.Add(gdpr);
```

To instantiate & trigger Consent Message Web View you must call function `ConsentWrapperV6.Instance.InitializeLib` in `Awake` along with spCampaigns, accountId, propertyName and language. You can also specify `messageTimeout` if you want, by default it is set to 3000 **milliseconds**.
```c#
ConsentWrapperV6.Instance.InitializeLib(spCampaigns: spCampaigns,
                                        accountId: 22,
                                        propertyName: "sid-multi-campaign.com",
                                        language: MESSAGE_LANGUAGE.ENGLISH);
```
Take a note that it may take some time to initialize CMP library, so we strongly suggest you to do this in `Awake`. SDK will construct the Web View which is ready to interact with and will show it to user.

To have a healthy flow of CMP library we also reccomend you to add next code pieces:

1. If user changes app focus during the first layer message flow, we need to re-show it once user got back to app. Next code piece will bring it up to user the consent message, if there any not resolved messages. 
```c#
private void OnApplicationPause(bool pause)
{
    if (!pause)
    {
        ConsentWrapperV6.Instance.LoadMessage(authId: authID);
    }
}
```
If there is a consent profile associated with authId ("JohDoe"), the SDK will bring the consent data from the server, overwriting whatever was stored in the device.

2. In order to free memory, necessary to call `Dispose` like this: 
```c#
private void OnDestroy()
{
    ConsentWrapperV6.Instance.Dispose();
}
```

# Usage - Handling Consent Callbacks

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
public enum CONSENT_ACTION_TYPE 
{
    SAVE_AND_EXIT = 1,
    PM_DISMISS = 2,
    ACCEPT_ALL = 11,
    SHOW_OPTIONS = 12,
    REJECT_ALL = 13,
    MSG_CANCEL = 15,
}
```

# Usage - Workflow of handling callbacks using interfaces:
Once you created your own script which derives from `MonoBehaviour` and attached this component to your `GameObject` you should:
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
* Register your `gameobject` (which implements any inheritor of `IConsentEventHandler` interface) as an event listener with `ConsentMessenger.AddListener` static method. It can be registered any time before you call the `CallConsentAAR` method (`Awake`, `Start` is enough, but you can adopt registration to your own logic)
```c#
void Awake()
{
    ConsentMessenger.AddListener<IOnConsentErrorEventHandler>(this.gameObject);
}
``` 
 ⤤ Adds current `gameObject` as listener for `IOnConsentErrorEventHandler` events ⤣ 
Please take a note that the event will be executed on all components of the game object that can handle it, regardless of whether they are subscribed or not if at least one have registered the gameObject as listener.

* Also you should unregister your listener when it becomes unnecessary due to garbage collection. `OnDestroy` is enough for our purposes:
```c#
private void OnDestroy()
{
    ConsentMessenger.RemoveListener<IOnConsentErrorEventHandler>(this.gameObject);
}
```
* BOOM! The solution is ready, configurate it and deploy!
 
Both calling & handling workflow are implemented in the `ConsentButtonCaller` and `ConsentEventHandler` scripts of example app accordingly. Feel free to use this components! 
```c#
using ConsentManagementProviderLib;
using System;
using UnityEngine;

public class ConsentEventHandler : MonoBehaviour, 
                                   IOnConsentUIReadyEventHandler, 
                                   IOnConsentActionEventHandler, 
                                   IOnConsentErrorEventHandler, 
                                   IOnConsentUIFinishedEventHandler, 
                                   IOnConsentReadyEventHandler
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

    public void OnConsentReady(string jsonConsents)
    {
        Debug.Log($"The user interaction on consent messages is done. Consent info: {jsonConsents} \n If it was the last from the series of consents, you can continue user's gaming experience!");
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
