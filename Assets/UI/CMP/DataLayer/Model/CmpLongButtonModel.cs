public class CmpLongButtonModel : CmpUiElementModel
{
    private string onText;
    private string offText;
    private string customText;

    public string OnText => onText;
    public string OffText => offText;
    public string CustomText => customText;
    
    public CmpLongButtonModel(string id, string type, string name, string onText, string offText, string customText) : base(id, type, name)
    {
        this.onText = onText;
        this.offText = offText;
        this.customText = customText;
    }
}