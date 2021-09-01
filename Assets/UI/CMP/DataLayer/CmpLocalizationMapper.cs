using System.Collections.Generic;

public static class CmpLocalizationMapper
{
    private static Dictionary<string, List<CmpUiElementModel>> elements;
    private static bool isInitialized = false;
    public static bool IsInitialized => isInitialized;

    public static List<CmpCategoryModel> categories;
    public static List<CmpSpecialPurposeModel> specialPurposes;
    public static List<CmpFeatureModel> features;
    public static List<CmpSpecialFeatureModel> specialFeatures;
    public static List<CmpVendorModel> vendors;
    public static List<CmpShortCategoryModel> shortCategories;
    
    static CmpLocalizationMapper()
    {
        //TODO: NetworkClient call -> json
        elements =  NativeUiJsonDeserializer.DeserializeNativePm(JSONSTUB.nativePM);
        NativeUiJsonDeserializer.DeserializeShortCategories(JSONSTUB.shortCategories, ref shortCategories);
        NativeUiJsonDeserializer.DeserializeExtraCall(JSONSTUB.extraCall, 
                                                    out List<CmpCategoryModel> categories,
                                                    out List<CmpSpecialPurposeModel> specialPurposes,
                                                    out List<CmpFeatureModel> features,
                                                    out List<CmpSpecialFeatureModel> specialFeatures,
                                                    out List<CmpVendorModel> vendors);
        CmpLocalizationMapper.categories = categories;
        CmpLocalizationMapper.specialPurposes = specialPurposes;
        CmpLocalizationMapper.features = features;
        CmpLocalizationMapper.specialFeatures = specialFeatures;
        CmpLocalizationMapper.vendors = vendors;
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