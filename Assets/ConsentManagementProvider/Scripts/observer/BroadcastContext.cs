using System.Collections.Generic;
using ConsentManagementProviderLib;

public static class BroadcastContext
{
    public static void BroadcastOnConsentUIReadyIfNeeded()
    {
        if (CmpHomeScrollController.isShown)
        {
            ConsentMessenger.Broadcast<IOnConsentUIReady>();
            CmpHomeScrollController.isShown = false;
        }
    }
    
    public static void BroadcastIOnConsentUIFinishedIfNeeded()
    {
        if (CmpHomeScrollController.isDestroyed)
        {
            ConsentMessenger.Broadcast<IOnConsentUIFinished>();
            CmpHomeScrollController.isDestroyed = false;
        }
    }

    public static void BroadcastIOnConsentErrorIfNeeded()
    {
        if (CmpLocalizationMapper.cmpException != null)
        {
            ConsentMessenger.Broadcast<IOnConsentError>(CmpLocalizationMapper.cmpException);
            CmpLocalizationMapper.cmpException = null;
        }
    }
    
    public static void BroadcastIOnConsentActionIfNeeded()
    {
        if (CmpLocalizationMapper.lastActionCode.HasValue)
        {
            ConsentMessenger.Broadcast<IOnConsentAction>(CmpLocalizationMapper.lastActionCode);
            CmpLocalizationMapper.lastActionCode = null;
        }
    }
        
    public static void BroadcastIOnConsentReadyIfNeeded()
    {
        if (!CmpCampaignPopupQueue.IsCampaignAvailable 
            && (CmpLocalizationMapper.IsGdprConsented || CmpLocalizationMapper.IsCcpaConsented)
            && (CmpLocalizationMapper.ccpaUserConsent != null || CmpLocalizationMapper.gdprUserConsent != null))
        {
            SpGdprConsent gdpr = null;
            SpCcpaConsent ccpa = null;
            if (CmpLocalizationMapper.IsGdprConsented &&
                CmpLocalizationMapper.gdprUserConsent != null &&
                CmpLocalizationMapper.gdprUserConsent.grants != null)
            {
                var gdprConsent = new GdprConsent();
                gdprConsent.euconsent = CmpLocalizationMapper.gdprUserConsent.euconsent;
                gdprConsent.TCData = CmpLocalizationMapper.gdprUserConsent.TCData;
                gdprConsent.uuid = CmpLocalizationMapper.gdprUserConsent.uuid;
                gdprConsent.grants = new Dictionary<string, SpVendorGrant>();
                foreach (var kv in CmpLocalizationMapper.gdprUserConsent.grants)
                {
                    gdprConsent.grants[kv.Key] = new SpVendorGrant(kv.Value.purposeGrants);
                }

                gdpr = new SpGdprConsent(gdprConsent);
                CmpLocalizationMapper.gdprUserConsent = null;
            }
            if (CmpLocalizationMapper.IsCcpaConsented &&
                CmpLocalizationMapper.ccpaUserConsent != null)
            {
                CcpaConsent ccpaConsent = new CcpaConsent(uuid: CmpLocalizationMapper.ccpaUserConsent.uuid,
                    status: CmpLocalizationMapper.ccpaUserConsent.status,
                    uspstring: CmpLocalizationMapper.ccpaUserConsent.uspstring,
                    rejectedVendors: CmpLocalizationMapper.ccpaUserConsent.rejectedVendors,
                    rejectedCategories: CmpLocalizationMapper.ccpaUserConsent.rejectedCategories);
                ccpa = new SpCcpaConsent(ccpaConsent);
                CmpLocalizationMapper.ccpaUserConsent = null;
            }
            ConsentMessenger.Broadcast<IOnConsentReady>(new SpConsents(gdpr, ccpa));
        }
    }
}