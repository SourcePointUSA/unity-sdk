using Assets.UI.Scripts.Util;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CmpLongButtonUiController : CmpLocalizationUiElement
{
    [SerializeField] private Text onText;
    [SerializeField] private Text offText;
    [SerializeField] private Text customText;
    [SerializeField] private Button button;
    [SerializeField] private Text mainText;
    [SerializeField] private GameObject onGroup;
    [SerializeField] private GameObject offGroup;

    public override void SetLocalization(CmpUiElementModel elementModel)
    {
        var longButton = elementModel as CmpLongButtonModel;
        if(onText!=null)
            onText.text = longButton.OnText;
        if(offText!=null)
            offText.text = longButton.OffText;
        if(customText != null)
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

    public void DisableOnOffGroup()
    {
        onGroup.SetActive(false);
        offGroup.SetActive(false);
    }

    public Button GetButton()
    {
        return button;
    }

    public void SetGroupState(bool isOn)
    {
        onGroup.SetActive(isOn);
        offGroup.SetActive(!isOn);
    }
}