using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

// CMP_id
// BackButton
// id LogoImage -> src

public static class NativeUiJsonDeserializer
{
    public static Dictionary<string, List<CmpUiElement>> localization;

    // static void Main(string[] args)
    // {
    //     localization = NativeUiJsonDeserializer.DeserializeNativePm(JSONSTUB.nativePM);
    // }

    public static Dictionary<string, List<CmpUiElement>> DeserializeNativePm(string json)
    {
        Dictionary<string, List<CmpUiElement>> result = new Dictionary<string, List<CmpUiElement>>();
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
                        result.Add(viewIdStr, new List<CmpUiElement>());
                    }
                    else
                    {
                        Debug.LogError(">>>DAFUQ >:C");
                        continue;
                    }
                }

                JsonElement viewChildren = view.GetProperty("children");
                foreach (JsonElement viewElement in viewChildren.EnumerateArray())
                {
                    viewElement.TryGetProperty("id", out JsonElement id);
                    viewElement.TryGetProperty("type", out JsonElement type);
                    viewElement.TryGetProperty("name", out JsonElement name);

                    if (viewElement.TryGetProperty("settings", out JsonElement settings)
                        && settings.TryGetProperty("text", out JsonElement text))
                    {
                        // if (!String.IsNullOrEmpty(text.GetString()))
                        // Console.WriteLine(id.GetString() + " | " + text.GetString());
                        result[viewIdStr].Add(new CmpText(id.GetString(), type.GetString(), name.GetString(),
                            text.GetString()));
                    }
                    else if (viewElement.TryGetProperty("type", out JsonElement sliderType)
                             && sliderType.GetString() != null
                             && sliderType.GetString().Equals("Slider")
                             && viewElement.TryGetProperty("settings", out JsonElement sliderSettings)
                             && sliderSettings.TryGetProperty("leftText", out JsonElement leftText)
                             && sliderSettings.TryGetProperty("rightText", out JsonElement rightText))
                    {
                        // Console.WriteLine("\n\n " + leftText + " | " + rightText);
                        result[viewIdStr].Add(new CmpSlider(id.GetString(), type.GetString(), name.GetString(),
                            leftText.GetString(), rightText.GetString()));
                    }
                    else if (viewElement.TryGetProperty("type", out JsonElement nativeImageType)
                             && nativeImageType.GetString() != null
                             && nativeImageType.GetString().Equals("NativeImage")
                             && viewElement.TryGetProperty("settings", out JsonElement nativeImageSettings)
                             && nativeImageSettings.TryGetProperty("src", out JsonElement src))
                    {
                        // Console.WriteLine("\n " + src + "\n");
                        result[viewIdStr].Add(new CmpNativeImage(id.GetString(), type.GetString(), name.GetString(),
                            src.GetString()));
                    }
                    else if (viewElement.TryGetProperty("type", out JsonElement longButtonType)
                             && longButtonType.GetString() != null
                             && longButtonType.GetString().Equals("LongButton")
                             && viewElement.TryGetProperty("settings", out JsonElement longButtonSettings)
                             && longButtonSettings.TryGetProperty("onText", out JsonElement onText)
                             && longButtonSettings.TryGetProperty("offText", out JsonElement offText)
                             && longButtonSettings.TryGetProperty("customText", out JsonElement customText))
                    {
                        // Console.WriteLine("\n\n " + onText + " | " + offText + " | " + customText);
                        result[viewIdStr].Add(new CmpLongButton(id.GetString(), type.GetString(), name.GetString(),
                            onText.GetString(), offText.GetString(), customText.GetString()));
                    }
                }
            }
        }
        return result;
    }
}
