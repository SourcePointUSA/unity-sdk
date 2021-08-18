public class CmpSliderModel : CmpUiElementModel
{
    string leftText;
    string rightText;
    string backgroundColor;
    string activeBackgroundColor;
    ColoredFontModel defaultFont;
    ColoredFontModel activeFont;

    public string LeftText => leftText;
    public string RightText => rightText;
    public string BackgroundColor => backgroundColor;
    public string ActiveBackgroundColor => activeBackgroundColor;
    public ColoredFontModel DefaultFont => defaultFont;
    public ColoredFontModel ActiveFont => activeFont;
    
    public CmpSliderModel(string id, string type, string name, string leftText, string rightText, string backgroundColor, string activeBackgroundColor, ColoredFontModel defaultFont, ColoredFontModel activeFont) : base(id, type, name)
    {
        this.leftText = leftText;
        this.rightText = rightText;
        this.backgroundColor = backgroundColor;
        this.activeBackgroundColor = activeBackgroundColor;
        this.defaultFont = defaultFont;
        this.activeFont = activeFont;
    }
}
