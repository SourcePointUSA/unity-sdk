using UnityEngine;

namespace GdprConsentLib
{
    internal class AndroidJavaConstruct 
    {
        static readonly string androidPluginName = "com.sourcepoint.cmplibrary.creation.FactoryKt";
        private AndroidJavaClass pluginFactoryClass;

        public AndroidJavaConstruct()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                this.pluginFactoryClass = new AndroidJavaClass(androidPluginName);
                DebugUtil.Log("plugin class is OK");
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
            DebugUtil.Log("consentLib is OK");
            return lib;
        }

        internal AndroidJavaObject ConstructPrivacyManagerTab(PRIVACY_MANAGER_TAB tab)
        {
            AndroidJavaObject privacyManagerTabK = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.PMTab", CSharp2JavaStringEnumMapper.GetPrivacyManagerTabKey(tab), (int)tab);
            privacyManagerTabK.Set("key", CSharp2JavaStringEnumMapper.GetPrivacyManagerTabKey(tab));
            DebugUtil.Log("PMTab is OK");
            return privacyManagerTabK;
        }

        internal AndroidJavaObject ConstructCampaign(AndroidJavaObject campaignType, AndroidJavaObject targetingParams, CAMPAIGN_TYPE campaignTypeForLog)
        {
            AndroidJavaObject campaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", campaignType, targetingParams);
            DebugUtil.Log($"Campaign {campaignTypeForLog} is OK");
            return campaign;
        }

        internal AndroidJavaObject ConstructCampaignType(CAMPAIGN_TYPE campaignType)
        {
            AndroidJavaObject type = null;
            switch (campaignType)
            {
                case CAMPAIGN_TYPE.GDPR:
                    type = new AndroidJavaObject("com.sourcepoint.cmplibrary.exception.CampaignType", CAMPAIGN_TYPE_STRING_KEY.GDPR, (int)campaignType);
                    break;
                case CAMPAIGN_TYPE.CCPA:
                    type = new AndroidJavaObject("com.sourcepoint.cmplibrary.exception.CampaignType", CAMPAIGN_TYPE_STRING_KEY.CCPA, (int)campaignType);
                    break;
                default:
                    DebugUtil.LogError("CampaignType is NULL. How did you get there?");
                    break;
            }
            DebugUtil.Log($"CampaignType {campaignType} is OK");
            return type;
        }

        internal AndroidJavaObject ConstructMessageLanguage(MESSAGE_LANGUAGE lang)
        {
            AndroidJavaObject msgLang = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.MessageLanguage", CSharp2JavaStringEnumMapper.GetMessageLanguageKey(lang), (int)lang);
            msgLang.Set("value", CSharp2JavaStringEnumMapper.GetMessageLanguageKey(lang));
            DebugUtil.Log("MessageLanguage is OK");
            return msgLang;
        }
        
        internal AndroidJavaObject ConstructSpConfig(int accountId, string propertyName, long messageTimeout, AndroidJavaObject language, AndroidJavaObject[] spCampaigns)
        {
            AndroidJavaObject spConfig;
            using (AndroidJavaObject SpConfigDataBuilderClass = new AndroidJavaObject("com.sourcepoint.cmplibrary.creation.SpConfigDataBuilder"))
            {
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addAccountId", accountId);
                DebugUtil.Log("addAccountId is OK");
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addPropertyName", propertyName);
                DebugUtil.Log("addPropertyName is OK");
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addMessageLanguage", language);
                DebugUtil.Log("addMessageLanguage is OK");
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addMessageTimeout", messageTimeout);
                DebugUtil.Log("addMessageTimeout is OK");

                foreach (AndroidJavaObject camp in spCampaigns)
                {
                    SpConfigDataBuilderClass.Call<AndroidJavaObject>("addCampaign", camp);
                    DebugUtil.Log("addCampaign is OK");
                }

                spConfig = SpConfigDataBuilderClass.Call<AndroidJavaObject>("build");
                DebugUtil.Log("build() is OK");
            }
            DebugUtil.Log("SpConfig is OK");
            return spConfig;
        }

        internal AndroidJavaObject ConstructTargetingParam(string key, string value)
        {
            AndroidJavaObject targetingParam = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.TargetingParam", key, value);
            DebugUtil.Log("TargetingParam is OK");
            return targetingParam;
        }

        private AndroidJavaObject ConstructCampaignEnv(CAMPAIGN_ENV environment)
        {
            AndroidJavaObject campaignEnv = new AndroidJavaObject("com.sourcepoint.cmplibrary.data.network.util.CampaignEnv", CSharp2JavaStringEnumMapper.GetCampaignEnvKey(environment), (int)environment);
            campaignEnv.Set("value", CSharp2JavaStringEnumMapper.GetCampaignEnvKey(environment));
            DebugUtil.Log("campaignEnv is OK");
            return campaignEnv;
        }

        internal void Dispose()
        {
            this.pluginFactoryClass = null;
        }
    }
}