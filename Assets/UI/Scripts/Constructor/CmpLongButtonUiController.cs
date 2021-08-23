using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class CmpLongButtonUiController : CmpLocalizationUiElement
{
    [SerializeField] private Text onText;
    [SerializeField] private Text offText;
    [SerializeField] private Text customText;
    [SerializeField] private Button button;

    [SerializeField] private Text mainText;

    public override void SetLocalization(CmpUiElementModel elementModel)
    {
        var longButton = elementModel as CmpLongButtonModel;
        onText.text = longButton.OnText;
        offText.text = longButton.OffText;
        customText.text = longButton.CustomText;

        if (longButton.Font != null && longButton.Font.color != null)
        {
            onText.color = GraphicExtension.HexToColor(longButton.Font.color);
            offText.color = GraphicExtension.HexToColor(longButton.Font.color);
            customText.color = GraphicExtension.HexToColor(longButton.Font.color);
            mainText.color = GraphicExtension.HexToColor(longButton.Font.color);
        }

        ColorBlock clrs = button.colors;
        clrs.highlightedColor = GraphicExtension.HexToColor(longButton.OnFocusColorCode);
        clrs.normalColor = GraphicExtension.HexToColor(longButton.OnUnfocusColorCode);
        button.colors = clrs;

        model = longButton;
    }

    public void SetMainText(string text)
    {
        mainText.text = text;
    }

    public void EnableCustomTextLabel(bool enable)
    {
        customText.gameObject.SetActive(enable);
    }
}