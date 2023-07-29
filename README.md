# Unity-SDK
Your plug &amp; play CMP for Unity.

<mark>**Note**: Sourcepoint's Unity SDK can be used for both Android OS and iOS. Sourcepoint's Unity SDK can be integrated with both Android and iOS. Since it embeds native SDKs, and those only work in their respective platforms, the Unity SDK can't be demoed using Unity's Editor.

# Instantiate consent UI

To start, include the following library namepsace in your script:
```c#
using ConsentManagementProviderLib;
```

1. Construct `List<SpCampaign>` which contains `SpCampaign` objects. Each `SpCampaign` object should consist of `CAMPAIGN_TYPE` along with `TargetingParams` you need.
```c#
    List<SpCampaign> spCampaigns = new List<SpCampaign>();

    List<TargetingParam> gdprParams = new List<TargetingParam> { new TargetingParam("location", "EU") };
    SpCampaign gdpr = new SpCampaign(CAMPAIGN_TYPE.GDPR, CAMPAIGN_ENV.PUBLIC, gdprParams);
    spCampaigns.Add(gdpr);

    List<TargetingParam> ccpaParams = new List<TargetingParam> { new TargetingParam("location", "US") };
    SpCampaign ccpa = new SpCampaign(CAMPAIGN_TYPE.CCPA, CAMPAIGN_ENV.PUBLIC, ccpaParams);
    spCampaigns.Add(ccpa);

    List<TargetingParam> ios14Params = new List<TargetingParam>();
    SpCampaign ios14 = new SpCampaign(CAMPAIGN_TYPE.IOS14, CAMPAIGN_ENV.PUBLIC, ios14Params);
    spCampaigns.Add(ios14);
```

2. In order to instantiate & trigger Consent Message Web View, you must call the `CMP.Initialize` function in `Awake` along with spCampaigns, accountId, propertyName and language.<br/> <br/>Additionally, you can also specify a `messageTimeout` which is set to **3 seconds** by default.
```c#
    CMP.Initialize(spCampaigns: spCampaigns,
                   accountId: 22,
                   propertyName: "mobile.multicampaign.demo",
                   language: MESSAGE_LANGUAGE.ENGLISH,
                   campaignsEnvironment: CAMPAIGN_ENV.PUBLIC,
                   messageTimeoutInSeconds: 3);
```

<mark>**Note**: It may take a frame to initialize the CMP library, so we strongly recommend that you `Initialize` in `Awake` separately from `LoadMessage`. We recommend that you `LoadMessage` in `Start` (see example below).</mark>

3. Right after the  `LoadMessage` call, the SDK will construct the Web View for the end-user. <br/><br/> If there is a consent profile associated with authId "JohnDoe", the SDK will bring the consent data from the server, overwriting whatever was stored in the device.
```c#
private void Start()
{
     CMP.LoadMessage(authId: null); // or pass it a String if you wish to use authenticated consent
}
```
3. In order to free memory, call `Dispose` as illustrated in the following example :
```c#
private void OnDestroy()
{
    CMP.Dispose();
}
```

# Handle consent callbacks

Consent callbacks allow you to track progress and receive updates of user interaction. We provide the following interfaces:

| Callback               | Description                                                                                                             |
|------------------------|-------------------------------------------------------------------------------------------------------------------------|
| `IOnConsentUIReady`    | Triggered when web view UI is ready and about to show                                                                   |
| `IOnConsentAction`     | Triggered when user made an action, provides you instance of enum `CONSENT_ACTION_TYPE`. See below for more information.|
| `IOnConsentError`      | Triggered when something went wrong, provides you instance of Exception                                                 |
| `IOnConsentUIFinished` | Triggered when user interaction with web view UI is done and view is about to disappear                                 |
| `IOnConsentReady`      | Triggered when server successfully reacted to user's consent, provides you `SpConsent` object with consent info         |

`CONSENT_ACTION_TYPE` can return the following:
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

# Workflow to handle callbacks using interfaces
Once you created your own script which derives from `MonoBehaviour` and attached this component to your `GameObject` you should:
1. Inherit your script from any number of interfaces from the `IConsentEventHandler` list you are interested in and implement its method(s).<br/>
For example, suppose you want to handle Exception callback via `IOnConsentErrorEventHandler`, and you already implemented `IOnConsentErrorEventHandler` inheritance and `OnConsentError` method in your script and attached this script to generic `GameObject` in hierarchy. What's next?
```c#
public class ConsentEventHandler : MonoBehaviour, IOnConsentError
{
    public void OnConsentError(Exception exception)
    {
        Debug.LogError("Oh no, an error! " + exception.Message);
    }
}
```
2. Register your `gameObject` (which implements any inheritor of `IConsentEventHandler` interface) as an event listener with `ConsentMessenger.AddListener` static method. It can be registered any time before you call the `LoadMessage` method (`Awake`, `Start` is enough, but you can adopt registration to your own logic).
```c#
void Awake()
{
    ConsentMessenger.AddListener<IOnConsentError>(this.gameObject);
}
```
 ⤤ Adds current `gameObject` as listener for `IOnConsentError` events ⤣<br/>
<mark>**Note**: The event will be executed on all components of the game object that can handle it, regardless of whether they are subscribed or not if at least one have registered the `gameObject` as a listener.</mark>

3. You should also unregister your listener when it becomes unnecessary due to garbage collection. `OnDestroy` is enough for our purposes:
```c#
private void OnDestroy()
{
    ConsentMessenger.RemoveListener<IOnConsentError>(this.gameObject);
}
```
4. The solution is ready. Configure it and deploy!

Both calling & handling workflows are implemented in the `ConsentMessageProvider` and `ConsentEventHandler` scripts of our example app accordingly. Feel free to use these components.

```c#
using ConsentManagementProviderLib;
using System;
using UnityEngine;

public class ConsentEventHandler : MonoBehaviour, IOnConsentUIReady, IOnConsentAction, IOnConsentError, IOnConsentUIFinished, IOnConsentReady
{
    void Awake()
    {
        ConsentMessenger.AddListener<IOnConsentUIReady>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentAction>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentError>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentUIFinished>(this.gameObject);
        ConsentMessenger.AddListener<IOnConsentReady>(this.gameObject);
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

    public void OnConsentReady(SpConsents spConsents)
    {
       Debug.Log($"The user interaction on consent messages is done. You can use the spConsent info; " +
          $"\n If it was the last from the series of consents, you can continue user's gaming experience!" +
          $"\n {spConsents.ToString()}");
    }

    private void OnDestroy()
    {
        ConsentMessenger.RemoveListener<IOnConsentUIReady>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentAction>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentError>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentUIFinished>(this.gameObject);
        ConsentMessenger.RemoveListener<IOnConsentReady>(this.gameObject);
    }
}
```

# Resurface Privacy Manager

Once a player has completed the consent flow, you may be interested to resurface your privacy manager so the player can see/manage their consents. To do this we provide the `LoadPrivacyManager` method. The following code snippet will show a GDPR privacy manager with default tab open.
```c#
    public void OnPrivacyManagerButtonClick()
    {
        CMP.LoadPrivacyManager(campaignType: CAMPAIGN_TYPE.GDPR,
                               pmId: "488393",
                               tab: PRIVACY_MANAGER_TAB.DEFAULT);
    }
```
Below is a list of available tabs in a GDPR privacy manager:
```c#
    public enum PRIVACY_MANAGER_TAB
    {
        DEFAULT = 0,
        PURPOSES = 1,
        VENDORS = 2,
        FEATURES = 3
    }
```

# Build for iOS

Since Unity Editor exports the pre-built project to Xcode on iOS build, there are several necessary steps to perform so you can compile your solution. They are implemented inside the `CMPPostProcessBuild` [PostProcessBuild] script. Feel free supplement or modify it if needed.
