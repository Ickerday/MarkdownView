using Xamarin.Forms;

namespace MarkdownView.Theming
{
    public interface IMarkdownTheme
    {
        Color BackgroundColor { get; set; }
        IBlockStyle Code { get; set; }
        IBlockStyle Heading1 { get; set; }
        IBlockStyle Heading2 { get; set; }
        IBlockStyle Heading3 { get; set; }
        IBlockStyle Heading4 { get; set; }
        IBlockStyle Heading5 { get; set; }
        IBlockStyle Heading6 { get; set; }
        IBlockStyle Link { get; set; }
        float Margin { get; set; }
        IBlockStyle Paragraph { get; set; }
        IBlockStyle Quote { get; set; }
        IBlockStyle Separator { get; set; }
    }
}