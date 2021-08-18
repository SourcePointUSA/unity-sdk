using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class CmpTextUiController : CmpLocalizationUiElement
{
    [SerializeField] private Text localizedText;
    
    public override void SetLocalization(CmpUiElementModel elementModel)
    {
        var text = elementModel as CmpTextModel;
        if(text.Font != null)
            localizedText.color = GraphicExtension.HexToColor(text.Font.color);
        localizedText.text = text.Text;
        model = text;
    }
}