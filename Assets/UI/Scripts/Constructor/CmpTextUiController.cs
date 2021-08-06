using UnityEngine;
using UnityEngine.UI;

public class CmpTextUiController : CmpLocalizationUiElement
{
    [SerializeField] private Text localizedText;
    
    public override void SetLocalization(CmpUiElementModel elementModel)
    {
        var text = elementModel as CmpTextModel;
        localizedText.text = text.Text;
        model = text;
    }
}