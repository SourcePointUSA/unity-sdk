//  ViewController.h
//  Created by Vilas on 12/04/21.
//  Modified by Dimas


#import <UIKit/UIKit.h>

@interface ViewController : UIViewController

-(void) testUnity;

-(void) setUnityCallback :(const char *)gameObjectName;// _:(const char *)methodName;

-(void) loadMessage :(int) accountId _:(int) propertyId _:(char*) propName _:(char*) pmId;

@end

