public class CmpNativeImage : CmpUiElement
{
    private string src;

    public string LogoImageLink => src;

    public CmpNativeImage(string id, string type, string name, string src) : base(id, type, name)
    {
        this.src = src;
    }
}