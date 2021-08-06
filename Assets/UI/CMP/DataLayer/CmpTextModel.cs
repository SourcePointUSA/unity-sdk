public class CmpTextModel : CmpUiElementModel
{
    private string text;
    public string Text => text;

    public CmpTextModel(string id, string type, string name, string text) : base(id, type, name)
    {
        this.text = text;
    }
}
