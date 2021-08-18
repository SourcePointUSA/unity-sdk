using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class CmpSliderUiController : CmpLocalizationUiElement
{
    [SerializeField] private Text leftLocalizedText;
    [SerializeField] private Text rightLocalizedText;
    [SerializeField] private TextColorAnimationController leftColorController;
    [SerializeField] private TextColorAnimationController rightColorController;
    [SerializeField] private Image bg;
    [SerializeField] private Image activeBg;
    
    public override void SetLocalization(CmpUiElementModel elementModel)
    {
        var slider = elementModel as CmpSliderModel;
        leftLocalizedText.text = slider.LeftText;
        rightLocalizedText.text = slider.RightText;

        leftColorController.activeTextColor = GraphicExtension.HexToColor(slider.ActiveFont.color);
        rightColorController.activeTextColor = GraphicExtension.HexToColor(slider.ActiveFont.color);

        leftColorController.defaultTextColor = GraphicExtension.HexToColor(slider.DefaultFont.color);
        rightColorController.defaultTextColor = GraphicExtension.HexToColor(slider.DefaultFont.color);

        bg.color = GraphicExtension.HexToColor(slider.BackgroundColor);
        activeBg.color = GraphicExtension.HexToColor(slider.ActiveBackgroundColor);
        
        model = slider;
    }
}