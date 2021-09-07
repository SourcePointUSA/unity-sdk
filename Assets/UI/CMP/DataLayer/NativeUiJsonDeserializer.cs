using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using UnityEngine;

[assembly: InternalsVisibleTo("Tests")] //"Friend assembly" gives access to internals from this assembly

public static class NativeUiJsonDeserializer
{
    public static void DeserializeShortCategories(string json, ref List<CmpShortCategoryModel> shortCategories)
    {
        // TODO:
        // InvalidOperationException
        // System.Text.Json.JsonReaderException (JsonException)
        shortCategories ??= new List<CmpShortCategoryModel>();
        using (JsonDocument document = JsonDocument.Parse(json))
        {
            shortCategories.AddRange(DeserializeCollection<CmpShortCategoryModel>(document.RootElement, "categories"));
        }
    }

    public static void DeserializeExtraCall(string json, 
                                            ref List<CmpCategoryModel> categoryModels, 
                                            ref List<CmpSpecialPurposeModel> specialPurposeModels,
                                            ref List<CmpFeatureModel> featureModels,
                                            ref List<CmpSpecialFeatureModel> specialFeatureModels,
                                            ref List<CmpVendorModel> vendorModels)
    {
        // TODO: System.Text.Json.JsonReaderException (JsonException)
        categoryModels ??= new List<CmpCategoryModel>();
        specialPurposeModels ??= new List<CmpSpecialPurposeModel>();
        featureModels ??= new List<CmpFeatureModel>();
        specialFeatureModels ??= new List<CmpSpecialFeatureModel>();
        vendorModels ??= new List<CmpVendorModel>();
        using (JsonDocument document = JsonDocument.Parse(json))
        {
            JsonElement root = document.RootElement;
            //DeserializeCollection< ... >(root, "stacks");
            categoryModels.AddRange(DeserializeCollection<CmpCategoryModel>(root, "categories"));
            specialPurposeModels.AddRange(DeserializeCollection<CmpSpecialPurposeModel>(root, "specialPurposes"));
            featureModels.AddRange(DeserializeCollection<CmpFeatureModel>(root, "features"));
            specialFeatureModels.AddRange(DeserializeCollection<CmpSpecialFeatureModel>(root, "specialFeatures"));
            vendorModels.AddRange(DeserializeCollection<CmpVendorModel>(root, "vendors"));
        }
    }
    
    public static Dictionary<string, List<CmpUiElementModel>> DeserializeNativePm(string json, ref Dictionary<string, string> popupBgColors)
    {
        // TODO: System.Text.Json.JsonReaderException (JsonException)
        Dictionary<string, List<CmpUiElementModel>> result = new Dictionary<string, List<CmpUiElementModel>>();
        popupBgColors = new Dictionary<string, string>();
        using (JsonDocument document = JsonDocument.Parse(json))
        {
            JsonElement root = document.RootElement;
            // Console.WriteLine(root.GetProperty("settings").GetProperty("defaultLanguage").GetString());
            JsonElement children = root.GetProperty("children");    // TODO: System.Collections.Generic.KeyNotFoundException : The given key was not present in the dictionary.
            foreach (JsonElement view in children.EnumerateArray())
            {
                if(view.ValueKind == JsonValueKind.Null) continue;
                
                string viewIdStr = null;
                if (view.TryGetProperty("id", out JsonElement viewId))
                {
                    // Console.WriteLine("\n>>>" + viewId.GetString());
                    viewIdStr = viewId.GetString();
                    if (!string.IsNullOrEmpty(viewIdStr))
                    {
                        result.Add(viewIdStr, new List<CmpUiElementModel>());
                    }
                    else
                    {
                        Debug.LogError(">>>DAFUQ >:C");
                        continue;
                    }
                }

                if(view.TryGetProperty("settings", out JsonElement viewSettings)
                   && viewSettings.TryGetProperty("style", out JsonElement viewStyle)
                   && viewStyle.TryGetProperty("backgroundColor", out JsonElement viewBackgroundColor)
                   && viewBackgroundColor.ValueKind != JsonValueKind.Null)
                {
                    popupBgColors[viewIdStr] = viewBackgroundColor.GetString();
                }

                JsonElement viewChildren = view.GetProperty("children"); // TODO: System.Collections.Generic.KeyNotFoundException : The given key was not present in the dictionary.
                foreach (JsonElement viewElement in viewChildren.EnumerateArray())
                {
                    if (viewElement.ValueKind == JsonValueKind.Null)
                        continue;
                    viewElement.TryGetProperty("id", out JsonElement id);
                    viewElement.TryGetProperty("type", out JsonElement type);
                    if (id.ValueKind == JsonValueKind.Null || type.ValueKind == JsonValueKind.Null) continue;
                    if (type.GetString() != null && id.GetString() != null && viewIdStr != null)
                        switch (type.GetString())
                        {
                            case "NativeText":
                                result[viewIdStr].Add(JsonSerializer.Deserialize<CmpTextModel>(viewElement.GetRawText()));
                                break;
                            case "Slider":
                                result[viewIdStr].Add(JsonSerializer.Deserialize<CmpSliderModel>(viewElement.GetRawText()));
                                break;
                            case "NativeImage":
                                result[viewIdStr].Add(JsonSerializer.Deserialize<CmpNativeImageModel>(viewElement.GetRawText()));
                                break;
                            case "LongButton":
                                result[viewIdStr].Add(JsonSerializer.Deserialize<CmpLongButtonModel>(viewElement.GetRawText()));
                                break;
                            case "NativeButton":
                                if (id.GetString().Equals("BackButton"))
                                {
                                    result[viewIdStr].Add(JsonSerializer.Deserialize<CmpBackButtonModel>(viewElement.GetRawText()));
                                }
                                else
                                {
                                    result[viewIdStr].Add(JsonSerializer.Deserialize<CmpNativeButtonModel>(viewElement.GetRawText()));
                                }
                                break;
                            case "CookieTable":
                                /*
                                 viewElement.TryGetProperty("settings", out JsonElement cookieTableSettings)
                                 && cookieTableSettings.TryGetProperty("nameText", out JsonElement cookieName)
                                 && cookieTableSettings.TryGetProperty("categoryText", out JsonElement categoryText)
                                 && cookieTableSettings.TryGetProperty("domainText", out JsonElement domainText)
                                 && cookieTableSettings.TryGetProperty("durationText", out JsonElement durationText)
                                 */
                                break;
                            default:
                                Debug.LogError(">>>DAFUQ >:C " + id.GetString());
                                break;
                        }
                }
            }
        }
        return result;
    }
    
    internal static List<T> DeserializeCollection<T>(JsonElement root, string propertyName)
    {
        List<T> result = new List<T>();
        if (root.TryGetProperty(propertyName, out JsonElement collection))
        {
            foreach (JsonElement collectionElement in collection.EnumerateArray())
            {
                if (collectionElement.ValueKind == JsonValueKind.Null || collectionElement.ValueKind != JsonValueKind.Object)  continue;
                T deserialized = JsonSerializer.Deserialize<T>(collectionElement.GetRawText());
                result.Add(deserialized);
            }
        }
        return result;
    }
}