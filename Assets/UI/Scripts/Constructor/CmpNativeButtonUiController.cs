using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class CmpNativeButtonUiController : CmpLocalizationUiElement
{
    [SerializeField] private Text localizedText;
    [SerializeField] private Button button;
    [SerializeField] private CmpButtonAnimatorController btnAnim;

    public override void SetLocalization(CmpUiElementModel elementModel)
    {
        if (elementModel.id.Equals("DoNotSellButton"))
        {
            var nativeButton = elementModel as CmpLongButtonModel;
            localizedText.text = nativeButton.name;
            ColorBlock clrs = button.colors;
            clrs.highlightedColor = GraphicExtension.HexToColor(nativeButton.OnFocusColorCode);
            clrs.normalColor = GraphicExtension.HexToColor(nativeButton.OnUnfocusColorCode);
            button.colors = clrs;
            // btnAnim.activeTextColor = GraphicExtension.HexToColor(nativeButton.OnUnfocusColorCode);
            // btnAnim.defaultTextColor = GraphicExtension.HexToColor(nativeButton.OnFocusColorCode);
            btnAnim.SetIdleState();
            model = nativeButton;
        }
        else
        {
            var nativeButton = elementModel as CmpNativeButtonModel;
            localizedText.text = nativeButton.Text;
            
            ColorBlock clrs = button.colors;
            clrs.highlightedColor = GraphicExtension.HexToColor(nativeButton.OnFocusBackgroundColor);
            clrs.normalColor = GraphicExtension.HexToColor(nativeButton.OnUnfocusBackgroundColor);
            button.colors = clrs;
            btnAnim.activeTextColor = GraphicExtension.HexToColor(nativeButton.OnFocusTextColor);
            btnAnim.defaultTextColor = GraphicExtension.HexToColor(nativeButton.OnUnfocusTextColor);
            if (nativeButton.StartFocus.HasValue && nativeButton.StartFocus.Value)
            {
                btnAnim.SetActiveState();
                //TODO
            }
            else
            {
                btnAnim.SetIdleState();
            }
            model = nativeButton;
        }
    }
}