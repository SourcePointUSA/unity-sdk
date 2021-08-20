public class CmpNativeImageModel : CmpUiElementModel
{
    private string src;

    public string LogoImageLink => src;

    public CmpNativeImageModel(string id, string type, string name, string src) : base(id, type, name)
    {
        this.src = src;
    }
}