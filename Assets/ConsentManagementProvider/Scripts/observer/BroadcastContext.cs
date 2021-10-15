using System.Collections.Generic;
using System.Diagnostics;
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
        if (CmpLocalizationMapper.userConsent != null)
        {
            var gdprConsent = new GdprConsent();
            gdprConsent.euconsent = CmpLocalizationMapper.userConsent.euconsent;
            gdprConsent.TCData = CmpLocalizationMapper.userConsent.TCData;
            // gdprConsent.uuid = CmpLocalizationMapper.userConsent.uuid; //TODO ??? 
            gdprConsent.grants = new Dictionary<string, SpVendorGrant>();
            foreach (var kv in CmpLocalizationMapper.userConsent.grants)
            {
                gdprConsent.grants[kv.Key] = new SpVendorGrant(kv.Value.purposeGrants);
            }
            SpConsents spConsents = new SpConsents(
                new SpGdprConsent(gdprConsent), 
                null //TODO
                );
            ConsentMessenger.Broadcast<IOnConsentReady>(spConsents);
            CmpLocalizationMapper.userConsent = null;
        }
    }
}