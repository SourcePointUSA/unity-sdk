using UnityEngine;
using UnityEngine.UI;

public class CmpLongButtonUiController : CmpLocalizationUiElement
{
    [SerializeField] private Text onText;
    [SerializeField] private Text offText;
    [SerializeField] private Text customText;
    
    public override void SetLocalization(CmpUiElementModel elementModel)
    {
        var longButton = elementModel as CmpLongButtonModel;
        onText.text = longButton.OnText;
        offText.text = longButton.OffText;
        customText.text = longButton.CustomText;
        model = longButton;
    }
}