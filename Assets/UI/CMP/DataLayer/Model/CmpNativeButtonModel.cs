public class CmpNativeButtonModel : CmpUiElementModel
{
    private string text;
    private bool startFocus;
    private string onFocusBackgroundColor;
    private string onUnfocusBackgroundColor;
    private string onFocusTextColor;
    private string onUnfocusTextColor;

    public string Text => text;
    public bool StartFocus => startFocus;
    public string OnFocusBackgroundColor => onFocusBackgroundColor;
    public string OnUnfocusBackgroundColor => onUnfocusBackgroundColor;
    public string OnFocusTextColor => onFocusTextColor;
    public string OnUnfocusTextColor => onUnfocusTextColor;

    public CmpNativeButtonModel(string id, string type, string name, bool startFocus, string text, string onFocusBackgroundColor,
        string onUnfocusBackgroundColor, string onFocusTextColor, string onUnfocusTextColor) : base(id, type, name)
    {
        this.startFocus = startFocus;
        this.text = text;
        this.onFocusBackgroundColor = onFocusBackgroundColor;
        this.onUnfocusBackgroundColor = onUnfocusBackgroundColor;
        this.onFocusTextColor = onFocusTextColor;
        this.onUnfocusTextColor = onUnfocusTextColor;
    }
}