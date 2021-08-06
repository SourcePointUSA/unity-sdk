public class CmpSliderModel : CmpUiElementModel
{
    private string leftText;
    private string rightText;

    public string LeftText => leftText;
    public string RightText => rightText;

    public CmpSliderModel(string id, string type, string name, string leftText, string rightText) : base(id, type, name)
    {
        this.leftText = leftText;
        this.rightText = rightText;
    }
}
