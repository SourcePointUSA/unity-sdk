# Upgrade from 2.x.x to 3.0.0

## CMP facade
CMP class moved from using the static methods to using the static instance (singleton pattern). So you must replace `CMP` with `CMP.Instance` before usage of each method.

### Initialize
In order to instantiate & trigger `Consent Message Web View`, you must call the `CMP.Instance.Initialize`.

```diff
-    CMP.Initialize(spCampaigns: spCampaigns,
+    CMP.Instance.Initialize(spCampaigns: spCampaigns,
                   accountId: 22,
                   propertyId: 16893,
                   propertyName: "mobile.multicampaign.demo",
-                   gdpr: true,
-                   ccpa: true,
-                   usnat: true,
                   language: MESSAGE_LANGUAGE.ENGLISH,
-                   gdprPmId: "488393",
-                   ccpaPmId: "509688",
-                   usnatPmId: "943886",
                   campaignsEnvironment: CAMPAIGN_ENV.PUBLIC,
                   messageTimeoutInSeconds: 30,
-                   transitionCCPAAuth: false,
-                   supportLegacyUSPString: false);
```

| Field          | **Description**                                                                                                                                                           |
| -------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `gdprPmId`     | Used only in `LoadPrivacyManager`           |
| `ccpaPmId`     | Used only in `LoadPrivacyManager`           |
| `usnatPmId`    | Used only in `LoadPrivacyManager`           |
| `transitionCCPAAuth`     | Moved to `SPCampaign`         |
| `supportLegacyUSPString` | Moved to `SPCampaign`         |

## SPCampaign
`transitionCCPAAuth` and `supportLegacyUSPString` are now part of `SpCampaign`. Parameters work only for the `usnat` campaign.

``` c#
    SpCampaign usnat = new SpCampaign(campaignType: CAMPAIGN_TYPE.USNAT, targetingParams: usnatParams, transitionCCPAAuth: true, supportLegacyUSPString: true);
    spCampaigns.Add(usnat);
```

### GetSpConsent
This getter is used to retrieve `SpConsents` data. After calling, it checks the platform (Android or iOS) and returns the `SPConsents` object with the following structure:

```diff
    SpConsents
        |-- gdpr?
        |   |-- applies: bool
        |   |-- consents: GdprConsent
+       |       |-- applies: bool
        |       |-- uuid: String?
        |       |-- tcData: Map<String, Object>
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
+       |       |-- applies: bool
        |       |-- uuid: String?
        |       |-- rejectedCategories: List<String>
        |       |-- rejectedVendors: List<String>
        |       |-- status: String?
        |       |-- uspstring: String
        |       |-- childPmId: String
        |       |-- signedLspa: bool
        |       |-- consentStatus: ConsentStatus?
+       |       |-- GPPData: Dictionary<string, object>?
        |-- usnat?
            |-- applies: bool
            |-- consents: CcpaConsent
                |-- uuid: String?
                |-- applies: bool
                |-- consentStrings: List<ConsentString>
                |-- vendors: List<Consentable>      //[{id: String, consented: bool }]
                |-- categories: List<Consentable>   //[{id: String, consented: bool }]
                |-- statuses: StatusesUsnat
                    |-- rejectedAny: bool?
                    |-- consentedToAll: bool?
                    |-- consentedToAny: bool?
                    |-- hasConsentData: bool?
                    |-- sellStatus: bool?
                    |-- shareStatus: bool?
                    |-- sensitiveDataStatus: bool?
                    |-- gpcStatus: bool?
+               |-- GPPData: Dictionary<string, object>?
```
