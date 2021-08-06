public abstract class CmpUiElementModel
{
    private string id;
    private string type;
    private string name;

    public string Id => id;
    public string Type => type;
    public string Name => name;

    protected CmpUiElementModel(string id, string type, string name)
    {
        this.id = id;
        this.type = type;
        this.name = name;
    }
}