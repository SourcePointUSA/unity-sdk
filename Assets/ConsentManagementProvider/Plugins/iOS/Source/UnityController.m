//  UnityController.m
//  UnityFramework
//  Created by Wombat MBP 17 on 31.05.2021.

#import "UnityController.h"
@import ConsentViewController;

@interface UnityController ()<SPDelegate> {
    SPConsentManager * consentManager;
    NSString* goNameStr;
    NSMutableDictionary<NSString *, NSMutableDictionary<NSString *, NSString *> *> * params;
    NSMutableArray<NSString *> * vendorsArr;
    NSMutableArray<NSString *> * categoriesArr;
    NSMutableArray<NSString *> * legIntCategoriesArr;
}
@property(nonatomic, copy) NSString* goNameStr;
@end

@implementation UnityController
@synthesize goNameStr;

- (const char*)convertNSStringToChar:(NSString*)str{
    const char *cString = [str UTF8String];
    return cString;
}

- (NSString*)convertCharToNSString:(char*)str{
    return [NSString stringWithFormat:@"%s", str];
}

- (const char*)getGOName{
    return [self convertNSStringToChar:goNameStr];
}

- (SPError *) checkIfPropNameNotNull:(char*) propName {
    NSString* nsPropName = [NSString stringWithFormat:@"%s", propName];
    if(propName == nil || [nsPropName isEqualToString : @""]){
        SPError *myErr = [SPError errorWithDomain:@"SPConsentManager"
                                  code:100
                                  userInfo:@{
                                       NSLocalizedDescriptionKey:@"propertyName is null or empty!"
                         }];
        return myErr;
    }else return nil;
}

- (BOOL *) checkIfConsentManagerNotNull
{
    if(consentManager == nil)
    {
        SPError *myErr = [SPError errorWithDomain:@"SPConsentManager"
                                  code:100
                                  userInfo:@{
                                       NSLocalizedDescriptionKey:@"ConsentManager is null! You should initialize it first."
                         }];
        [self onErrorWithError: myErr];
        return false;
    }else{
        return true;
    }
}

- (void) setUnityCallback:(const char *)gameObjectName {
    NSString* goString = [NSString stringWithFormat:@"%s", gameObjectName];
    goNameStr = goString;
//    NSLog(@"RECEIVED FROM C#: %@", goNameStr);
    [self initDict];
    [self initArrays];
}

-(void)initDict {
    params = [[NSMutableDictionary<NSString *, NSMutableDictionary<NSString *, NSString *> *> alloc] init];
    for (int i=0; i<=2; i++) {
        NSNumber * campaignType = [NSNumber numberWithInt:i];
        [params setValue:[NSMutableDictionary dictionary] forKey:[campaignType stringValue]];
    }
//    NSLog(@"initDict => params %@", params);
}

-(void)cleanDict {
    [params removeAllObjects];
}

-(void)initArrays {
    vendorsArr = [[NSMutableArray<NSString *> alloc] init];
    categoriesArr = [[NSMutableArray<NSString *> alloc] init];
    legIntCategoriesArr = [[NSMutableArray<NSString *> alloc] init];
}

-(void)cleanArrays {
    [vendorsArr removeAllObjects];
    [categoriesArr removeAllObjects];
    [legIntCategoriesArr removeAllObjects];
}

- (void) addTargetingParamForCampaignType : (int) campaignType : (char*) key : (char*) value {
    NSString * nsVal = [self convertCharToNSString:value];
    NSString * nsKey = [self convertCharToNSString:key];
    NSNumber * nsCampaignType = [NSNumber numberWithInt:campaignType];
    
//    NSLog(@"campType: %@ K= %@ V= %@",nsCampaignType,nsKey,nsVal);
    NSMutableDictionary<NSString *, NSString *> * campaignParams = params[[nsCampaignType stringValue]];
    [campaignParams setObject:nsVal forKey:nsKey];
    
//    NSLog(@"campaignParams => %@", params);
}

-(void) consrtuctLib : (int) accountId _:(char*) propName _: (int) arrSize _: (int[]) campaignTypes _: (int[]) campaignEnvironments _: (long) timeOutSeconds 
{
    SPError * err = [self checkIfPropNameNotNull: propName];
    if(err != nil)
    {
        [self onErrorWithError:err];
    }else{
        NSString* nsPropName = [NSString stringWithFormat:@"%s", propName];
        
        @try
        {
            SPPropertyName *propertyName = [[SPPropertyName alloc] init:nsPropName error:NULL];
        
            SPCampaign * gdpr = nil;
            SPCampaign * ccpa = nil;
            SPCampaign * ios14 = nil;
            
            NSMutableDictionary<NSString *, NSString *> * gdprParams;
            NSMutableDictionary<NSString *, NSString *> * ccpaParams;
            NSMutableDictionary<NSString *, NSString *> * ios14Params;
            
            for (int index=0; index < arrSize; index++)
            {
                SPCampaignType type = campaignTypes[index];
//                NSLog(@"and type is... %ld", (long)type);
                
                NSNumber * nsCampaignType = [NSNumber numberWithInt:index];
                NSMutableDictionary<NSString *, NSString *> * campaignParams = params[[nsCampaignType stringValue]];
                
                SPCampaignEnv * env = campaignEnvironments[index];
                if(type == SPCampaignTypeGdpr)
                {
                    gdprParams = campaignParams;
                    if([gdprParams count] == 0){
                        [gdprParams setObject:@"" forKey:@""];
                    }
                    gdpr = [[SPCampaign alloc]
                            initWithEnvironment: env
                            targetingParams: gdprParams];
                }else if(type == SPCampaignTypeCcpa){
                    ccpaParams = campaignParams;
                    if([ccpaParams count] == 0){
                        [ccpaParams setObject:@"" forKey:@""];
                    }
                    ccpa = [[SPCampaign alloc]
                            initWithEnvironment: env
                            targetingParams: gdprParams];
                }else if(type == SPCampaignTypeIos14){
                    ios14Params = campaignParams;
                    if([ios14Params count] == 0){
                        [ios14Params setObject:@"" forKey:@""];
                    }
                    ios14 = [[SPCampaign alloc]
                            initWithEnvironment: env
                            targetingParams: gdprParams];
                }else{
                    SPError *myErr = [SPError errorWithDomain:@"SPConsentManager"
                                              code:100
                                              userInfo:@{
                                                   NSLocalizedDescriptionKey:@"Unknown SpCampaign!"
                                     }];
                    [self onErrorWithError:myErr];
                }
            }
            
//            NSLog(@"%@ ||| %@ ||| %@", gdpr.description, ccpa.description, ios14.description);
            
            SPCampaigns * campaigns = [[SPCampaigns alloc]
                                       initWithGdpr: gdpr
                                       ccpa: ccpa
                                       ios14: ios14];
            
            consentManager = [[SPConsentManager alloc]
                    initWithAccountId:accountId
                    propertyName: propertyName
                    campaigns: campaigns
                    delegate: self];
            consentManager.messageTimeoutInSeconds = timeOutSeconds;
        } @catch (SPError *exception) {
            [self onErrorWithError: exception];
        }
    }
}

-(void) loadMessage : (char*) authId
{
    if([self checkIfConsentManagerNotNull])
    {
        NSString * nsAuthId = [self convertCharToNSString: authId];
        if ([nsAuthId isEqual: @""] || authId == nil) {
            [consentManager loadMessageForAuthId: NULL];
//            NSLog(@"loadMessage");
        }else{
            [consentManager loadMessageForAuthId: nsAuthId];
//            NSLog(@"loadMessage with authId");
        }
    }
}

- (void) loadGDPRPrivacyManager : (char *) pmId _: (int) tabId{
    if([self checkIfConsentManagerNotNull]){
        NSString * nsPmId = [self convertCharToNSString:pmId];
        SPPrivacyManagerTab tab = tabId;
        [consentManager loadGDPRPrivacyManagerWithId:nsPmId tab:tab];
    }
}

- (void) loadCCPAPrivacyManager : (char *) pmId _: (int) tabId{
    if([self checkIfConsentManagerNotNull]){
        NSString * nsPmId = [self convertCharToNSString:pmId];
        SPPrivacyManagerTab tab = tabId;
        [consentManager loadCCPAPrivacyManagerWithId:nsPmId tab:tab];
    }
}

- (void) setMessageLanguage : (int) langId{
    if([self checkIfConsentManagerNotNull]){
        SPMessageLanguage lang = langId;
        consentManager.messageLanguage = lang;
//        NSLog(@"Message Lang is set %ld", (long)lang);
    }
}

-(void) addVendor:(char*) vendor{
    NSString * nsVendor = [self convertCharToNSString:vendor];
    [vendorsArr addObject:nsVendor];
}

-(void) addLegIntCategory:(char*) legIntCategory{
    NSString * nsLegIntCategory = [self convertCharToNSString:legIntCategory];
    [legIntCategoriesArr addObject:nsLegIntCategory];
}

-(void) addCategory:(char*) category{
    NSString * nsCategory = [self convertCharToNSString:category];
    [categoriesArr addObject:nsCategory];
}

- (void) customConsentGDPRWithVendors {
//    NSLog(@"%@, %@, %@", vendorsArr.description, categoriesArr.description, legIntCategoriesArr.description);
    [consentManager customConsentGDPRWithVendors:vendorsArr
                    categories:categoriesArr
                    legIntCategories:legIntCategoriesArr
                    handler:(void (^ _Nonnull)(SPGDPRConsent * _Nonnull)) ^(SPGDPRConsent * gdprUserConsent)
    {
//        NSLog(@"customConsentGDPRWithVendors");
        UnitySendMessage([self getGOName], "OnCustomConsentGDPRCallback", [self convertNSStringToChar:[gdprUserConsent toJSON]]);
    }];
}

- (void)onSPUIReady:(SPMessageViewController * _Nonnull)controller {
//    NSLog(@"onSPUIReady");
    UIViewController *top = [UIApplication sharedApplication].keyWindow.rootViewController;
    [top presentViewController: controller animated:YES completion: nil];
    UnitySendMessage([self getGOName], "OnConsentUIReady", "onSPUIReady from iOS!");
}

- (void)onAction:(SPAction * _Nonnull)action from:(SPMessageViewController * _Nonnull)controller {
//    NSLog(@"onAction");
    NSInteger actionTypeId = ((NSInteger)action.type);
    NSString *tmp = [NSString stringWithFormat:@"%ld", actionTypeId];
    const char *str = [tmp UTF8String];
    UnitySendMessage([self getGOName], "OnConsentAction", str);
}

- (void)onSPUIFinished:(SPMessageViewController * _Nonnull)controller {
//    NSLog(@"onSPUIFinished");
    UIViewController *top = [UIApplication sharedApplication].keyWindow.rootViewController;
    [top dismissViewControllerAnimated:true completion:nil];
}

- (void)onErrorWithError:(SPError * _Nonnull)error {
//    NSLog(@"onErrorWithError");
//    NSLog(@"Error desc : %@", error.description);
    if(error != nil){
        UnitySendMessage([self getGOName], "OnErrorCallback", [error.description cStringUsingEncoding:NSUTF8StringEncoding]);
    }
}

- (void)onConsentReadyWithUserData:(SPUserData *)userData {
//    NSLog(@"onConsentReady");
//    NSLog(@"%s", [self convertNSStringToChar:[userData toJSON]]);
    UnitySendMessage([self getGOName], "OnConsentReady", [self convertNSStringToChar:[userData toJSON]]);
}

@end
