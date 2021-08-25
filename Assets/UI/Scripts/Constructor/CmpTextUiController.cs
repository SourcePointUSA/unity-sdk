using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class CmpTextUiController : CmpLocalizationUiElement
{
    [SerializeField] private Text localizedText;
    
    public override void SetLocalization(CmpUiElementModel elementModel)
    {
        if (elementModel != null)
        {
            var text = elementModel as CmpTextModel;
            if (text.Font != null && text.Font.color != null)
                localizedText.color = GraphicExtension.HexToColor(text.Font.color);
            localizedText.text = text.Text;
            model = text;
        }
        else
            localizedText.text = "";
    }
}