using ConsentManagementProviderLib.Enum;
using ConsentMessagePlugin.Android;
using System.Collections.Generic;
using System.Linq;
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
            this.pluginFactoryClass = new AndroidJavaClass(androidPluginName);
            CmpDebugUtil.Log("plugin class is OK");
        }

        internal static AndroidJavaObject GetActivity()
        {
            AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            return activity;
        }

        internal AndroidJavaObject ConstructLib(AndroidJavaObject spConfig, AndroidJavaObject activity, SpClientProxy spClient)
        {
            AndroidJavaObject lib = pluginFactoryClass.CallStatic<AndroidJavaObject>("makeConsentLib", spConfig, activity, spClient);
            CmpDebugUtil.Log("consentLib is OK");
            return lib;
        }

        internal AndroidJavaObject ConstructPrivacyManagerTab(PRIVACY_MANAGER_TAB tab)
        {
            string enumName = CSharp2JavaStringEnumMapper.GetPrivacyManagerTab(tab);

            AndroidJavaClass pmTabClass = new AndroidJavaClass("com.sourcepoint.cmplibrary.model.PMTab");
            AndroidJavaObject privacyManagerTab = pmTabClass.GetStatic<AndroidJavaObject>(enumName);
            privacyManagerTab.Set("key", CSharp2JavaStringEnumMapper.GetPrivacyManagerTabKey(tab));

            CmpDebugUtil.Log("PMTab is OK");
            return privacyManagerTab;
        }

        internal AndroidJavaObject ConstructCampaign(AndroidJavaObject campaignType, AndroidJavaObject targetingParams, CAMPAIGN_TYPE campaignTypeForLog)
        {
            AndroidJavaObject campaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", campaignType, targetingParams);
            CmpDebugUtil.Log($"Campaign {campaignTypeForLog} is OK");
            return campaign;
        }

        internal AndroidJavaObject ConstructCampaign(AndroidJavaObject campaignType, AndroidJavaObject targetingParams, CAMPAIGN_TYPE campaignTypeForLog, bool transitionCCPAAuth = false, bool supportLegacyUSPString = false)
        {
            AndroidJavaObject[] configOptions = new AndroidJavaObject[2];
            AndroidJavaClass enumConfigOption = new AndroidJavaClass("com.sourcepoint.cmplibrary.creation.ConfigOption");
            if (transitionCCPAAuth==true)
            {
                AndroidJavaObject option = enumConfigOption.GetStatic<AndroidJavaObject>(CONFIG_OPTION_FULL_KEY.TRANSITION_CCPA_AUTH);
                configOptions.Append(option);
            }
            if (supportLegacyUSPString==true)
            {
                AndroidJavaObject option = enumConfigOption.GetStatic<AndroidJavaObject>(CONFIG_OPTION_FULL_KEY.SUPPORT_LEGACY_USPSTRING);
                configOptions.Append(option);
            }
            AndroidJavaObject configSet = CmpJavaToUnityUtils.ConvertArrayToSet(configOptions);
            AndroidJavaObject campaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", campaignType, targetingParams, configSet);
            CmpDebugUtil.Log($"Campaign {campaignTypeForLog} with configOptions is OK");
            return campaign;
        }

        internal AndroidJavaObject ConstructCampaignType(CAMPAIGN_TYPE campaignType)
        {
            AndroidJavaObject type = null;
            AndroidJavaClass enumClass = new AndroidJavaClass("com.sourcepoint.cmplibrary.exception.CampaignType");
            switch (campaignType)
            {
                case CAMPAIGN_TYPE.GDPR:
                    type = enumClass.GetStatic<AndroidJavaObject>("GDPR");
                    break;
                case CAMPAIGN_TYPE.CCPA:
                    type = enumClass.GetStatic<AndroidJavaObject>("CCPA");
                    break;
                case CAMPAIGN_TYPE.USNAT:
                    type = enumClass.GetStatic<AndroidJavaObject>("USNAT");
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
            string enumName = CSharp2JavaStringEnumMapper.GetMessageLanguageKey(lang);

            AndroidJavaClass messageLanguageClass = new AndroidJavaClass("com.sourcepoint.cmplibrary.model.MessageLanguage");
            AndroidJavaObject msgLang = messageLanguageClass.GetStatic<AndroidJavaObject>(CSharp2JavaStringEnumMapper.GetMessageFullLanguageKey(lang));
            msgLang.Set("value", CSharp2JavaStringEnumMapper.GetMessageLanguageKey(lang));

            CmpDebugUtil.Log("MessageLanguage is OK");
            return msgLang;
        }
        
        internal AndroidJavaObject ConstructSpConfig(int accountId, int propertyId, string propertyName, long messageTimeout, AndroidJavaObject language, CAMPAIGN_ENV campaignsEnvironment, AndroidJavaObject[] spCampaigns)
        {
            AndroidJavaObject spConfig;
            using (AndroidJavaObject SpConfigDataBuilderClass = new AndroidJavaObject("com.sourcepoint.cmplibrary.creation.SpConfigDataBuilder"))
            {
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addAccountId", accountId);
                CmpDebugUtil.Log("addAccountId is OK");
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addPropertyId", propertyId);
                CmpDebugUtil.Log("addPropertyId is OK");
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
                if (CmpDebugUtil.IsLogging)
                {
                    SpConfigDataBuilderClass.Call<AndroidJavaObject>("addLogger", new CmpAndroidLoggerProxy());
                    CmpDebugUtil.Log("addLogger is OK");
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
            string enumName = CSharp2JavaStringEnumMapper.GetCampaignEnvKey(environment);

            AndroidJavaClass campaignsEnvClass = new AndroidJavaClass("com.sourcepoint.cmplibrary.data.network.util.CampaignsEnv");
            AndroidJavaObject campaignEnv = campaignsEnvClass.GetStatic<AndroidJavaObject>(CSharp2JavaStringEnumMapper.GetCampaignEnvKey(environment));

            CmpDebugUtil.Log("campaignEnv is OK");
            return campaignEnv;
        }

        internal void Dispose() => 
            this.pluginFactoryClass = null;
    }
}