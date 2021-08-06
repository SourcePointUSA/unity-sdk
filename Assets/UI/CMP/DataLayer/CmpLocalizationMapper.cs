using System.Collections.Generic;

public static class CmpLocalizationMapper
{
    private static Dictionary<string, List<CmpUiElementModel>> elements;
    private static bool isInitialized = false;
    public static bool IsInitialized => isInitialized;

    static CmpLocalizationMapper()
    {
        //TODO: Network call -> json
        elements =  NativeUiJsonDeserializer.DeserializeNativePm(JSONSTUB.nativePM);
        isInitialized = true;
    }

    public static CmpUiElementModel GetCmpUiElement(string viewId, string uiElementId)
    {
        CmpUiElementModel result = null;
        foreach (var uiElement in elements[viewId])
        {
            if (uiElement.Id.Equals(uiElementId))
            {
                result = uiElement;
                break;
            }
        }
        return result;
    }
}