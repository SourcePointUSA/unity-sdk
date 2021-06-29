# Unity-SDK
Your plug &amp; play CMP for Unity

# Usage - Instantiate Consent UI

Take a note that right now SDK is working with Android OS only. We will add other platforms in the nearest future!

First of all, you should include the library namespace to your script:
```c#
using ConsentManagementProviderLib;
```

1. You need to construct `List<SpCampaign>` which contains the SpCampaign objects which consist of `CAMPAIGN_TYPE` along with `TargetingParams` you need.
```c#
    List<SpCampaign> spCampaigns = new List<SpCampaign>();
    
    List<TargetingParam> gdprParams = new List<TargetingParam> { new TargetingParam("location", "EU") };
    SpCampaign gdpr = new SpCampaign(CAMPAIGN_TYPE.GDPR, CAMPAIGN_ENV.PUBLIC, gdprParams);
    spCampaigns.Add(gdpr);
    
    List<TargetingParam> ccpaParams = new List<TargetingParam> { new TargetingParam("location", "US") };
    SpCampaign ccpa = new SpCampaign(CAMPAIGN_TYPE.CCPA, CAMPAIGN_ENV.PUBLIC, ccpaParams);
    spCampaigns.Add(ccpa);
    
    List<TargetingParam> ios14Params = new List<TargetingParam>();
    SpCampaign ccpa = new SpCampaign(CAMPAIGN_TYPE.IOS14, CAMPAIGN_ENV.PUBLIC, ios14Params);
    spCampaigns.Add(ccpa);
```

1. To instantiate & trigger Consent Message Web View you must call function `CMP.Initialize` in `Awake` along with spCampaigns, accountId, propertyName and language. You can also specify `messageTimeout` if you want, by default it is set to 3 **seconds**.
```c#
    CMP.Initialize(spCampaigns: spCampaigns,
                   accountId: 22,
                   propertyName: "sid-multi-campaign.com",
                   language: MESSAGE_LANGUAGE.ENGLISH,
                   messageTimeoutInSeconds: 3);
```

Take a note that it may take a frame to initialize CMP library, so we strongly recommend you to do `Initialize` in `Awake` separately from next step `LoadMessage` which is better to perform in `Start`. 

2. Right after `LoadMessage` call SDK will construct the Web View which is ready to interact with and will show it to user.
```c#
private void Start()
{
    CMP.LoadMessage(authId: "<Your autId if required>");
}
```
If there is a consent profile associated with authId "JohnDoe", the SDK will bring the consent data from the server, overwriting whatever was stored in the device.

3. In order to free memory, necessary to call `Dispose` like this: 
```c#
private void OnDestroy()
{
    CMP.Dispose();
}
```

# Usage - Handling Consent Callbacks

Once you did this, you may be interested in tracking progess and receiving updates of user interaction. We provide you such list of interfaces to do this:
```c#
IOnConsentUIReady,    //Triggered when web view UI is ready and about to show 
IOnConsentAction,     //Triggered when user made an action, provides you instance of enum CONSENT_ACTION_TYPE
IOnConsentError,      //Triggered when something went wrong, provides you instance of Exception
IOnConsentUIFinished, //Triggered when user interaction with web view UI is done and view is about to dissapear
IOnConsentReady       //Triggered when server succesfully reacted to user's consent, provides you SpConsent object with consent info
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
public class ConsentEventHandler : MonoBehaviour, IOnConsentError
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
    ConsentMessenger.AddListener<IOnConsentError>(this.gameObject);
}
``` 
 ⤤ Adds current `gameObject` as listener for `IOnConsentError` events ⤣ 
**Please take a note that the event will be executed on all components of the game object that can handle it, regardless of whether they are subscribed or not if at least one have registered the gameObject as listener.**

* Also you should unregister your listener when it becomes unnecessary due to garbage collection. `OnDestroy` is enough for our purposes:
```c#
private void OnDestroy()
{
    ConsentMessenger.RemoveListener<IOnConsentError>(this.gameObject);
}
```
* BOOM! The solution is ready, configurate it and deploy!

Both calling & handling workflows are implemented in the `ConsentMessageProvider` and `ConsentEventHandler` scripts of our example app accordingly. Feel free to use this components! 

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
        Debug.Log($"The user interaction on consent messages is done. You can use the spConsent info; \n If it was the last from the series of consents, you can continue user's gaming experience!");
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

# Usage – Privacy Manager re-show

Once player completed the consent flow, you may be interested to re-show `Privacy Manager` so user could see or manage own consents. To do this we provide `LoadPrivacyManager` method. The following code snippet will show GDPR privacy manager with default tab open.
```c#
    public void OnPrivacyManagerButtonClick()
    {
        CMP.LoadPrivacyManager(campaignType: CAMPAIGN_TYPE.GDPR,
                               pmId: "488393",
                               tab: PRIVACY_MANAGER_TAB.DEFAULT);
    }
```
By the way, here is the list of available tabs : 
```c#
    public enum PRIVACY_MANAGER_TAB
    {
        DEFAULT = 0,
        PURPOSES = 1,
        VENDORS = 2,
        FEATURES = 3
    }
```

# Usage – Custom GDPR Consent

To provide you ability to request custom GDPR consent with specific vendors, categories and legitimate interest categories we created method `CustomConsentGDPR`. One additional step to handle the result of such call is delegate which handles `GdprConsent` object, represented as `SuccessDelegate` method in our code example. It will be asyncroniously triggered when the result of user interactiov will have been handled.
```c#
    public void OnCustomConsentButtonClick()
    {
        CMP.CustomConsentGDPR(vendors: new string[] { "5fbe6f050d88c7d28d765d47" },
                              categories: new string[] { "60657acc9c97c400122f21f3" },
                              legIntCategories: new string[] { },
                              onSuccessDelegate: SuccessDelegate);
    }

    private void SuccessDelegate(GdprConsent customConsent)
    {
        Debug.Log($"I am your success callback!");
    }
```

# Usage – Building for iOS

Since Unity Editor export the pre-built project to Xcode on iOS build, there is several necessary steps to perform so you could compile your solution. You can follow next steps or write own custom [PostProcessBuild] script to do it for you.

## iOS – Adding the `ConsentViewController.xcframework`
To use xcframework functionality you should tie it with your xcode project. 
1. Drag and drop `ConsentViewController.xcframework` to your Xcode project (into Project Navigator)
2. Find in Project Navigator the `Unity-Iphone` icon. Usually it is topmost. Select it.
3. Then select `Unity-iPhone` target
4. Select `Build Phase`
5. Add `ConsentViewController.xcframework` to `EmbedFramework` section
6. Tick the `Copy only when installing` checkbox to hassle-free building for device build only (if needed). **Skip this step if build for simulator**
7. Select `UnityFramework` target
8. Add `ConsentViewController.xcframework` to `Link Binary With Libraries` section

Take a note: Unity may handle XCFramework in wrong way if you store it in your Unity project and have any import settings applied. In such case, go to `UnityFramework` target > `Build Phases` > `Link Binary With Libraries` and delete all entries of `ConsentViewController.framework` (But make sure that **xcframework** is still there)

## iOS – Enabling `New Build System`
With release of ios14 Apple presented .xcframework to replace old .framework which requires modern build system. On the other hand, Unity enables `Legacy Build System (Deprecated)` so it is necessary to switch the `Build System` to new one.
1. Go to `Project` > `Project Settings`.
2. Switch `Build System` to `New Build System (Default)`.

## iOS – Adding tracking disclaimer to `info.plist`
This feature is also reqired by Apple due to new privacy terms.
1. Find in Project Navigator and open `info.plist` file. 
2. Click `+` icon which adds new row. 
3. Add following as a key: `Privacy - Tracking Usage Description`
4. Add following as a value `This identifier will be used to deliver personalized ads to you.`

## iOS – Enabling Objective-C Exceptions
Due to usage of exceptions, it is necessary to enable them.
1. Find in Project Navigator the `Unity-Iphone` icon. Usually it is topmost. Click it.
2. Select `Unity-Iphone` project.
3. Go to `Build Settings`. Find `Enable Objective-C Exceptions` and set it to `Yes`.
