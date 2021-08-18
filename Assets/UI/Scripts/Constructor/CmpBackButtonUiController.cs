using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class CmpBackButtonUiController : CmpLocalizationUiElement
{
    [SerializeField] private Text localizedText;
    [SerializeField] private Button button;

    public override void SetLocalization(CmpUiElementModel elementModel)
    {
        var btn = elementModel as CmpBackButtonModel;
        localizedText.color = GraphicExtension.HexToColor(btn.Font.color);
        localizedText.text = btn.Text;

        ColorBlock clrs = button.colors;
        clrs.normalColor = GraphicExtension.HexToColor(btn.BackgroundColor);
        button.colors = clrs;

        if(btn.StartFocus)
        {
            //TODO
        }
        model = btn;
    }
}