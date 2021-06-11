using System.Collections;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public enum MESSAGE_LANGUAGE 
    {
        BROWSER_DEFAULT = 0, // SPMessageLanguageBrowserDefault exist ios only
        ENGLISH = 1,
        BULGARIAN = 2,
        CATALAN = 3,
        CHINESE = 4,
        CROATIAN = 5,
        CZECH = 6,
        DANISH = 7,
        DUTCH = 8,
        ESTONIAN = 9,
        FINNISH = 10,
        FRENCH = 11,
        GAELIC = 12,
        GERMAN = 13,
        GREEK = 14,
        HUNGARIAN = 15,
        ICELANDIC = 16,
        ITALIAN = 17,
        JAPANESE = 18,
        LATVIAN = 19,
        LITHUANIAN = 20,
        NORWEGIAN = 21,
        POLISH = 22,
        PORTUGEESE = 23,
        ROMANIAN = 24,
        RUSSIAN = 25,
        SERBIAN_CYRILLIC = 26,
        SERBIAN_LATIN = 27,
        SLOVAKIAN = 28,
        SLOVENIAN = 29,
        SPANISH = 30,
        SWEDISH = 31,
        TURKISH = 32
    }

    internal static class MESSAGE_LANGUAGE_STRING_KEY
    {
        internal const string
        BULGARIAN="BG",
        CATALAN="CA",
        CHINESE="ZH",
        CROATIAN="HR",
        CZECH="CS",
        DANISH="DA",
        DUTCH="NL",
        ENGLISH="EN",
        ESTONIAN="ET",
        FINNISH="FI",
        FRENCH="FR",
        GAELIC="GD",
        GERMAN="DE",
        GREEK="EL",
        HUNGARIAN="HU",
        ICELANDIC="IS",
        ITALIAN="IT",
        JAPANESE="JA",
        LATVIAN="LV",
        LITHUANIAN="LT",
        NORWEGIAN="NO",
        POLISH="PL",
        PORTUGEESE="PT",
        ROMANIAN="RO",
        RUSSIAN="RU",
        SERBIAN_CYRILLIC="SR-CYRL",
        SERBIAN_LATIN="SR-LATN",
        SLOVAKIAN="SK",
        SLOVENIAN="SL",
        SPANISH="ES",
        SWEDISH="SV",
        TURKISH="TR";
    }
}