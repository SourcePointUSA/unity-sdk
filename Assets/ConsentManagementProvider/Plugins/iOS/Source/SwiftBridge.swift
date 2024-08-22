//
//  SwiftBridge.swift
//  Unity-iPhone
//
//  Created by Dmytro Fedko on 23.10.23.
//

import Foundation
import UIKit

@objc public class SwiftBridge: NSObject {
    public override init() {
        config = Config(
            transitionCCPAAuth: nil,
            supportLegacyUSPString: nil,
            vendors: [],
            categories: [],
            legIntCategories: []
        )
    }
    enum CAMPAIGN_TYPE: Int {
        case GDPR = 0
        case IOS14 = 1
        case CCPA = 2
        case USNAT = 3
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
        var transitionCCPAAuth, supportLegacyUSPString: Bool?
        var vendors: [String] = []
        var categories: [String] = []
        var legIntCategories: [String] = []
    }
    
    var idfaStatus: SPIDFAStatus { SPIDFAStatus.current() }
    var myVendorAccepted: VendorStatus = .Unknown
    var config: Config
    lazy var gdprTargetingParams: SPTargetingParams = [:]
    lazy var ccpaTargetingParams: SPTargetingParams = [:]
    lazy var usnatTargetingParams: SPTargetingParams = [:]
    
    var consentManager: SPSDK?
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
    var callbackOnCustomConsent: СallbackCharMessage? = nil
    let callbacks = Callbacks()

// MARK: - Bridge config
    @objc public func addTargetingParam(campaignType: Int, key: String, value: String){
        switch CAMPAIGN_TYPE(rawValue: campaignType) {
        case .GDPR: gdprTargetingParams[key]=value

        case .IOS14: break

        case .CCPA: ccpaTargetingParams[key]=value
            
        case .USNAT: usnatTargetingParams[key]=value

        case .none:
            print("Incorrect campaignType on addTargetingParam")
        }
    }
    
    @objc public func setTransitionCCPAAuth(value: Bool){
        print("transitionCCPAAuth set to "+String(value))
        config.transitionCCPAAuth = value
    }
    
    @objc public func setSupportLegacyUSPString(value: Bool){
        print("supportLegacyUSPString set to "+String(value))
        config.supportLegacyUSPString = value
    }

    @objc public func configLib(
        accountId: Int,
        propertyId: Int,
        propertyName: String,
        gdpr: Bool,
        ccpa: Bool,
        usnat: Bool,
        language: String,
        messageTimeoutInSeconds: Double) {
            guard let propName = try? SPPropertyName(propertyName) else {
                self.callbacks.RunCallback(callbackType: .OnErrorCallback, arg: "`propertyName` invalid!")
                return
            }
            self.consentManager = { SPConsentManager(
                accountId: accountId,
                propertyId: propertyId,
                propertyName: propName,
                campaigns: SPCampaigns(
                    gdpr: gdpr ? SPCampaign(targetingParams: gdprTargetingParams) : nil,
                    ccpa: ccpa ? SPCampaign(targetingParams: ccpaTargetingParams) : nil,
                    usnat: usnat ? SPCampaign(targetingParams: usnatTargetingParams, transitionCCPAAuth: config.transitionCCPAAuth, supportLegacyUSPString: config.supportLegacyUSPString) : nil,
                    ios14: SPCampaign()
                ),
                language: SPMessageLanguage.init(rawValue: language) ?? SPMessageLanguage.BrowserDefault,
                delegate: self
            )}()
            self.consentManager?.messageTimeoutInSeconds = messageTimeoutInSeconds
        }
    
    @objc public func addCustomVendor(vendor: String) {
        config.vendors.append(vendor)
    }

    @objc public func addCustomCategory(category: String) {
        config.categories.append(category)
    }

    @objc public func addCustomLegIntCategory(legIntCategory: String) {
        config.legIntCategories.append(legIntCategory)
    }
    
    @objc public func clearCustomArrays() {
        config.vendors = []
        config.categories = []
        config.legIntCategories = []
    }
    
    @objc public func dispose() {
        callbacks.CleanCallbacks()
    }

// MARK: - Manage lib
    @objc public func loadMessage(authId: String? = nil) {
        print("PURE SWIFT loadMessage with authId="+(authId ?? "nil"))
        (consentManager != nil) ?
            consentManager?.loadMessage(forAuthId: authId) :
            self.callbacks.RunCallback(callbackType: .OnErrorCallback, arg: "Library was not initialized correctly!")
    }
    
    @objc public func onClearConsentTap() {
        print("PURE SWIFT onClearConsentTap")
        SPConsentManager.clearAllData()
        myVendorAccepted = .Unknown
    }
    
    @objc public func loadPrivacyManager(campaignType: Int, pmId: String, tab: SPPrivacyManagerTab) {
        guard let consentManager = consentManager else {
            self.callbacks.RunCallback(callbackType: .OnErrorCallback, arg: "Library was not initialized correctly!")
            return
        }
        switch CAMPAIGN_TYPE(rawValue: campaignType) {
        case .GDPR: consentManager.loadGDPRPrivacyManager(withId: pmId, tab: tab)
        case .CCPA: consentManager.loadCCPAPrivacyManager(withId: pmId, tab: tab)
        case .USNAT: consentManager.loadUSNatPrivacyManager(withId: pmId, tab: tab)
        case .IOS14: break
        case .none: print("Incorrect campaignType on loadPrivacyManager")
        }
    }

    @objc public func customConsentToGDPR() {
        if let consentManager = consentManager {
            consentManager.customConsentGDPR(
                vendors: config.vendors,
                categories: config.categories,
                legIntCategories: config.legIntCategories){contents in
                    self.print(contents)
                    self.callbacks.RunCallback(callbackType: .OnCustomConsent, arg: contents.toJSON())
                }
        } else {
            self.callbacks.RunCallback(callbackType: .OnErrorCallback, arg: "Library was not initialized correctly!")
        }
    }

    @objc public func deleteCustomConsentGDPR() {
        if let consentManager = consentManager {
            consentManager.deleteCustomConsentGDPR(
                vendors: config.vendors,
                categories: config.categories,
                legIntCategories: config.legIntCategories){contents in
                    self.print(contents)
                    self.callbacks.RunCallback(callbackType: .OnCustomConsent, arg: contents.toJSON())
                }
        } else {
            self.callbacks.RunCallback(callbackType: .OnErrorCallback, arg: "Library was not initialized correctly!")
        }
    }

    @objc public func rejectAll(campaignType: Int) {
        guard let campaign = CAMPAIGN_TYPE(rawValue: campaignType) else {
            self.callbacks.RunCallback(callbackType: .OnErrorCallback, arg: "Wrong `campaignType` on `rejectAll` call!")
            return
        }
        if let consentManager = consentManager {
            consentManager.rejectAll(campaignType: convertBridgeCampaignToNative(campaign: campaign))
        } else {
            self.callbacks.RunCallback(callbackType: .OnErrorCallback, arg: "Library was not initialized correctly!")
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
        callbacks.RunCallback(callbackType: .OnConsentUIReady, arg: "onSPUIReady")
    }
    
    public func onAction(_ action: SPAction, from controller: UIViewController) {
        print("onAction:", action)
        logger.log("PURE SWIFT onAction")
        let responce: Dictionary<String, String> = [
            "type":String(action.type.rawValue),
            "customActionId":action.customActionId ?? ""
        ]
        var resp = ""
        if let data = try? JSONEncoder().encode(responce) {
            resp = String(data: data, encoding: .utf8) ?? ""
        }
        callbacks.RunCallback(callbackType: .OnConsentAction, arg: resp)
    }
    
    public func onSPUIFinished(_ controller: UIViewController) {
        UIApplication.shared.firstKeyWindow?.rootViewController?.dismiss(animated: true)
        logger.log("PURE SWIFT onSPUIFinished")
        callbacks.RunCallback(callbackType: .OnSPUIFinished, arg: "onSPUIFinished")
    }
    
    public func onConsentReady(userData: SPUserData) {
        print("onConsentReady:", userData)
        logger.log("PURE SWIFT onConsentReady")
        callbacks.RunCallback(callbackType: .OnConsentReady, arg: userData.toJSON())
    }
    
    public func onSPFinished(userData: SPUserData) {
        logger.log("SDK DONE")
        logger.log("PURE SWIFT onSPFinished")
        callbacks.RunCallback(callbackType: .OnSPFinished, arg: userData.toJSON())
    }
    
    public func onError(error: SPError) {
        printLog("Something went wrong: ", error)
        logger.log("PURE SWIFT onError")
        callbacks.RunCallback(callbackType: .OnErrorCallback, arg: error.toJSON())
    }
}

// MARK: - Callback Set Up
extension SwiftBridge {
    @objc public func setCallback(callback: @escaping СallbackCharMessage, typeCallback: String) -> Void{
        callbacks.AddCallback(callback: callback, callbackType: Callbacks.CallbackType.init(rawValue: typeCallback) ?? Callbacks.CallbackType.NotSet)
    }
    
    public func print(_ items: Any..., separator: String = " ", terminator: String = "\n") {
        if OSLogger.defaultLevel == .debug {
            printLog(items)
        }
    }
}

// MARK: - callbacks class
class Callbacks: NSObject {
    public typealias СallbackCharMessage = @convention(c) (UnsafePointer<CChar>?) -> Void

    enum CallbackType: String {
        case NotSet = "CallbackNotSet"
        case System = "System"
        case Default = "Default"
        case OnConsentReady = "OnConsentReady"
        case OnConsentUIReady = "OnConsentUIReady"
        case OnConsentAction = "OnConsentAction"
        case OnConsentUIFinished = "OnConsentUIFinished"
        case OnErrorCallback = "OnErrorCallback"
        case OnSPFinished = "OnSPFinished"
        case OnSPUIFinished = "OnSPUIFinished"
        case OnCustomConsent = "OnCustomConsent"
    }

    var Callbacks: [CallbackType:CallbackSwift] = [:]
    
    func AddCallback(callback: @escaping СallbackCharMessage, callbackType: CallbackType) {
        printLog("Add callback type: \(callbackType.rawValue)")
        Callbacks[callbackType] = CallbackSwift.init(callback: callback, callbackType: callbackType)
    }

    func RunCallback(callbackType: CallbackType, arg: String?) {
        printLog("Run callback type: \(callbackType.rawValue) Callback set up:\(String(Callbacks[callbackType] != nil))")
        (Callbacks[callbackType]?.callback ?? printChar)(arg ?? "")
    }
    
    func CleanCallbacks() {
        Callbacks = [:]
    }
}

public class CallbackSwift {
    var callback: Callbacks.СallbackCharMessage
    var callbackType: Callbacks.CallbackType
    
    init(
        callback: @escaping Callbacks.СallbackCharMessage = printChar,
        callbackType: Callbacks.CallbackType = .NotSet) {
        self.callback = callback
        self.callbackType = callbackType
    }
}

// MARK: - Util
internal func convertBridgeCampaignToNative(campaign: SwiftBridge.CAMPAIGN_TYPE) -> SPCampaignType {
    switch campaign {
    case .GDPR: return SPCampaignType.gdpr
    case .IOS14: return SPCampaignType.ios14
    case .CCPA: return SPCampaignType.ccpa
    case .USNAT: return SPCampaignType.usnat
    }
}

public func printLog(_ items: Any..., separator: String = " ", terminator: String = "\n") {
    Swift.print("CMP SWIFT LOG:",items)
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
