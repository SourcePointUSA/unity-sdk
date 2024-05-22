//  Unity.mm
//  Created by Vilas Mane and Dmytro Fedko on 12/04/21.

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>
#import "ConsentViewController-Swift.h"
#import "UnityPlugin-Bridging-Header.h"

static SwiftBridge * swiftBridge = nil;
typedef void (*СallbackCharMessage) (const char*);

extern "C"
{
    void _setCallbackDefault (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackDefaultWithCallback:callback];
    }

    void _setCallbackOnConsentReady (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnConsentReadyWithCallback:callback];
    }

    void _setCallbackOnConsentUIReady (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnConsentUIReadyWithCallback:callback];
    }

    void _setCallbackOnConsentAction (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnConsentActionWithCallback:callback];
    }

    void _setCallbackOnConsentUIFinished (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnConsentUIFinishedWithCallback:callback];
    }

    void _setCallbackOnErrorCallback (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnErrorCallbackWithCallback:callback];
    }

    void _setCallbackOnSPFinished (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnSPFinishedWithCallback:callback];
    }

    void _setCallbackOnCustomConsent (СallbackCharMessage callback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackOnCustomConsentWithCallback:callback];
    }

    void _initLib()
    {
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
    }

    void _addTargetingParamForCampaignType(int campaignType, char* key, char* value)
    {
        [swiftBridge addTargetingParamWithCampaignType:campaignType key:[NSString stringWithFormat:@"%s", key] value:[NSString stringWithFormat:@"%s", value]];
    }

    void _setTransitionCCPAAuth(bool value)
    {
        [swiftBridge setTransitionCCPAAuthWithValue:value];
    }

    void _setSupportLegacyUSPString(bool value)
    {
        [swiftBridge setSupportLegacyUSPStringWithValue:value];
    }

    void _configLib(int accountId, int propertyId, char* propertyName, bool gdpr, bool ccpa, bool usnat, SPMessageLanguage language, char* gdprPmId, char* ccpaPmId, char* usnatPmId)
    {
        [swiftBridge configLibWithAccountId:accountId propertyId:propertyId propertyName:[NSString stringWithFormat:@"%s", propertyName] gdpr:gdpr ccpa:ccpa usnat:usnat language:language gdprPmId:[NSString stringWithFormat:@"%s", gdprPmId] ccpaPmId:[NSString stringWithFormat:@"%s", ccpaPmId] usnatPmId:[NSString stringWithFormat:@"%s", usnatPmId]];
    }

    void _loadMessage()
    {
        [swiftBridge loadMessage];
    }

    void _loadMessageWithAuthId(char * authId)
    {
        [swiftBridge loadMessageWithAuthId:[NSString stringWithFormat:@"%s", authId]];
    }

    void _loadGDPRPrivacyManager()
    {
        [swiftBridge onGDPRPrivacyManagerTap];
    }

    void _loadCCPAPrivacyManager()
    {
        [swiftBridge onCCPAPrivacyManagerTap];
    }

    void _loadUSNATPrivacyManager()
    {
        [swiftBridge onUSNATPrivacyManagerTap];
    }


    void _cleanConsent()
    {
        [swiftBridge onClearConsentTap];
    }

    void _customConsentGDPR()
    {
        [swiftBridge customConsentToGDPR];
    }

    void _deleteCustomConsentGDPR()
    {
        [swiftBridge deleteCustomConsentGDPR];
    }

    void _addVendor(char* vendor)
    {
        [swiftBridge addCustomVendorWithVendor:[NSString stringWithFormat:@"%s", vendor]];
    }

    void _addCategory(char* category)
    {
        [swiftBridge addCustomCategoryWithCategory:[NSString stringWithFormat:@"%s", category]];
    }

    void _addLegIntCategory(char* legIntCategory)
    {
        [swiftBridge addCustomLegIntCategoryWithLegIntCategory:[NSString stringWithFormat:@"%s", legIntCategory]];
    }

    void _clearCustomArrays()
    {
        [swiftBridge clearCustomArrays];
    }

    void _dispose()
    {
        [swiftBridge dispose];
    }
}
