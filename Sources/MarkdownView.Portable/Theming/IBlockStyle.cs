using Xamarin.Forms;

namespace MarkdownView.Theming
{
    public interface IBlockStyle
    {
        FontAttributes Attributes { get; set; }
        Color BackgroundColor { get; set; }
        Color BorderColor { get; set; }
        float BorderSize { get; set; }
        string FontFamily { get; set; }
        string FontFamilyBold { get; set; }
        string FontFamilyItalic { get; set; }
        float FontSize { get; set; }
        Color ForegroundColor { get; set; }
    }
}