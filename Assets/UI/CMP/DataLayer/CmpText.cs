public class CmpText : CmpUiElement
{
    private string text;
    public string Text => text;

    public CmpText(string id, string type, string name, string text) : base(id, type, name)
    {
        this.text = text;
    }
}
