//  Unity.mm
//  Created by Vilas Mane and Dmytro Fedko on 12/04/21.

#import <Foundation/Foundation.h>
#import "UnityController.h"
#import "ConsentViewController-Swift.h"
#import "UnityPlugin-Bridging-Header.h"

static UnityController * unityBridgePlugin = nil;
static SwiftBridge * swiftBridge = nil;
typedef void (*СallbackCharMessage) (const char*);

extern "C"
{
    void _setUnityCallback (const char * gameObjectName){
        /*if (unityBridgePlugin == nil)
            unityBridgePlugin = [[UnityController alloc] init];
        [unityBridgePlugin setUnityCallback: gameObjectName];*/
    }

    void _addTargetingParamForCampaignType(int campaignType, char * key, char * value)
    {
        /*[unityBridgePlugin addTargetingParamForCampaignType:campaignType :key :value];*/
    }

    void _consrtuctLib(int accountId, int propId, char* propName, int arrSize, int campaignTypes[], int campaignsEnvironment, long timeOutSeconds)
    {
        /*[unityBridgePlugin consrtuctLib:accountId _:propId _:propName _:arrSize _:campaignTypes _: campaignsEnvironment _: timeOutSeconds];*/
    }

    //updated
    void _setCallbackDefault (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackDefaultWithCallback:callback];
    }

    //updated
    void _setCallbackOnConsentReady (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnConsentReadyWithCallback:callback];
    }

    //updated
    void _setCallbackOnConsentUIReady (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnConsentUIReadyWithCallback:callback];
    }

    //updated
    void _setCallbackOnConsentAction (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnConsentActionWithCallback:callback];
    }

    //updated
    void _setCallbackOnConsentUIFinished (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnConsentUIFinishedWithCallback:callback];
    }

    //updated
    void _setCallbackOnErrorCallback (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnErrorCallbackWithCallback:callback];
    }

    //updated
    void _setCallbackOnSPFinished (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnSPFinishedWithCallback:callback];
    }
    //updated
    void _initLib()
    {
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
    }

    //updated
    void _configLib(int accountId, int propertyId, char* propertyName, bool gdpr, bool ccpa, SPMessageLanguage language, char* gdprPmId, char* ccpaPmId)
    {
        [swiftBridge configLibWithAccountId:accountId propertyId:propertyId propertyName:[NSString stringWithFormat:@"%s", propertyName] gdpr:gdpr ccpa:ccpa language:language gdprPmId:[NSString stringWithFormat:@"%s", gdprPmId] ccpaPmId:[NSString stringWithFormat:@"%s", ccpaPmId]];
    }

    //updated
    void _loadMessage(char * authId)
    {
        /*[unityBridgePlugin loadMessage:authId];*/
        [swiftBridge loadMessage];
    }

    void _setMessageLanguage(int langId)
    {
        //[unityBridgePlugin setMessageLanguage:langId];
    }

    //updated
    void _loadGDPRPrivacyManager()
    {
        //[unityBridgePlugin loadGDPRPrivacyManager:pmId _:tabId];
        [swiftBridge onGDPRPrivacyManagerTap];
    }

    //updated
    void _loadCCPAPrivacyManager()
    {
        //[unityBridgePlugin loadCCPAPrivacyManager:pmId _:tabId];
        [swiftBridge onCCPAPrivacyManagerTap];
    }

    void _cleanDict()
    {
        //[unityBridgePlugin cleanDict];
    }

    void _cleanArrays()
    {
        //[unityBridgePlugin cleanArrays];
    }
    
    void _customConsentGDPRWithVendors()
    {
        //[unityBridgePlugin customConsentGDPRWithVendors];
    }

    void _addVendor(char* vendor)
    {
        //[unityBridgePlugin addVendor:vendor];
    }

    void _addCategory(char* category)
    {
        //[unityBridgePlugin addCategory:category];
    }

    void _addLegIntCategory(char* legIntCategory)
    {
        //[unityBridgePlugin addLegIntCategory:legIntCategory];
    }
}
