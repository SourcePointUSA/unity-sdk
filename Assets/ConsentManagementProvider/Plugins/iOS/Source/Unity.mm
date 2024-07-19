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
    NSString* _getString(char * value)
    {
        return [NSString stringWithFormat:@"%s", value];
    }
    
    char* _checkGetString(char * value)
    {
        NSLog(@"%s","Check string");
        if (value == NULL)
        {
            NSLog(@"%s","Checked string is NULL");
            return NULL;
        }
        char* res = (char*)malloc(strlen(value)+1);
        strcpy (res, value);
        NSLog(@"data = %s", value);
        return res;
    }

    void _setCallback (СallbackCharMessage callback, char* typeCallback){
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
        [swiftBridge setCallbackWithCallback:callback typeCallback:_getString(typeCallback)];
    }

    void _initLib()
    {
        if (swiftBridge == nil)
            swiftBridge = [[SwiftBridge alloc] init];
    }

    void _addTargetingParamForCampaignType(int campaignType, char* key, char* value)
    {
        [swiftBridge addTargetingParamWithCampaignType:campaignType key:_getString(key) value:_getString(value)];
    }

    void _setTransitionCCPAAuth(bool value)
    {
        [swiftBridge setTransitionCCPAAuthWithValue:value];
    }

    void _setSupportLegacyUSPString(bool value)
    {
        [swiftBridge setSupportLegacyUSPStringWithValue:value];
    }

    void _configLib(int accountId, int propertyId, char* propertyName, bool gdpr, bool ccpa, bool usnat, char* language, int messageTimeoutInSeconds)
    {
        [swiftBridge configLibWithAccountId:accountId propertyId:propertyId propertyName:_getString(propertyName) gdpr:gdpr ccpa:ccpa usnat:usnat language:_getString(language) messageTimeoutInSeconds:messageTimeoutInSeconds];
    }

    void _loadMessage()
    {
        [swiftBridge loadMessageWithAuthId:nil];
    }

    void _loadMessageWithAuthId(char * authId)
    {
        [swiftBridge loadMessageWithAuthId:_getString(authId)];
    }
    
    void _loadPrivacyManager(int campaignType, char* pmId, int tab)
    {
        [swiftBridge loadPrivacyManagerWithCampaignType:campaignType pmId:_getString(pmId) tab:(SPPrivacyManagerTab)tab];
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
        [swiftBridge addCustomVendorWithVendor:_getString(vendor)];
    }

    void _addCategory(char* category)
    {
        [swiftBridge addCustomCategoryWithCategory:_getString(category)];
    }

    void _addLegIntCategory(char* legIntCategory)
    {
        [swiftBridge addCustomLegIntCategoryWithLegIntCategory:_getString(legIntCategory)];
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
