# Unity-SDK
Your plug &amp; play CMP for Unity.

<mark>**Note**: Sourcepoint's Unity SDK can be used for both Android OS and iOS. When you are ready to compile your project for iOS, you will need to perform additional steps. [Click here](https://github.com/SourcePointUSA/unity-sdk#usage--build-for-ios) for more information.

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
                   propertyName: "sid-multi-campaign.com",
                   language: MESSAGE_LANGUAGE.ENGLISH,
                   campaignsEnvironment: CAMPAIGN_ENV.STAGE,
                   messageTimeoutInSeconds: 3);
```

<mark>**Note**: It may take a frame to initialize the CMP library, so we strongly recommend that you `Initialize` in `Awake` separately from `LoadMessage`. We recommend that you `LoadMessage` in `Start` (see example below).</mark>

3. Right after the  `LoadMessage` call, the SDK will construct the Web View for the end-user. <br/><br/> If there is a consent profile associated with authId "JohnDoe", the SDK will bring the consent data from the server, overwriting whatever was stored in the device.
There is two possible ways to call `LoadMessage`, first works only for mobile platforms:
```c#
private void Start()
{
    CMP.LoadMessage(authId: "JohnDoe");
}
```
And second is for any other platform Unity supports. This way uses Unity Native UI to show Privacy Manager and reqiures `Canvas` to instantiate on and prefab which located at /Assets/UI/Prefabs/Home.prefab
```c#
CMP.LoadMessage(cmpHomePrefab: CmpPrefab,
                canvas: canvas,
                privacyManagerId: "16879",
                propertyId: "4933");
```
We recommend you using first overload of method for mobile platforms and second for any other. Please note that property name and other credentials may differ.
    
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

# Resurface Privacy Manager

Once a player has completed the consent flow, you may be interested to resurface your privacy manager so the player can see/manage their consents. To do this we provide the `LoadPrivacyManager` method. The following code snippet will show a GDPR privacy manager with default tab open.
Once again, there is two overloads of `LoadPrivacyManager` first is for mobile platforms:
```c#
    public void OnPrivacyManagerButtonClick()
    {
        CMP.LoadPrivacyManager(campaignType: CAMPAIGN_TYPE.GDPR,
                               pmId: "488393",
                               tab: PRIVACY_MANAGER_TAB.DEFAULT);
    }
```
And second is for any other OS:
```c#
        public void LoadGdprPM()
    {
        CMP.LoadPrivacyManager(CmpPrefab, canvas, CAMPAIGN_TYPE.GDPR, "16879", "4933");
    }
    
    public void LoadCcpaPM()
    {
        CMP.LoadPrivacyManager(CmpPrefab, canvas, CAMPAIGN_TYPE.CCPA, "16435", "17935");
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

# Custom GDPR Consent

To provide you with the ability to request custom GDPR consent with specific vendors, categories, and legitimate interest categories we created the `CustomConsentGDPR` method. One additional step to handle the result of the call is the delegate which handles the `GdprConsent` object, represented as `SuccessDelegate` method in our code example below. It will be asynchronously triggered when the result of end-user interaction is handled. Works for mobile platforms only in the moment.
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

# Build for iOS

Since Unity Editor exports the pre-built project to Xcode on iOS build, there are several necessary steps to perform so you can compile your solution. You can follow the steps below to supplement our `CMPPostProcessBuild`[PostProcessBuild] script.

## iOS – Adding the `ConsentViewController.xcframework`
To use xcframework functionality you should tie it with your xcode project.
1. Drag and drop `ConsentViewController.xcframework` to your Xcode project (into Project Navigator). Select the checkboxes for both **Unity-iPhone** and **UnityFrameworks** under the **Add to targets** field.
2. Find in Project Navigator the `Unity-Iphone` icon. Usually it is topmost. Select it.
3. Then select `Unity-iPhone` target
4. Select `Build Phase`
5. Add `ConsentViewController.xcframework` to `EmbedFramework` section
6. **If building for device**: Tick the `Copy only when installing` checkbox. <mark>**Note**: Skip this step if you are building for a simulator.</mark>
7. Select `UnityFramework` target
8. Add `ConsentViewController.xcframework` to `Link Binary With Libraries` section

<mark>**Note**: Unity may handle XCFramework in a wrong way if you store it in your Unity project and have any import settings applied. In such case, go to `UnityFramework` target > `Build Phases` > `Link Binary With Libraries` and delete all entries of `ConsentViewController.framework` (But make sure that **xcframework** is still there)
<br/><br/>
If you store xcframework under **Assets** folder in your unity project, to avoid this odd behaviour in the future, go to the  **ConsentViewController.xcframework** folder in Unity Editor, and perform the following for both **ios-arm64_armv7** and **ios-arm64_i386_x86_64-simulator** folders:
<br/><br/>
• Select **ConsentViewController.framework** <br/>
• Untick every platform in **Select platforms for plugin**<br/> • Click **Apply** to import the new settings.
</mark>

## iOS – Enable `New Build System`
 Apple introduced .xcframework to replace the old .framework. The .xcframework requires a new build system. Unity older than 2020.3.10 enables `Legacy Build System (Deprecated)` by default so it is necessary to switch the `Build System` to the new one manually.
1. In Xcode navigate to `Project` > `Project Settings`.
2. Switch `Build System` to `New Build System (Default)`.

## iOS – Add tracking disclaimer to `info.plist`
This feature is also required by Apple due to privacy terms.
1. Find in Project Navigator and open `info.plist` file.
2. Click `+` icon which adds new row.
3. Add following as a key: `Privacy - Tracking Usage Description`
4. Add following as a value `This identifier will be used to deliver personalized ads to you.`
