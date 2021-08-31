using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

public static class NativeUiJsonDeserializer
{
    public static Dictionary<string, string> popupBgColors;

    public static void DeserializeShortCategories(string json, ref List<CmpShortCategoryModel> shortCategories)
    {
        using (JsonDocument document = JsonDocument.Parse(json))
        {
            JsonElement root = document.RootElement;
            if(shortCategories == null)
                shortCategories = new List<CmpShortCategoryModel>();
            if (root.TryGetProperty("categories", out JsonElement categories))
            {
                foreach (JsonElement cat in categories.EnumerateArray())
                {
                    CmpShortCategoryModel category = JsonSerializer.Deserialize<CmpShortCategoryModel>(cat.GetRawText());
                    shortCategories.Add(category);
                }
            }
        }
    }
    
    public static void DeserializeExtraCall(string json, 
                                            out List<CmpCategoryModel> categoryModels, 
                                            out List<CmpSpecialPurposeModel> specialPurposeModels,
                                            out List<CmpFeatureModel> featureModels,
                                            out List<CmpSpecialFeatureModel> specialFeatureModels,
                                            out List<CmpVendorModel> vendorModels)
    {
        using (JsonDocument document = JsonDocument.Parse(json))
        {
            JsonElement root = document.RootElement;
            categoryModels = new List<CmpCategoryModel>();
            specialPurposeModels = new List<CmpSpecialPurposeModel>();
            featureModels = new List<CmpFeatureModel>();
            specialFeatureModels = new List<CmpSpecialFeatureModel>();
            vendorModels = new List<CmpVendorModel>();

            if (root.TryGetProperty("stacks", out JsonElement stacks))
            {
                foreach (JsonElement stack in stacks.EnumerateArray())
                {
                    //TODO: Deserialize stack ...
                }
            }
            if (root.TryGetProperty("categories", out JsonElement categories))
            {
                foreach (JsonElement category in categories.EnumerateArray())
                {
                    CmpCategoryModel cat = JsonSerializer.Deserialize<CmpCategoryModel>(category.GetRawText());
                    categoryModels.Add(cat);
                }
            }
            if (root.TryGetProperty("specialPurposes", out JsonElement specialPurposes))
            {
                foreach (JsonElement specElement in specialPurposes.EnumerateArray())
                {
                    CmpSpecialPurposeModel specPurp = JsonSerializer.Deserialize<CmpSpecialPurposeModel>(specElement.GetRawText());
                    specialPurposeModels.Add(specPurp);
                }
            }
            if (root.TryGetProperty("features", out JsonElement features))
            {
                foreach (JsonElement feature in features.EnumerateArray())
                {
                    CmpFeatureModel feat = JsonSerializer.Deserialize<CmpFeatureModel>(feature.GetRawText());
                    featureModels.Add(feat);
                }
            }
            if (root.TryGetProperty("specialFeatures", out JsonElement specialFeatures))
            {
                foreach (JsonElement specialFeature in specialFeatures.EnumerateArray())
                {
                    CmpSpecialFeatureModel specFeat = JsonSerializer.Deserialize<CmpSpecialFeatureModel>(specialFeature.GetRawText());
                    specialFeatureModels.Add(specFeat);
                }
            }
            if (root.TryGetProperty("vendors", out JsonElement vendors))
            {
                foreach (JsonElement vendor in vendors.EnumerateArray())
                {
                    CmpVendorModel vendr = JsonSerializer.Deserialize<CmpVendorModel>(vendor.GetRawText());
                    vendorModels.Add(vendr);
                }
            }
        }
    }

    public static Dictionary<string, List<CmpUiElementModel>> DeserializeNativePm(string json)
    {
        Dictionary<string, List<CmpUiElementModel>> result = new Dictionary<string, List<CmpUiElementModel>>();
        popupBgColors = new Dictionary<string, string>();
        using (JsonDocument document = JsonDocument.Parse(json))
        {
            JsonElement root = document.RootElement;
            // Console.WriteLine(root.GetProperty("settings").GetProperty("defaultLanguage").GetString());
            JsonElement children = root.GetProperty("children");
            foreach (JsonElement view in children.EnumerateArray())
            {
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
                    )
                {
                    popupBgColors[viewIdStr] = viewBackgroundColor.GetString();
                }

                JsonElement viewChildren = view.GetProperty("children");
                foreach (JsonElement viewElement in viewChildren.EnumerateArray())
                {
                    viewElement.TryGetProperty("id", out JsonElement id);
                    viewElement.TryGetProperty("type", out JsonElement type);
                    viewElement.TryGetProperty("name", out JsonElement name);

                    if (viewElement.TryGetProperty("settings", out JsonElement settings)
                        && viewElement.TryGetProperty("type", out JsonElement nativeTextType)
                        && nativeTextType.GetString() != null
                        && nativeTextType.GetString().Equals("NativeText")
                        && settings.TryGetProperty("text", out JsonElement text))
                    {
                        // if (!String.IsNullOrEmpty(text.GetString()))
                        // Console.WriteLine(id.GetString() + " | " + text.GetString());
                        ColoredFontModel font = null;
                        if (settings.TryGetProperty("style", out JsonElement nativeTextstyle)
                            && nativeTextstyle.TryGetProperty("font", out JsonElement nativeTextFont))
                        {
                            font = JsonSerializer.Deserialize<ColoredFontModel>(nativeTextFont.GetRawText());
                        }
                        result[viewIdStr].Add(new CmpTextModel(id.GetString(), type.GetString(), name.GetString(), font,
                            text.GetString()));
                    }
                    else if (viewElement.TryGetProperty("type", out JsonElement sliderType)
                             && sliderType.GetString() != null
                             && sliderType.GetString().Equals("Slider")
                             && viewElement.TryGetProperty("settings", out JsonElement sliderSettings)
                             && sliderSettings.TryGetProperty("leftText", out JsonElement leftText)
                             && sliderSettings.TryGetProperty("rightText", out JsonElement rightText)
                             && sliderSettings.TryGetProperty("style", out JsonElement sliderStyle)
                             && sliderStyle.TryGetProperty("backgroundColor", out JsonElement sliderBgColor)
                             && sliderStyle.TryGetProperty("activeBackgroundColor", out JsonElement sliderActiveBgColor)
                             && sliderStyle.TryGetProperty("font", out JsonElement sliderFont)
                             && sliderStyle.TryGetProperty("activeFont", out JsonElement sliderActiveFont))
                    {
                        ColoredFontModel font = JsonSerializer.Deserialize<ColoredFontModel>(sliderFont.GetRawText());
                        ColoredFontModel activeFont  = JsonSerializer.Deserialize<ColoredFontModel>(sliderActiveFont.GetRawText());
                        // Console.WriteLine("\n\n " + leftText + " | " + rightText);
                        result[viewIdStr].Add(new CmpSliderModel(id.GetString(), type.GetString(), name.GetString(),
                            leftText.GetString(), rightText.GetString(), sliderBgColor.GetString(), sliderActiveBgColor.GetString(), font, activeFont));
                    }
                    else if (viewElement.TryGetProperty("type", out JsonElement nativeImageType)
                             && nativeImageType.GetString() != null
                             && nativeImageType.GetString().Equals("NativeImage")
                             && viewElement.TryGetProperty("settings", out JsonElement nativeImageSettings)
                             && nativeImageSettings.TryGetProperty("src", out JsonElement src))
                    {
                        // Console.WriteLine("\n " + src + "\n");
                        result[viewIdStr].Add(new CmpNativeImageModel(id.GetString(), type.GetString(), name.GetString(),
                            src.GetString()));
                    }
                    else if (viewElement.TryGetProperty("type", out JsonElement longButtonType)
                             && longButtonType.GetString() != null
                             && longButtonType.GetString().Equals("LongButton")
                             && viewElement.TryGetProperty("settings", out JsonElement longButtonSettings)
                             && longButtonSettings.TryGetProperty("onText", out JsonElement onText)
                             && longButtonSettings.TryGetProperty("offText", out JsonElement offText)
                             && longButtonSettings.TryGetProperty("customText", out JsonElement customText)
                             && longButtonSettings.TryGetProperty("style", out JsonElement longButtonStyle)
                             && longButtonStyle.TryGetProperty("onFocusBackgroundColor", out JsonElement onFocusBackgroundColor)
                             && longButtonStyle.TryGetProperty("onUnfocusBackgroundColor", out JsonElement onUnfocusBackgroundColor))
                    {
                        // Console.WriteLine("\n\n " + onText + " | " + offText + " | " + customText);

                        //bool startFocus = false;
                        //if (longButtonSettings.TryGetProperty("startFocus", out JsonElement startFocusJE))
                        //{
                        //    startFocus = startFocusJE.GetBoolean();
                        //}
                        ColoredFontModel font = null;
                        if(longButtonStyle.TryGetProperty("font", out JsonElement longButtFont))
                        {
                            font = JsonSerializer.Deserialize<ColoredFontModel>(longButtFont.GetRawText());
                        }
                        result[viewIdStr].Add(new CmpLongButtonModel(id.GetString(), type.GetString(), name.GetString(), font,
                            onText.GetString(), offText.GetString(), customText.GetString(), /*startFocus,*/ onFocusBackgroundColor.GetString(), onUnfocusBackgroundColor.GetString()));
                    }
                    else if (viewElement.TryGetProperty("type", out JsonElement cookieTableType)
                             && cookieTableType.GetString() != null
                             && cookieTableType.GetString().Equals("CookieTable")
                             && viewElement.TryGetProperty("settings", out JsonElement cookieTableSettings)
                             && cookieTableSettings.TryGetProperty("nameText", out JsonElement cookieName)
                             && cookieTableSettings.TryGetProperty("categoryText", out JsonElement categoryText)
                             && cookieTableSettings.TryGetProperty("domainText", out JsonElement domainText)
                             && cookieTableSettings.TryGetProperty("durationText", out JsonElement durationText)
                        )
                    {
                        result[viewIdStr].Add(new CmpCookieTableModel(id.GetString(), type.GetString(), name.GetString(),
                            cookieName.GetString(), categoryText.GetString(), domainText.GetString(), durationText.GetString()));
                    }
                    else if (viewElement.TryGetProperty("type", out JsonElement nativeButtonType)
                             && nativeButtonType.GetString() != null
                             && nativeButtonType.GetString().Equals("NativeButton")
                             && viewElement.TryGetProperty("settings", out JsonElement nativeButtonSettings)
                             && nativeButtonSettings.TryGetProperty("text", out JsonElement nativeButtonText)
                             && nativeButtonSettings.TryGetProperty("startFocus", out JsonElement startFocus)
                             && nativeButtonSettings.TryGetProperty("style", out JsonElement nativeButtonStyle)
                             && nativeButtonStyle.TryGetProperty("font", out JsonElement nativeButtonFont))
                    {
                        FontModel font;
                        if (id.GetString().Equals("BackButton")
                            && nativeButtonStyle.TryGetProperty("backgroundColor", out JsonElement backgroundColor))
                        {
                            font = JsonSerializer.Deserialize<ColoredFontModel>(nativeButtonFont.GetRawText());
                            result[viewIdStr].Add(new CmpBackButtonModel(id.GetString(), type.GetString(), name.GetString(), font as ColoredFontModel,
                            startFocus.GetBoolean(), nativeButtonText.GetString(), backgroundColor.GetString()));
                        }
                        else if(nativeButtonStyle.TryGetProperty("onFocusBackgroundColor", out JsonElement nativeButtonOnFocusBackgroundColor)
                             && nativeButtonStyle.TryGetProperty("onUnfocusBackgroundColor", out JsonElement nativeButtonOnUnfocusBackgroundColor)
                             && nativeButtonStyle.TryGetProperty("onFocusTextColor", out JsonElement nativeButtonOnFocusTextColor)
                             && nativeButtonStyle.TryGetProperty("onUnfocusTextColor", out JsonElement nativeButtonOnUnfocusTextColor))
                        {
                            font = JsonSerializer.Deserialize<FontModel>(nativeButtonFont.GetRawText());
                            result[viewIdStr].Add(new CmpNativeButtonModel(id.GetString(), type.GetString(), name.GetString(), font,
                            startFocus.GetBoolean(), nativeButtonText.GetString(), nativeButtonOnFocusBackgroundColor.GetString(), nativeButtonOnUnfocusBackgroundColor.GetString(), nativeButtonOnFocusTextColor.GetString(), nativeButtonOnUnfocusTextColor.GetString()));
                        }
                    }
                    else
                    {
                        Debug.LogError(">>>DAFUQ >:C " + id.GetString());
                    }
                }
            }
        }
        return result;
    }
}