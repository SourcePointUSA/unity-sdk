public class CmpLongButtonModel : CmpUiElementModel
{
    private string onText;
    private string offText;
    private string customText;
    private string onFocusColorCode;
    private string onUnfocusColorCode;
    private ColoredFontModel font;
    //private bool startFocus;

    public string OnText => onText;
    public string OffText => offText;
    public string CustomText => customText;
    public string OnFocusColorCode => onFocusColorCode;
    public string OnUnfocusColorCode => onUnfocusColorCode;
    public ColoredFontModel Font => font;
    //public bool StartFocus => startFocus;   


    public CmpLongButtonModel(string id, string type, string name, ColoredFontModel font, string onText, string offText, string customText, /* bool startFocus, */ string onFocusBackgroundColor, string onUnfocusBackgroundColor) : base(id, type, name)
    {
        this.onText = onText;
        this.offText = offText;
        this.customText = customText;
        this.onFocusColorCode = onFocusBackgroundColor;
        this.onUnfocusColorCode = onUnfocusBackgroundColor;
        this.font = font;
        //this.startFocus = startFocus;
    }
}