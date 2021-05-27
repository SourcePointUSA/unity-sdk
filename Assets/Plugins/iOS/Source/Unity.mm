//  Unity.mm
//  Created by Vilas on 12/04/21.
//  Modified by Dimas

#import <Foundation/Foundation.h>
#import "ViewController.h"

static ViewController *adsPlugin = nil;

extern "C"
{
    void _initializeSDKTest() {
        if (adsPlugin == nil)
            adsPlugin = [[ViewController alloc] init];
        [adsPlugin testUnity];
    }
    
    void _loadMessage(int accountId, int propertyId, char * propertyName, char * pmId) {
        /*
         TODO:
            targetingParams:targetingParameter
            campaignEnv: GDPRCampaignEnvPublic
         */
        if (adsPlugin == nil)
            adsPlugin = [[ViewController alloc] init];
        [adsPlugin loadMessage: accountId _:propertyId _:propertyName _:pmId];
    }
    
    void _setUnityCallback (const char * gameObjectName){//}, const char * methodName) {
        if (adsPlugin == nil)
            adsPlugin = [[ViewController alloc] init];
        [adsPlugin setUnityCallback: gameObjectName];//_:methodName];
    }
}
