//
//  SwiftBridge.swift
//  Unity-iPhone
//
//  Created by Dmytro Fedko on 23.10.23.
//

import Foundation
import UIKit

@objc public class SwiftBridge: NSObject {
    
    enum CAMPAIGN_TYPE: Int {
            case GDPR = 0
            case IOS14 = 1
            case CCPA = 2
        }
    
    enum VendorStatus: String {
        case Accepted
        case Rejected
        case Unknown
        
        init(fromBool bool: Bool?) {
            if bool == nil {
                self = .Unknown
            } else if bool == false {
                self = .Rejected
            } else {
                self = .Accepted
            }
        }
    }
    
    struct Config {
        
        let accountId, propertyId: Int
        let propertyName: String
        let gdpr, ccpa, att: Bool
        let language: SPMessageLanguage
        let gdprPmId, ccpaPmId: String?
        
        let myVendorId: String = ""
        let myPurposesId: [String] = []
    }
    
    var idfaStatus: SPIDFAStatus { SPIDFAStatus.current() }
    var myVendorAccepted: VendorStatus = .Unknown
    lazy var config = { Config(
        accountId: 22,
        propertyId: 16893,
        propertyName: "mobile.multicampaign.demo",
        gdpr: true,
        ccpa: true,
        att: true,
        language: .BrowserDefault,
        gdprPmId: "488393",
        ccpaPmId: "509688"
    )}()
    lazy var gdprTargetingParams: SPTargetingParams = [:]
    lazy var ccpaTargetingParams: SPTargetingParams = [:]
    
    lazy var consentManager: SPSDK = { SPConsentManager(
        accountId: config.accountId,
        propertyId: config.propertyId,
        // swiftlint:disable:next force_try
        propertyName: try! SPPropertyName(config.propertyName),
        campaigns: SPCampaigns(
            gdpr: config.gdpr ? SPCampaign(targetingParams: gdprTargetingParams, groupPmId: config.gdprPmId) : nil,
            ccpa: config.ccpa ? SPCampaign(targetingParams: ccpaTargetingParams, groupPmId: config.ccpaPmId) : nil,
            ios14: config.att ? SPCampaign() : nil
        ),
        language: config.language,
        delegate: self
    )}()
    let logger: OSLogger = OSLogger.standard

    public typealias СallbackCharMessage = @convention(c) (UnsafePointer<CChar>?) -> Void
    var callbackSystem: СallbackCharMessage = printChar
    var callbackDefault: СallbackCharMessage? = nil
    var callbackOnConsentReady: СallbackCharMessage? = nil
    var callbackOnConsentUIReady: СallbackCharMessage? = nil
    var callbackOnConsentAction: СallbackCharMessage? = nil
    var callbackOnConsentUIFinished: СallbackCharMessage? = nil
    var callbackOnErrorCallback: СallbackCharMessage? = nil
    var callbackOnSPFinished: СallbackCharMessage? = nil
    var callbackOnSPUIFinished: СallbackCharMessage? = nil

// MARK: - Bridge config
    @objc public func addTargetingParam(campaignType: Int, key: String, value: String){
        switch CAMPAIGN_TYPE(rawValue: campaignType) {
        case .GDPR: gdprTargetingParams[key]=value

        case .IOS14: break

        case .CCPA: ccpaTargetingParams[key]=value

        case .none:
            print("Incorrect campaignType on addTargetingParam")
        }
    }
    
    @objc public func configLib(
        accountId: Int,
        propertyId: Int,
        propertyName: String,
        gdpr: Bool,
        ccpa: Bool,
        language: SPMessageLanguage,
        gdprPmId: String,
        ccpaPmId: String) {
            self.config = { Config(
                accountId: accountId,
                propertyId: propertyId,
                propertyName: propertyName,
                gdpr: gdpr,
                ccpa: ccpa,
                att: true,
                language: language,
                gdprPmId: gdprPmId,
                ccpaPmId: ccpaPmId
            )}()
        }
 
// MARK: - Manage lib
    @objc public func loadMessage(authId: String? = nil) {
        print("PURE SWIFT loadMessage")
        consentManager.loadMessage(forAuthId: authId)
    }

    @objc public func onClearConsentTap() {
        SPConsentManager.clearAllData()
        myVendorAccepted = .Unknown
    }
    
    @objc public func onGDPRPrivacyManagerTap() {
        consentManager.loadGDPRPrivacyManager(withId: config.gdprPmId!)
    }
    
    @objc public func onCCPAPrivacyManagerTap() {
        consentManager.loadCCPAPrivacyManager(withId: config.ccpaPmId!)
    }
    
    //TO-DO
    func onAcceptMyVendorTap(_ sender: Any) {
        consentManager.customConsentGDPR(
            vendors: [config.myVendorId],
            categories: config.myPurposesId,
            legIntCategories: []) { consents in
                self.myVendorAccepted = VendorStatus(fromBool: consents.vendorGrants[self.config.myVendorId]?.granted)
            }
    }
    
    //TO-DO
    func onDeleteMyVendorTap(_ sender: Any) {
        consentManager.deleteCustomConsentGDPR(
            vendors: [config.myVendorId],
            categories: config.myPurposesId,
            legIntCategories: []) { consents in
                self.myVendorAccepted = VendorStatus(fromBool: consents.vendorGrants[self.config.myVendorId]?.granted)
            }
    }
}
    
// MARK: - SPDelegate implementation
extension SwiftBridge: SPDelegate {
    public func onSPUIReady(_ controller: UIViewController) {
        let top = UIApplication.shared.firstKeyWindow?.rootViewController
        controller.modalPresentationStyle = UIModalPresentationStyle.overFullScreen
        top?.present(controller, animated: true)
        logger.log("PURE SWIFT onSPUIReady")
        runCallback(callback: callbackOnConsentUIReady, arg: "onSPUIReady")
    }
    
    public func onAction(_ action: SPAction, from controller: UIViewController) {
        print("onAction:", action)
        logger.log("PURE SWIFT onAction")
        let responce: Dictionary<String, String> = [
            "type":String(action.type.rawValue),
            "customActionId":action.customActionId ?? ""
        ]
        var resp = "type:0"
        if let data = try? JSONEncoder().encode(responce) {
            resp = String(data: data, encoding: .utf8) ?? ""
        }
        runCallback(callback: callbackOnConsentAction, arg: resp)
    }
    
    public func onSPUIFinished(_ controller: UIViewController) {
        let top = UIApplication.shared.firstKeyWindow?.rootViewController
        top?.dismiss(animated: true)
        logger.log("PURE SWIFT onSPUIFinished")
        runCallback(callback: callbackOnSPUIFinished, arg: "onSPUIFinished")
    }
    
    public func onConsentReady(userData: SPUserData) {
        print("onConsentReady:", userData)
        myVendorAccepted = VendorStatus(
            fromBool: userData.gdpr?.consents?.vendorGrants[config.myVendorId]?.granted
        )
        logger.log("PURE SWIFT onConsentReady")
        runCallback(callback: callbackOnConsentReady, arg: userData.toJSON())
    }
    
    public func onSPFinished(userData: SPUserData) {
        logger.log("SDK DONE")
        logger.log("PURE SWIFT onSPFinished")
        runCallback(callback: callbackOnSPFinished, arg: userData.toJSON())
    }
    
    public func onError(error: SPError) {
        print("Something went wrong: ", error)
        logger.log("PURE SWIFT onError")
        runCallback(callback: callbackOnErrorCallback, arg: error.toJSON())
    }
}

// MARK: - Callback Set Up
extension SwiftBridge {
    @objc public func setCallbackDefault(callback: @escaping СallbackCharMessage) -> Void{
        print("setCallbackDefault")
        callbackDefault = callback
    }
    
    @objc public func setCallbackOnConsentReady(callback: @escaping СallbackCharMessage) -> Void{
        print("setCallbackOnConsentReady")
        callbackOnConsentReady = callback
    }
    
    @objc public func setCallbackOnConsentUIReady(callback: @escaping СallbackCharMessage) -> Void{
        print("setCallbackOnConsentUIReady")
        callbackOnConsentUIReady = callback
    }

    @objc public func setCallbackOnConsentAction(callback: @escaping СallbackCharMessage) -> Void{
        print("setCallbackOnConsentAction")
        callbackOnConsentAction = callback
    }
    
    @objc public func setCallbackOnConsentUIFinished(callback: @escaping СallbackCharMessage) -> Void{
        print("setCallbackOnConsentUIFinished")
        callbackOnConsentUIFinished = callback
    }
    
    @objc public func setCallbackOnErrorCallback(callback: @escaping СallbackCharMessage) -> Void{
        print("setCallbackOnErrorCallback")
        callbackOnErrorCallback = callback
    }
    
    @objc public func setCallbackOnSPFinished(callback: @escaping СallbackCharMessage) -> Void{
        print("setCallbackOnSPFinished")
        callbackOnSPFinished = callback
    }
    
    func runCallback(callback: СallbackCharMessage?, arg: String?) {
        if callback != nil {
            callback!(arg)
        }else{
            (callbackDefault ?? callbackSystem)("onError not set")
        }
    }
    
    public func print(_ items: Any..., separator: String = " ", terminator: String = "\n") {
        if logger.level == .debug {
            printLog(items)
        }
    }
}

// MARK: - Util
public func printLog(_ items: Any..., separator: String = " ", terminator: String = "\n") {
    Swift.print("SWIFT LOG:",items)
}

public func printChar(text: UnsafePointer<CChar>?) {
    let base: UnsafePointer<CChar> = UnsafePointer.init("0")
    print(String(cString: text ?? base))
}

extension UIApplication {
    var firstKeyWindow: UIWindow? {
        if #available(iOS 15.0, *) {
            return UIApplication
                .shared
                .connectedScenes
                .compactMap { ($0 as? UIWindowScene)?.keyWindow }
                .last
        } else {
            if #available(iOS 13.0, *) {
                return UIApplication
                    .shared
                    .connectedScenes
                    .flatMap { ($0 as? UIWindowScene)?.windows ?? [] }
                    .last { $0.isKeyWindow }
            }
        }
        return UIApplication.shared.windows.last { $0.isKeyWindow }
    }
}
