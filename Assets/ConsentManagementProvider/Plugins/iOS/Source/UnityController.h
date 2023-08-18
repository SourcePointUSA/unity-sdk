//  UnityController.h
//  Created by Dmytro Fedko on 31.05.2021.

#import <UIKit/UIKit.h>

@interface UnityController : UIViewController

-(void) setUnityCallback :(const char *)gameObjectName;

-(void) consrtuctLib : (int) accountId _:(int) propId _:(char*) propName _: (int) arrSize _: (int[]) campaignTypes _: (int) campaignsEnvironment _: (long) timeOutSeconds;

-(void) loadMessage : (char*) authId;

-(void) addTargetingParamForCampaignType : (int) campaignType : (char*) key : (char*) value;

-(void) setMessageLanguage : (int) langId;

-(void) loadGDPRPrivacyManager : (char *) pmId _: (int) tabId;

-(void) loadCCPAPrivacyManager : (char *) pmId _: (int) tabId;

-(void) cleanArrays;

-(void) cleanDict;

-(void) customConsentGDPRWithVendors;

-(void) addVendor:(char*) vendor;

-(void) addLegIntCategory:(char*) legIntCategory;

-(void) addCategory:(char*) category;

@end
