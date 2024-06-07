# Unity-SDK

In Unity SDK version 2.3.3, the native Android SDK dependency was updated to version 7.8.3, so CMP SDK is now supported on Unity version 2022.3.0 and higher. This was done due to Java version upgrade as well as Gradle upgrade to version 7.5 in native Android SDK 7.8.2 which is a dependency of Unity SDK. However, we are able to build with Gradle v7.2 which is built-in in Unity 2022.3
Please also note that in order to build a project:
* You need to use a custom template for Gradle, please refer to "Plugins\Android\mainTemplate.gradle", lines 41-43 are what you need.

Sourcepoint's plug and play Unity SDK can be integrated with both Android and iOS.

> **Note**: The Unity SDK can not be demoed using Unity's Editor since it embeds native SDKs and those only work in their respective platforms.
> <br><br>Additionally, this SDK utilizes [ExternalDependencyManager by Google](https://github.com/googlesamples/unity-jar-resolver) in order to fetch native SDKs and their dependencies. Ensure all the dependencies mentioned in `Assets/ExternalDependencyManager/Editor/SourcepointDependencies.xml` are resolved before building your application.

---

# Instantiate consent UI

To start, include the following library namepsace in your script:

```c#
using ConsentManagementProviderLib;
```

Construct `List<SpCampaign>` which contains `SpCampaign` objects. Each `SpCampaign` object should consist of `CAMPAIGN_TYPE` along with the `TargetingParams` you need.

```c#
    List<SpCampaign> spCampaigns = new List<SpCampaign>();

    List<TargetingParam> gdprParams = new List<TargetingParam> { new TargetingParam("location", "EU") };
    SpCampaign gdpr = new SpCampaign(CAMPAIGN_TYPE.GDPR, gdprParams);
    spCampaigns.Add(gdpr);

    List<TargetingParam> ccpaParams = new List<TargetingParam> { new TargetingParam("location", "US") };
    SpCampaign ccpa = new SpCampaign(CAMPAIGN_TYPE.CCPA, ccpaParams);
    spCampaigns.Add(ccpa);

    List<TargetingParam> ios14Params = new List<TargetingParam>();
    SpCampaign ios14 = new SpCampaign(CAMPAIGN_TYPE.IOS14, ios14Params);
    spCampaigns.Add(ios14);

    List<TargetingParam> usnatParams = new List<TargetingParam> { new TargetingParam("location", "US") };
    SpCampaign usnat = new SpCampaign(CAMPAIGN_TYPE.USNAT, usnatParams);
    spCampaigns.Add(usnat);
```

In order to instantiate & trigger `Consent Message Web View`, you must call the `CMP.Initialize` function in `Awake` along with `spCampaigns`, `accountId`, `propertyId`, `propertyName`, `gdpr`, `ccpa`, `usnat`,`gdprPmId`, `ccpaPmId`, `usnatPmId` and `language`.<br/> <br/>Additionally, you can also specify a `messageTimeout` which, by default, is set to **30 seconds**.

```c#
    CMP.Initialize(spCampaigns: spCampaigns,
                   accountId: 22,
                   propertyId: 16893,
                   propertyName: "mobile.multicampaign.demo",
                   gdpr: true,
                   ccpa: true,
                   usnat: true,
                   language: MESSAGE_LANGUAGE.ENGLISH,
                   gdprPmId: "488393",
                   ccpaPmId: "509688",
                   usnatPmId: "943886",
                   campaignsEnvironment: CAMPAIGN_ENV.PUBLIC,
                   messageTimeoutInSeconds: 30,
                   transitionCCPAAuth: false,
                   supportLegacyUSPString: false);
```

| Field          | **Description**                                                                                                                                                           |
| -------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `spCampaigns`  | List with campaigns launched on the property through the Sourcepoint portal                                                                                               |
| `accountId`    | Organization's account ID found in the Sourcepoint portal                                                                                                                 |
| `propertyId`   | ID for property found in the Sourcepoint portal                                                                                                                           |
| `propertyName` | Name of property found in the Sourcepoint portal                                                                                                                          |
| `language`     | Force a message to be displayed in a certain language. Look `MESSAGE_LANGUAGE` for all available languages                                                                |
| `gdprPmId`     | Id of the privacy manager for `gdpr` campaign                                                                                                                             |
| `ccpaPmId`     | Id of the privacy manager for `ccpa` campaign                                                                                                                             |
| `usnatPmId`    | Id of the privacy manager for `usnat` campaign                                                                                                                            |
| `transitionCCPAAuth`     | Transfer opt-in/opt out preferences from U.S. Privacy (Legacy) to U.S. Multi-State Privacy [ios](https://github.com/SourcePointUSA/ios-cmp-app?tab=readme-ov-file#transfer-opt-inopt-out-preferences-from-us-privacy-legacy-to-us-multi-state-privacy), [android](https://github.com/SourcePointUSA/android-cmp-app?tab=readme-ov-file#transfer-opt-inopt-out-preferences-from-us-privacy-legacy-to-us-multi-state-privacy)    |
| `supportLegacyUSPString` | Support U.S. Privacy (Legacy) with U.S. Multi-State Privacy [ios](https://github.com/SourcePointUSA/ios-cmp-app?tab=readme-ov-file#support-us-privacy-legacy-with-us-multi-state-privacy)                                                                                                                          |

> **Note**: It may take a frame to initialize the CMP library, so we strongly recommend that you `Initialize` in `Awake` separately from `LoadMessage`. We recommend that you `LoadMessage` in `Start` (see example below).

When the SDK receives the `LoadMessage` call, it will instantiate a webview if the end-user needs to see a message. <br/><br/> If there is a consent profile associated with `authId`, the SDK will bring the consent data from the server, overwriting whatever was stored in the device.

```c#
private void Start()
{
     CMP.LoadMessage(authId: null); // or pass it a String if you wish to use authenticated consent
}
```

Utilize the following method if an end-user requests to have their data deleted:

```c#
private void ClearData()
{
     CMP.ClearAllData();
}
```

In order to free memory, call `Dispose` as illustrated in the following example :

```c#
private void OnDestroy()
{
    CMP.Dispose();
}
```

If you need library logs, you can use this:
```c#
CmpDebugUtil.EnableCmpLogs(true)
```

# Handle consent callbacks

Consent callbacks allow you to track progress and receive updates of user interaction. We provide the following interfaces:

| Callback               | Description                                                                                                              |
| ---------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| `IOnConsentUIReady`    | Triggered when web view UI is ready and about to show                                                                    |
| `IOnConsentAction`     | Triggered when user made an action, provides you instance of enum `CONSENT_ACTION_TYPE`. See below for more information. |
| `IOnConsentError`      | Triggered when something went wrong, provides you instance of Exception                                                  |
| `IOnConsentUIFinished` | Triggered when user interaction with web view UI is done and view is about to disappear                                  |
| `IOnConsentReady`      | Triggered when server successfully reacted to user's consent, provides you `SpConsent` object with consent info          |

# Custom Action, OnConsentAction

`IOnConsentAction` is triggered when the user takes an action in the first layer message or privacy manager, and provides you an instance of `SpAction` which contains:
1) The enumeration of type `CONSENT_ACTION_TYPE` which indicates the type of action:
```c#
public enum CONSENT_ACTION_TYPE
{
    SAVE_AND_EXIT = 1,
    PM_DISMISS = 2,
    CUSTOM_ACTION = 9,
    ACCEPT_ALL = 11,
    SHOW_OPTIONS = 12,
    REJECT_ALL = 13,
    MSG_CANCEL = 15,
}
```
2) `CustomActionId : String` - If the type of action is Custom, this attribute will contain the id you assigned to it when building the message in our message builder. In other cases the line is empty.
   
# Workflow to handle callbacks using interfaces

After you have created your own script which derives from `MonoBehaviour` and attached this component to your `GameObject` you should perform the following:

Inherit your script from any number of interfaces from the `IConsentEventHandler` list you are interested in and implement its method(s).

> Example<br>Suppose you want to handle exception callback via `IOnConsentErrorEventHandler`, and you already implemented `IOnConsentErrorEventHandler` inheritance and `OnConsentError` method in your script and attached this script to generic `GameObject` in hierarchy. What's next?

```c#
public class ConsentEventHandler : MonoBehaviour, IOnConsentError
{
    public void OnConsentError(Exception exception)
    {
        Debug.LogError("Oh no, an error! " + exception.Message);
    }
}
```

Register your `gameObject` (which implements any inheritor of `IConsentEventHandler` interface) as an event listener with `ConsentMessenger.AddListener` static method. It can be registered any time before you call the `LoadMessage` method (`Awake`, `Start` is enough, but you can adopt the registration to your own logic).

> In the example below, we have added the current `gameObject` as listener for `IOnConsentError` events.<br><br>The event will be executed on all components of the game object that can handle it, regardless of whether they are subscribed or not if at least one have registered the `gameObject` as a listener.

```c#
void Awake()
{
    ConsentMessenger.AddListener<IOnConsentError>(this.gameObject);
}
```

You should also unregister your listener when it becomes unnecessary due to garbage collection. `OnDestroy` is enough for our purposes:

```c#
private void OnDestroy()
{
    ConsentMessenger.RemoveListener<IOnConsentError>(this.gameObject);
}
```

The solution is ready. Configure it and deploy!

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

    public void OnConsentAction(SpAction action)
    {
        Debug.LogWarning($"User made action={action.Type} and customActionId={action.CustomActionId} action with consent view!");
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

Once a player has completed the consent flow, you might want to provide a way for them to resurface the privacy manager so they can see/manage their consents on an ongoing basis. To do this, we provide the `LoadPrivacyManager` method. The following code snippet will show a GDPR privacy manager with the default tab open.

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

# GetSpConsent

This getter is used to retrieve `SpConsents` data. After calling, it checks the platform (Android or iOS) and returns the `SPConsents` structure:

```c#
    SpConsents
        |-- gdpr?
        |   |-- applies: bool
        |   |-- consents: GdprConsent
        |       |-- uuid: String?
        |       |-- tcData: Map<String, String>
        |       |-- grants: Map<String, GDPRPurposeGrants>
        |       |-- euconsent: String
        |       |-- acceptedCategories: List<String>
        |       |-- consentStatus: ConsentStatus
        |       |-- googleConsentMode: SPGCMData?
        |           |-- adStorage: SPGCMData.Status?
        |           |-- analyticsStorage: SPGCMData.Status?
        |           |-- adUserData: SPGCMData.Status?
        |           |-- adPersonalization: SPGCMData.Status?
        |-- ccpa?
        |   |-- applies: bool
        |   |-- consents: CcpaConsent
        |       |-- uuid: String?
	    |       |-- rejectedCategories: List<String>
        |       |-- rejectedVendors: List<String>
        |       |-- status: String?
        |       |-- uspstring: String
        |       |-- childPmId: String
        |       |-- signedLspa: bool
        |       |-- consentStatus: ConsentStatus?
        |-- usnat?
            |-- applies: bool
            |-- consents: CcpaConsent
                |-- uuid: String?
                |-- applies: bool
	            |-- consentStrings: List<ConsentString>
                |-- vendors: List<Consentable> //[{id: String, consented: bool }]
                |-- categories: List<Consentable> //[{id: String, consented: bool }]
                |-- statuses: StatusesUsnat
                    |-- rejectedAny: bool?
                    |-- consentedToAll: bool?
                    |-- consentedToAny: bool?
                    |-- hasConsentData: bool?
                    |-- sellStatus: bool?
                    |-- shareStatus: bool?
                    |-- sensitiveDataStatus: bool?
                    |-- gpcStatus: bool?
```

This method may return null. Sample usage:

```c#
    CMP.GetSpConsents()
```

## Verifying end-user consent for a given vendor

### IAB Vendors
If the vendor you're interested in verifying consent for is part of the IAB group, you don't need to manually check for consent. Our SDK implements the Transparency Consent Framework (TCF) spec and IAB vendors know how to retrieve consent from the local storage. 

### Non-IAB vendors (aka custom vendors)
For vendors that are not part of the IAB, you can verify the user consented to the vendor with the following:
     
```c#
     consents = CMP.GetSpConsents();
     
     // for GDPR
     bool isMyGDPRVendorConsented = consents.gdpr.consents.grants["a_vendor_id"].vendorGrant;
     
     // for CCPA, notice vendors are "opted-in" by default
     bool isMyCCPAVendorRejected = consents.ccpa.consents.status == "rejectedAll" ||
                                       consents.ccpa.consents.rejectedVendors.Contains("a_vendor_id");
```

## Google Consent Mode

[Google Consent Mode 2.0](https://developers.google.com/tag-platform/security/concepts/consent-mode) ensures that Google vendors on your property comply with an end-user's consent choices for purposes (called consent checks) defined by Google. It is implemented via [Google Analytics for Firebase SDK](https://firebase.google.com/docs/analytics/unity/start).

### Update consent checks

Use Google's `setConsent` method to update the relevant consent checks when the appropriate purposes are consented to/rejected. Be advised that the `googleConsentMode` object in `GdprConsent` will only return values for Google consent checks that are mapped to a custom purpose within your vendor list. For all other Google consent checks, the response will be `null`.

## Adding or Removing custom consents

It's possible to programmatically consent the current user to a list of custom vendors, categories and legitimate interest categories with the method:

```c#
void CustomConsentGDPR(
    string[] vendors, 
    string[] categories, 
    string[] legIntCategories, 
    Action<GdprConsent> onSuccessDelegate)
```

The vendor grants will be re-generated, this time taking into consideration the list of vendors, categories and legitimate interest categories you pass as parameters. The method is asynchronous so you must pass a completion handler that will receive back an instance of `GdprConsent` in case of success or it'll call the delegate method `onError` in case of failure.

Using the same strategy for the custom consent, it's possible to programmatically delete the current user consent to a list of vendors, categories and legitimate interest categories by using the following method from the consent lib:

```c#
void DeleteCustomConsentGDPR(
    string[] vendors, 
    string[] categories, 
    string[] legIntCategories, 
    Action<GdprConsent> onSuccessDelegate)
```

It's important to notice, this methods are intended to be used for **custom** vendors and purposes only. For IAB vendors and purposes, it's still required to get consent via the consent message or privacy manager.

# Build for iOS

Since Unity Editor exports the pre-built project to Xcode on iOS build, there are several necessary steps to perform so you can compile your solution. They are implemented inside the `CMPPostProcessBuild` [PostProcessBuild] script. Supplement or modify it if it is needed. 

You also need to check lines `33`, `55` and `90` of the `CMPPostProcessBuild` script, the paths may differ. Check that the path of the file `SwiftBridge.swift` in the Unity project matches that specified in the `33` line and the `55` line in the exported Xcode project. You also need to check the path of the file `UnityPlugin-Bridging-Header.h` in the project exported to xcode with path on line `90`.

## Troubleshooting
### `ConsentViewController-Swift.h` file not found (IOS)
Every time you update the Unity SDK from an older to a newer version, you are likely to encounter the absence of the newer version of the native iOS SDK in your build machine's cocoapods cache.
The fix is:
Try to update cocoapods to the latest version (1.15.2 at the moment) on your build machine and execute the commands consequently:
1) pod update  
2) pod install 

In the terminal where the "Podfile" is situated (usually, it is situated in the folder where Unity exports the Xcode project). 
It will fetch the native ConsentViewController version specified in Podfile (X.Y.Z) in your build machine cache. 
It should be performed once, after this the build machine will have an X.Y.Z version of SDK in cocoapod's cache and consequent builds will go smoothly.
