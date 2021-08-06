using UnityEngine;
using UnityEngine.UI;

public class CmpSliderUiController : CmpLocalizationUiElement
{
    [SerializeField] private Text leftLocalizedText;
    [SerializeField] private Text rightLocalizedText;
    
    public override void SetLocalization(CmpUiElementModel elementModel)
    {
        var slider = elementModel as CmpSliderModel;
        leftLocalizedText.text = slider.LeftText;
        rightLocalizedText.text = slider.RightText;
        model = slider;
    }
}