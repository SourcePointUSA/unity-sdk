﻿using UnityEngine;

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
                Util.Log("plugin class is OK");
            }
#endif
        }

        internal static AndroidJavaObject GetActivity()
        {
            AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            return activity;
        }

        internal AndroidJavaObject ConsrtuctLib(AndroidJavaObject[] campaigns, int accountId, string propertyName, AndroidJavaObject language, AndroidJavaObject activity, SpClientProxy spClient)
        {
            AndroidJavaObject spConfig = ConstructSpConfig(accountId, propertyName, language, campaigns);
            AndroidJavaObject lib = pluginFactoryClass.CallStatic<AndroidJavaObject>("makeConsentLib", spConfig, activity, spClient);
            Util.Log("consentLib is OK");
            return lib;
        }

        internal AndroidJavaObject ConstructPrivacyManagerTab(PRIVACY_MANAGER_TAB tab)
        {
            AndroidJavaObject privacyManagerTabK = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.PMTab");
            privacyManagerTabK.Set("key", CSharp2JavaStringEnumMapper.GetPrivacyManagerTabKey(tab));
            Util.Log("PMTab is OK");
            return privacyManagerTabK;
        }

        internal AndroidJavaObject ConstructCampaign(AndroidJavaObject campaignType)
        {
            AndroidJavaObject param = ConstructTargetingParam("location", "EU");
            AndroidJavaObject paramList = UnityUtils.ConvertArrayToList(new AndroidJavaObject[] { param });
            AndroidJavaObject campaign = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.SpCampaign", campaignType, paramList);
            Util.Log($"Campaign {campaignType} is OK");
            return campaign;
        }

        internal AndroidJavaObject ConstructCampaignType(CAMPAIGN_TYPE campaignType)
        {
            AndroidJavaObject type = null;
            switch (campaignType)
            {
                case CAMPAIGN_TYPE.GDPR:
                    type = new AndroidJavaObject("com.sourcepoint.cmplibrary.exception.CampaignType", CAMPAIGN_TYPE_STRING_KEY.GDPR, 0);
                    break;
                case CAMPAIGN_TYPE.CCPA:
                    type = new AndroidJavaObject("com.sourcepoint.cmplibrary.exception.CampaignType", CAMPAIGN_TYPE_STRING_KEY.CCPA, 0);
                    break;
                default:
                    Util.LogError("CampaignType is NULL. How did you get there?");
                    break;
            }
            Util.Log($"CampaignType {campaignType} is OK");
            return type;
        }

        internal AndroidJavaObject ConstructMessageLanguage(MESSAGE_LANGUAGE lang)
        {
            AndroidJavaObject msgLang = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.MessageLanguage");
            msgLang.Set("value", CSharp2JavaStringEnumMapper.GetMessageLanguageKey(lang));
            Util.Log("MessageLanguage is OK");
            return msgLang;
        }
        
        private AndroidJavaObject ConstructSpConfig(int accountId, string propertyName, AndroidJavaObject language, AndroidJavaObject[] spCampaigns)
        {
            AndroidJavaObject spConfig;
            using (AndroidJavaObject SpConfigDataBuilderClass = new AndroidJavaObject("com.sourcepoint.cmplibrary.creation.SpConfigDataBuilder"))
            {
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addAccountId", accountId);
                Util.Log("addAccountId is OK");
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addPropertyName", propertyName);
                Util.Log("addPropertyName is OK");
                SpConfigDataBuilderClass.Call<AndroidJavaObject>("addMessageLanguage", language);
                Util.Log("addMessageLanguage is OK");

                foreach (AndroidJavaObject camp in spCampaigns)
                {
                    SpConfigDataBuilderClass.Call<AndroidJavaObject>("addCampaign", camp);
                    Util.Log("addCampaign is OK");
                }

                spConfig = SpConfigDataBuilderClass.Call<AndroidJavaObject>("build");
                Util.Log("build() is OK");
            }
            Util.Log("SpConfig is OK");
            return spConfig;
        }

        private AndroidJavaObject ConstructTargetingParam(string key, string value)
        {
            AndroidJavaObject targetingParam = new AndroidJavaObject("com.sourcepoint.cmplibrary.model.exposed.TargetingParam", key, value);
            Util.Log("TargetingParam is OK");
            return targetingParam;
        }

        private AndroidJavaObject ConstructCampaignEnv(CAMPAIGN_ENV environment)
        {
            AndroidJavaObject campaignEnv = new AndroidJavaObject("com.sourcepoint.cmplibrary.data.network.util.CampaignEnv");
            campaignEnv.Set("value", CSharp2JavaStringEnumMapper.GetCampaignEnvKey(environment));
            Util.Log("campaignEnv is OK");
            return campaignEnv;
        }

        internal void Dispose()
        {
            this.pluginFactoryClass = null;
        }
    }
}