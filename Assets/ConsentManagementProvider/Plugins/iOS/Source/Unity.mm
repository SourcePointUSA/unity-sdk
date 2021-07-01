//  Unity.mm
//  Created by Vilas Mane and Dmytro Fedko on 12/04/21.

#import <Foundation/Foundation.h>
#import "UnityController.h"

static UnityController * unityBridgePlugin = nil;

extern "C"
{
    void _setUnityCallback (const char * gameObjectName){
        if (unityBridgePlugin == nil)
            unityBridgePlugin = [[UnityController alloc] init];
        [unityBridgePlugin setUnityCallback: gameObjectName];
    }

    void _addTargetingParamForCampaignType(int campaignType, char * key, char * value)
    {
        [unityBridgePlugin addTargetingParamForCampaignType:campaignType :key :value];
    }

    void _consrtuctLib(int accountId, char* propName, int arrSize, int campaignTypes[], int campaignsEnvironment, long timeOutSeconds)
    {
        [unityBridgePlugin consrtuctLib:accountId _:propName _:arrSize _:campaignTypes _: campaignsEnvironment _: timeOutSeconds];
    }

    void _loadMessage(char * authId)
    {
        [unityBridgePlugin loadMessage:authId];
    }

    void _setMessageLanguage(int langId)
    {
        [unityBridgePlugin setMessageLanguage:langId];
    }

    void _loadGDPRPrivacyManager(char * pmId, int tabId)
    {
        [unityBridgePlugin loadGDPRPrivacyManager:pmId _:tabId];
    }

    void _loadCCPAPrivacyManager(char * pmId, int tabId)
    {
        [unityBridgePlugin loadCCPAPrivacyManager:pmId _:tabId];
    }

    void _cleanDict()
    {
        [unityBridgePlugin cleanDict];
    }

    void _cleanArrays()
    {
        [unityBridgePlugin cleanArrays];
    }
    
    void _customConsentGDPRWithVendors()
    {
        [unityBridgePlugin customConsentGDPRWithVendors];
    }

    void _addVendor(char* vendor)
    {
        [unityBridgePlugin addVendor:vendor];
    }

    void _addCategory(char* category)
    {
        [unityBridgePlugin addCategory:category];
    }

    void _addLegIntCategory(char* legIntCategory)
    {
        [unityBridgePlugin addLegIntCategory:legIntCategory];
    }
}
