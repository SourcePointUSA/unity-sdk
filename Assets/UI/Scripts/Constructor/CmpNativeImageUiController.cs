using UnityEngine;
using UnityEngine.UI;

public class CmpNativeImageUiController : CmpLocalizationUiElement
{
    [SerializeField] private Image CompanyLogo;
    private string imageLink;
    
    public override void SetLocalization(CmpUiElementModel elementModel)
    {
        var nativeImage = elementModel as CmpNativeImageModel;
        imageLink = nativeImage.LogoImageLink;
        // CompanyLogo = LoadImage(imageLink);
        model = nativeImage;
    }

    private Sprite LoadImage(string link)
    {
        Sprite result = null;
        //TODO: Network call ...
        return result;
    }
}