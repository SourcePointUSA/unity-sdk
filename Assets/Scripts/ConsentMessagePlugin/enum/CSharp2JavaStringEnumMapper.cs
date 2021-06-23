using System.Collections.Generic;
using ConsentMessagePlugin.Android;

namespace ConsentManagementProviderLib.Enum
{
    internal static class CSharp2JavaStringEnumMapper
    {
        static Dictionary<PRIVACY_MANAGER_TAB, string> privacyManagerTabToJavaEnumKey;
        static Dictionary<CAMPAIGN_ENV, string> campaignEnvToJavaEnumKey;
        static Dictionary<MESSAGE_LANGUAGE, string> messageLanguageToJavaKey;

        static CSharp2JavaStringEnumMapper()
        {
            InitializeMapping();
        }

        #region Getters
        public static string GetMessageLanguageKey(MESSAGE_LANGUAGE lang)
        {
            return messageLanguageToJavaKey[lang];
        }

        public static string GetCampaignEnvKey(CAMPAIGN_ENV environment)
        {
            return campaignEnvToJavaEnumKey[environment];
        }

        public static string GetPrivacyManagerTabKey(PRIVACY_MANAGER_TAB tab)
        {
            return privacyManagerTabToJavaEnumKey[tab];
        }
        #endregion

        #region Initializers
        private static  void InitializeMapping()
        {
            InitializePrivacyManagerTabMapping();
            InitializeCampaignEnvMapping();
            InitializeMessageLanguageMapping();
        }

        private static void InitializeMessageLanguageMapping()
        {
            messageLanguageToJavaKey = new Dictionary<MESSAGE_LANGUAGE, string>();
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.BULGARIAN, MESSAGE_LANGUAGE_STRING_KEY.BULGARIAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.CHINESE, MESSAGE_LANGUAGE_STRING_KEY.CHINESE);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.CROATIAN, MESSAGE_LANGUAGE_STRING_KEY.CROATIAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.CZECH, MESSAGE_LANGUAGE_STRING_KEY.CZECH);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.DANISH, MESSAGE_LANGUAGE_STRING_KEY.DANISH);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.DUTCH, MESSAGE_LANGUAGE_STRING_KEY.DUTCH);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ENGLISH, MESSAGE_LANGUAGE_STRING_KEY.ENGLISH);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ESTONIAN, MESSAGE_LANGUAGE_STRING_KEY.ESTONIAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.FINNISH, MESSAGE_LANGUAGE_STRING_KEY.FINNISH);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.FRENCH, MESSAGE_LANGUAGE_STRING_KEY.FRENCH);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.GAELIC, MESSAGE_LANGUAGE_STRING_KEY.GAELIC);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.GERMAN, MESSAGE_LANGUAGE_STRING_KEY.GERMAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.GREEK, MESSAGE_LANGUAGE_STRING_KEY.GREEK);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.HUNGARIAN, MESSAGE_LANGUAGE_STRING_KEY.HUNGARIAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ICELANDIC, MESSAGE_LANGUAGE_STRING_KEY.ICELANDIC);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ITALIAN, MESSAGE_LANGUAGE_STRING_KEY.ITALIAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.JAPANESE, MESSAGE_LANGUAGE_STRING_KEY.JAPANESE);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.LATVIAN, MESSAGE_LANGUAGE_STRING_KEY.LATVIAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.LITHUANIAN, MESSAGE_LANGUAGE_STRING_KEY.LITHUANIAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.NORWEGIAN, MESSAGE_LANGUAGE_STRING_KEY.NORWEGIAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.POLISH, MESSAGE_LANGUAGE_STRING_KEY.POLISH);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.PORTUGEESE, MESSAGE_LANGUAGE_STRING_KEY.PORTUGEESE);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ROMANIAN, MESSAGE_LANGUAGE_STRING_KEY.ROMANIAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.RUSSIAN, MESSAGE_LANGUAGE_STRING_KEY.RUSSIAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SERBIAN_CYRILLIC, MESSAGE_LANGUAGE_STRING_KEY.SERBIAN_CYRILLIC);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SERBIAN_LATIN, MESSAGE_LANGUAGE_STRING_KEY.SERBIAN_LATIN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SLOVAKIAN, MESSAGE_LANGUAGE_STRING_KEY.SLOVAKIAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SLOVENIAN, MESSAGE_LANGUAGE_STRING_KEY.SLOVENIAN);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SPANISH, MESSAGE_LANGUAGE_STRING_KEY.SPANISH);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SWEDISH, MESSAGE_LANGUAGE_STRING_KEY.SWEDISH);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.TURKISH, MESSAGE_LANGUAGE_STRING_KEY.TURKISH);
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.CATALAN, MESSAGE_LANGUAGE_STRING_KEY.CATALAN);
        }

        private static void InitializeCampaignEnvMapping()
        {
            campaignEnvToJavaEnumKey = new Dictionary<CAMPAIGN_ENV, string>();
            campaignEnvToJavaEnumKey.Add(CAMPAIGN_ENV.PUBLIC, CAMPAIGN_ENV_STRING_KEY.PUBLIC);
            campaignEnvToJavaEnumKey.Add(CAMPAIGN_ENV.STAGE, CAMPAIGN_ENV_STRING_KEY.STAGE);
        }

        private static void InitializePrivacyManagerTabMapping()
        {
            privacyManagerTabToJavaEnumKey = new Dictionary<PRIVACY_MANAGER_TAB, string>();
            privacyManagerTabToJavaEnumKey.Add(PRIVACY_MANAGER_TAB.DEFAULT, PRIVACY_MANAGER_TAB_STRING_KEY.DEFAULT);
            privacyManagerTabToJavaEnumKey.Add(PRIVACY_MANAGER_TAB.PURPOSES, PRIVACY_MANAGER_TAB_STRING_KEY.PURPOSES);
            privacyManagerTabToJavaEnumKey.Add(PRIVACY_MANAGER_TAB.VENDORS, PRIVACY_MANAGER_TAB_STRING_KEY.VENDORS);
            privacyManagerTabToJavaEnumKey.Add(PRIVACY_MANAGER_TAB.FEATURES, PRIVACY_MANAGER_TAB_STRING_KEY.FEATURES);
        }
        #endregion
    }
}