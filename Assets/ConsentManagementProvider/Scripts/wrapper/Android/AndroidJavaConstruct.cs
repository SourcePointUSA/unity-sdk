using ConsentManagementProviderLib.Enum;
using ConsentMessagePlugin.Android;
using UnityEngine;

namespace ConsentManagementProviderLib.Android
{
    internal class AndroidJavaConstruct 
    {
#pragma warning disable
        static readonly string androidPluginName = "com.sourcepoint.cmplibrary.creation.FactoryKt";
#pragma warning restore
        private AndroidJavaClass pluginFactoryClass;

        public AndroidJavaConstruct()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                this.pluginFactoryClass = new AndroidJavaClass(androidPluginName);
                CmpDebugUtil.Log("plugin class is OK");
            }
#endif
        }

        internal static AndroidJavaObject GetActivity()
        {
            AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            return activity;
        }

        internal AndroidJavaObject ConsrtuctLib(AndroidJavaObject spConfig, AndroidJavaObject activity, SpClientProxy spClient)
        {
            AndroidJavaObject lib = pluginFactoryClass.CallStatic<AndroidJavaObject>("makeConsentLib", spConfig, activity, spClient);
            CmpDebugUtil.Log("consentLib is OK");
            return lib;
        }

        internal AndroidJavaObject ConstructPrivacyManagerTab(PRIVACY_MANAGER_TAB tab)
        {
            AndroidJavaObject privacyManagerTabK = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.PMTab", CSharp2JavaStringEnumMapper.GetPrivacyManagerTabKey(tab), (int)tab);
            privacyManagerTabK.Set("key", CSharp2JavaStringEnumMapper.GetPrivacyManagerTabKey(tab));
            CmpDebugUtil.Log("PMTab is OK");
            return privacyManagerTabK;
        }

        internal AndroidJavaObject ConstructCampaign(AndroidJavaObject campaignType, AndroidJavaObject targetingParams, CAMPAIGN_TYPE campaignTypeForLog)
        {
            AndroidJavaObject campaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", campaignType, targetingParams);
            CmpDebugUtil.Log($"Campaign {campaignTypeForLog} is OK");
            return campaign;
        }

        internal AndroidJavaObject ConstructCampaignType(CAMPAIGN_TYPE campaignType)
        {
            AndroidJavaObject type = null;
            switch (campaignType)
            {
                case CAMPAIGN_TYPE.GDPR:
                    type = new AndroidJavaObject("com.sourcepoint.cmplibrary.exception.CampaignType", CAMPAIGN_TYPE_STRING_KEY.GDPR, (int)CAMPAIGN_TYPE_ANDROID.GDPR); //GDPR has ordinal 0 in Java enum!
                    break;
                case CAMPAIGN_TYPE.CCPA:
                    type = new AndroidJavaObject("com.sourcepoint.cmplibrary.exception.CampaignType", CAMPAIGN_TYPE_STRING_KEY.CCPA, (int)CAMPAIGN_TYPE_ANDROID.CCPA); //CCPA has ordinal 1 in Java enum!
                    break;
                default:
                    CmpDebugUtil.LogError("CampaignType is NULL. How did you get there?");
                    break;
            }
            CmpDebugUtil.Log($"CampaignType {campaignType} is OK");
            return type;
        }

        internal AndroidJavaObject ConstructMessageLanguage(MESSAGE_LANGUAGE lang)
        {
            AndroidJavaObject msgLang = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.MessageLanguage", CSharp2JavaStringEnumMapper.GetMessageLanguageKey(lang), (int)lang);
            msgLang.Set("value", CSharp2JavaStringEnumMapper.GetMessageLanguageKey(lang));
            CmpDebugUtil.Log("MessageLanguage is OK");
            return msgLang;
        }
        
        internal AndroidJavaObject ConstructSpConfig(int accountId, string propertyName, long messageTimeout, AndroidJavaObject language, CAMPAIGN_ENV campaignsEnvironment, AndroidJavaObject[] spCampaigns)
        {
            AndroidJavaObject spConfig;
            using (AndroidJavaObject SpConfigDataBuilderClass = new AndroidJavaObject("com.sourcepoint.cmplibrary.creation.SpConfigDataBuilder"))
            {
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addAccountId", accountId);
                CmpDebugUtil.Log("addAccountId is OK");
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addPropertyName", propertyName);
                CmpDebugUtil.Log("addPropertyName is OK");
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addMessageLanguage", language);
                CmpDebugUtil.Log("addMessageLanguage is OK");
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addMessageTimeout", messageTimeout);
                CmpDebugUtil.Log("addMessageTimeout is OK");
                AndroidJavaObject env = ConstructCampaignEnv(campaignsEnvironment);
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addCampaignsEnv", env);
                CmpDebugUtil.Log("addCampaignsEnv is OK");

                foreach (AndroidJavaObject camp in spCampaigns)
                {
                    SpConfigDataBuilderClass.Call<AndroidJavaObject>("addCampaign", camp);
                    CmpDebugUtil.Log("addCampaign is OK");
                }

                spConfig = SpConfigDataBuilderClass.Call<AndroidJavaObject>("build");
                CmpDebugUtil.Log("build() is OK");
            }
            CmpDebugUtil.Log("SpConfig is OK");
            return spConfig;
        }

        internal AndroidJavaObject ConstructTargetingParam(string key, string value)
        {
            AndroidJavaObject targetingParam = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.TargetingParam", key, value);
            CmpDebugUtil.Log("TargetingParam is OK");
            return targetingParam;
        }

        private AndroidJavaObject ConstructCampaignEnv(CAMPAIGN_ENV environment)
        {
            AndroidJavaObject campaignEnv = new AndroidJavaObject("com.sourcepoint.cmplibrary.data.network.util.CampaignsEnv", CSharp2JavaStringEnumMapper.GetCampaignEnvKey(environment), (int)environment);
            campaignEnv.Set("env", CSharp2JavaStringEnumMapper.GetCampaignEnvKey(environment));
            CmpDebugUtil.Log("campaignEnv is OK");
            return campaignEnv;
        }

        internal void Dispose()
        {
            this.pluginFactoryClass = null;
        }
    }
}