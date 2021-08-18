public class CmpBackButtonModel : CmpUiElementModel
{
    private string text;
    private bool startFocus;
    private string backgroundColor;
    private ColoredFontModel font;

    public string Text => text;
    public bool StartFocus => startFocus;
    public string BackgroundColor => backgroundColor;
    public ColoredFontModel Font => font;

    public CmpBackButtonModel(string id, string type, string name, ColoredFontModel font, bool startFocus, string text, string backgroundColor) : base(id, type, name)
    {
        this.startFocus = startFocus;
        this.text = text;
        this.backgroundColor = backgroundColor;
        this.font = font;
    }
}