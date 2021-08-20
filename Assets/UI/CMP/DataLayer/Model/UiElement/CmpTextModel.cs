public class CmpTextModel : CmpUiElementModel
{
    private string text;
    public string Text => text;
    private ColoredFontModel font;
    public ColoredFontModel Font => font;

    public CmpTextModel(string id, string type, string name, ColoredFontModel font, string text) : base(id, type, name)
    {
        this.text = text;
        this.font = font;
    }
}
