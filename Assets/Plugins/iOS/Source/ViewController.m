//  ViewController.m
//  Created by Vilas on 12/04/21.
//  Modified by Dimas

#import "ViewController.h"
@import ConsentViewController;

@interface ViewController ()<GDPRConsentDelegate> {
    GDPRConsentViewController *cvc;
    NSString* goNameStr;
}
@property(nonatomic, copy) NSString* goNameStr;
@end

@implementation ViewController
@synthesize goNameStr;

- (void)viewDidLoad {
    [super viewDidLoad];
}

- (void) setUnityCallback:(const char *)gameObjectName {//_:(const char *)methodName {
    NSString* goString = [NSString stringWithFormat:@"%s", gameObjectName];
    goNameStr = goString;
//    strcat(goName,goMethodName);
    NSLog(@"RECEIVED FROM C#: %@", goNameStr);
}

- (void) testUnity {
    NSLog(@"Unity Test");
    GDPRPropertyName *propertyName = [[GDPRPropertyName alloc] init:@"tcfv2.mobile.webview" error:NULL];

    NSDictionary *targetingParameter = [NSDictionary dictionary];

    cvc = [[GDPRConsentViewController alloc]
           initWithAccountId: 22
           propertyId: 7639
           propertyName: propertyName
           PMId: @"122058"
           campaignEnv: GDPRCampaignEnvPublic
           targetingParams:targetingParameter
           consentDelegate: self];

    [cvc loadMessage];
}

- (void) loadMessage:(int) accountId _:(int) propertyId _:(char*) propName _:(char*) pmId {
    NSLog(@"Unity Load Message");
    NSString* nsPropName = [NSString stringWithFormat:@"%s", propName];
    GDPRPropertyName *propertyName = [[GDPRPropertyName alloc] init:nsPropName error:NULL];
    NSString* nsPmId = [NSString stringWithFormat:@"%s", pmId];

    NSDictionary *targetingParameter = [NSDictionary dictionary];
    /*
     TODO:
        targetingParams:targetingParameter
        campaignEnv: GDPRCampaignEnvPublic
     */
    NSLog(@"%i,%i,%@,%@",accountId, propertyId, nsPropName, nsPmId);
    cvc = [[GDPRConsentViewController alloc]
           initWithAccountId: accountId
           propertyId: propertyId
           propertyName: propertyName
           PMId: nsPmId
           campaignEnv: GDPRCampaignEnvPublic
           targetingParams:targetingParameter
           consentDelegate: self];

    [cvc loadMessage];
}

- (const char*)getGOName{
    return [self convertNSStringToChar:goNameStr];
}

- (const char*)convertNSStringToChar:(NSString*)str{
    const char *cString = [str UTF8String];
    return cString;
}

- (void)onConsentReadyWithGdprUUID:(NSString *)gdprUUID userConsent:(GDPRUserConsent *)userConsent {
    NSLog(@"ConsentUUID: %@", gdprUUID);
    NSLog(@"ConsentString: %@", userConsent.euconsent);
    for (id vendorId in userConsent.acceptedVendors) {
        NSLog(@"Consented to Vendor(id: %@)", vendorId);
    }
    for (id purposeId in userConsent.acceptedCategories) {
        NSLog(@"Consented to Purpose(id: %@)", purposeId);
    }
    //TODO: userConsent -> JSON
    UnitySendMessage([self getGOName], "OnConsentReady", [self convertNSStringToChar:gdprUUID]);
}

- (void)gdprConsentUIWillShow {
    UIViewController *top = [UIApplication sharedApplication].keyWindow.rootViewController;
    [top presentViewController:cvc animated:YES completion: nil];
    UnitySendMessage([self getGOName], "OnConsentUIReady", "gdprConsentUIWillShow from iOS!");
}

- (void)consentUIDidDisappear {
    [self dismissViewControllerAnimated:true completion:nil];
    UnitySendMessage([self getGOName], "OnConsentUIFinished", "consentUIDidDisappear from iOS!");
}

- (void)onErrorWithError:(GDPRConsentViewControllerError * _Nonnull)error {
    if(error != nil){
//        NSError* parsingError = nil;
//        NSData* dataJson = [NSJSONSerialization dataWithJSONObject:error options:0 error:&parsingError];
//        if (parsingError != nil) {
//                NSLog(@"Oh no! GDPRConsentViewControllerError got an error while parsing %@", parsingError);
//            }
//        else{
//            NSString* dataStr = [[NSString alloc] initWithData:dataJson encoding:NSUTF8StringEncoding];
//            UnitySendMessage([self getGOName], "OnErrorCallback", [dataStr cStringUsingEncoding:NSUTF8StringEncoding]);
//        }
        UnitySendMessage([self getGOName], "OnErrorCallback", [error.description cStringUsingEncoding:NSUTF8StringEncoding]);
    }
}

- (void)onAction:(GDPRAction * _Nonnull)action {
    NSInteger actionTypeId = ((NSInteger)action.type);
    NSString *tmp = [NSString stringWithFormat:@"%ld", actionTypeId];
    const char *str = [tmp UTF8String];
    UnitySendMessage([self getGOName], "OnConsentAction", str);
}

@end

/*
 /// called when there’s a consent Message to be shown or before the PM is shown
 - (void)consentUIWillShowWithMessage:(GDPRMessage * _Nonnull)message;
 /// called when the consent message is about to show
 - (void)messageWillShow;
 
 //WON'T DO:
 /// called when the privacy manager is about to show
 - (void)gdprPMWillShow;
 /// called when the privacy manager is closed
 - (void)gdprPMDidDisappear;
 /// called when the consent message is closed
 - (void)messageDidDisappear;
 */
