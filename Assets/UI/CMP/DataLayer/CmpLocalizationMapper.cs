using System.Collections.Generic;

public static class CmpLocalizationMapper
{
    private static Dictionary<string, List<CmpUiElementModel>> elements;
    private static bool isInitialized = false;
    public static bool IsInitialized => isInitialized;

    public static Dictionary<string, string> popupBgColors;
    public static List<CmpCategoryModel> categories;
    public static List<CmpSpecialPurposeModel> specialPurposes;
    public static List<CmpFeatureModel> features;
    public static List<CmpSpecialFeatureModel> specialFeatures;
    public static List<CmpVendorModel> vendors;
    public static List<CmpShortCategoryModel> shortCategories;
    
    static CmpLocalizationMapper()
    {
        //TODO: NetworkClient call -> json
        NetworkClient n = new NetworkClient();
        
        elements =  NativeUiJsonDeserializer.DeserializeNativePm(JSONSTUB.nativePM, ref popupBgColors);
        NativeUiJsonDeserializer.DeserializeShortCategories(JSONSTUB.shortCategories, ref shortCategories);
        NativeUiJsonDeserializer.DeserializeExtraCall(JSONSTUB.extraCall, 
                                                        ref categories,
                                                        ref specialPurposes,
                                                        ref features,
                                                        ref specialFeatures,
                                                        ref vendors);
        isInitialized = true;
    }

    public static CmpUiElementModel GetCmpUiElement(string viewId, string uiElementId)
    {
        CmpUiElementModel result = null;
        foreach (var uiElement in elements[viewId])
        {
            if (uiElement.id.Equals(uiElementId))
            {
                result = uiElement;
                break;
            }
        }
        return result;
    }
}