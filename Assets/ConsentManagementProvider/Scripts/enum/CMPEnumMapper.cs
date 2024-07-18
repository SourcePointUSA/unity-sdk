using System.Collections.Generic;
using ConsentMessagePlugin.Android;

namespace ConsentManagementProvider.Enum
{
    internal static class CMPEnumMapper
    {
        static Dictionary<PRIVACY_MANAGER_TAB, string> privacyManagerTabToJavaEnumKey;
        static Dictionary<PRIVACY_MANAGER_TAB, string> privacyManagerTabToJavaEnum;
        static Dictionary<CAMPAIGN_ENV, string> campaignEnvToJavaEnumKey;
        static Dictionary<MESSAGE_LANGUAGE, string> messageLanguageToJavaKey;
        static Dictionary<MESSAGE_LANGUAGE, string> messageFullLanguageToJavaKey;

        static CMPEnumMapper()
        {
            InitializeMapping();
        }

        #region Getters
        public static string GetMessageLanguageKey(MESSAGE_LANGUAGE lang)
        {
            return messageLanguageToJavaKey[lang];
        }

        public static string GetMessageFullLanguageKey(MESSAGE_LANGUAGE lang)
        {
            return messageFullLanguageToJavaKey[lang];
        }

        public static string GetCampaignEnvKey(CAMPAIGN_ENV environment)
        {
            return campaignEnvToJavaEnumKey[environment];
        }

        public static string GetPrivacyManagerTabKey(PRIVACY_MANAGER_TAB tab)
        {
            return privacyManagerTabToJavaEnumKey[tab];
        }

        public static string GetPrivacyManagerTab(PRIVACY_MANAGER_TAB tab)
        {
            return privacyManagerTabToJavaEnum[tab];
        }
        #endregion

        #region Initializers
        private static  void InitializeMapping()
        {
            InitializePrivacyManagerTabMapping();
            InitializePrivacyManagerTabMappingKey();
            InitializeCampaignEnvMapping();
            InitializeMessageLanguageMapping();
            InitializeMessageFullLanguageMapping();
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
            messageLanguageToJavaKey.Add(MESSAGE_LANGUAGE.PORTUGUESE, MESSAGE_LANGUAGE_STRING_KEY.PORTUGUESE);
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

        private static void InitializeMessageFullLanguageMapping()
        {
            messageFullLanguageToJavaKey = new Dictionary<MESSAGE_LANGUAGE, string>();
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.BULGARIAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.BULGARIAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.CHINESE, MESSAGE_LANGUAGE_FULL_STRING_KEY.CHINESE);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.CROATIAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.CROATIAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.CZECH, MESSAGE_LANGUAGE_FULL_STRING_KEY.CZECH);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.DANISH, MESSAGE_LANGUAGE_FULL_STRING_KEY.DANISH);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.DUTCH, MESSAGE_LANGUAGE_FULL_STRING_KEY.DUTCH);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ENGLISH, MESSAGE_LANGUAGE_FULL_STRING_KEY.ENGLISH);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ESTONIAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.ESTONIAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.FINNISH, MESSAGE_LANGUAGE_FULL_STRING_KEY.FINNISH);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.FRENCH, MESSAGE_LANGUAGE_FULL_STRING_KEY.FRENCH);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.GAELIC, MESSAGE_LANGUAGE_FULL_STRING_KEY.GAELIC);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.GERMAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.GERMAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.GREEK, MESSAGE_LANGUAGE_FULL_STRING_KEY.GREEK);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.HUNGARIAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.HUNGARIAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ICELANDIC, MESSAGE_LANGUAGE_FULL_STRING_KEY.ICELANDIC);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ITALIAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.ITALIAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.JAPANESE, MESSAGE_LANGUAGE_FULL_STRING_KEY.JAPANESE);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.LATVIAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.LATVIAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.LITHUANIAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.LITHUANIAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.NORWEGIAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.NORWEGIAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.POLISH, MESSAGE_LANGUAGE_FULL_STRING_KEY.POLISH);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.PORTUGUESE, MESSAGE_LANGUAGE_FULL_STRING_KEY.PORTUGUESE);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.ROMANIAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.ROMANIAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.RUSSIAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.RUSSIAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SERBIAN_CYRILLIC, MESSAGE_LANGUAGE_FULL_STRING_KEY.SERBIAN_CYRILLIC);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SERBIAN_LATIN, MESSAGE_LANGUAGE_FULL_STRING_KEY.SERBIAN_LATIN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SLOVAKIAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.SLOVAKIAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SLOVENIAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.SLOVENIAN);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SPANISH, MESSAGE_LANGUAGE_FULL_STRING_KEY.SPANISH);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.SWEDISH, MESSAGE_LANGUAGE_FULL_STRING_KEY.SWEDISH);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.TURKISH, MESSAGE_LANGUAGE_FULL_STRING_KEY.TURKISH);
            messageFullLanguageToJavaKey.Add(MESSAGE_LANGUAGE.CATALAN, MESSAGE_LANGUAGE_FULL_STRING_KEY.CATALAN);
        }

        private static void InitializeCampaignEnvMapping()
        {
            campaignEnvToJavaEnumKey = new Dictionary<CAMPAIGN_ENV, string>();
            campaignEnvToJavaEnumKey.Add(CAMPAIGN_ENV.PUBLIC, CAMPAIGN_ENV_STRING_KEY.PUBLIC);
            campaignEnvToJavaEnumKey.Add(CAMPAIGN_ENV.STAGE, CAMPAIGN_ENV_STRING_KEY.STAGE);
        }

        private static void InitializePrivacyManagerTabMappingKey()
        {
            privacyManagerTabToJavaEnumKey = new Dictionary<PRIVACY_MANAGER_TAB, string>();
            privacyManagerTabToJavaEnumKey.Add(PRIVACY_MANAGER_TAB.DEFAULT, PRIVACY_MANAGER_TAB_STRING_KEY.DEFAULT);
            privacyManagerTabToJavaEnumKey.Add(PRIVACY_MANAGER_TAB.PURPOSES, PRIVACY_MANAGER_TAB_STRING_KEY.PURPOSES);
            privacyManagerTabToJavaEnumKey.Add(PRIVACY_MANAGER_TAB.VENDORS, PRIVACY_MANAGER_TAB_STRING_KEY.VENDORS);
            privacyManagerTabToJavaEnumKey.Add(PRIVACY_MANAGER_TAB.FEATURES, PRIVACY_MANAGER_TAB_STRING_KEY.FEATURES);
        }

        private static void InitializePrivacyManagerTabMapping()
        {
            privacyManagerTabToJavaEnum = new Dictionary<PRIVACY_MANAGER_TAB, string>();
            privacyManagerTabToJavaEnum.Add(PRIVACY_MANAGER_TAB.DEFAULT, PRIVACY_MANAGER_TAB_STRING.DEFAULT);
            privacyManagerTabToJavaEnum.Add(PRIVACY_MANAGER_TAB.PURPOSES, PRIVACY_MANAGER_TAB_STRING.PURPOSES);
            privacyManagerTabToJavaEnum.Add(PRIVACY_MANAGER_TAB.VENDORS, PRIVACY_MANAGER_TAB_STRING.VENDORS);
            privacyManagerTabToJavaEnum.Add(PRIVACY_MANAGER_TAB.FEATURES, PRIVACY_MANAGER_TAB_STRING.FEATURES);
        }
        #endregion
    }
}