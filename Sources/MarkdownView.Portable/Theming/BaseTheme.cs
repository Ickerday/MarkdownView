using Xamarin.Forms;

namespace MarkdownView.Theming
{
    public class BaseTheme : IMarkdownTheme
    {
        #region PROPS
        public float Margin { get; set; } = 5f;

        public Color BackgroundColor { get; set; } = Color.Transparent;

        public IBlockStyle Paragraph { get; set; } = new BlockStyle
        {
            Attributes = FontAttributes.None,
            FontSize = 12
        };

        public IBlockStyle Heading1 { get; set; } = new BlockStyle
        {
            Attributes = FontAttributes.Bold,
            BorderSize = 1,
            FontSize = 26
        };

        public IBlockStyle Heading2 { get; set; } = new BlockStyle
        {
            Attributes = FontAttributes.Bold,
            BorderSize = 1,
            FontSize = 22
        };

        public IBlockStyle Heading3 { get; set; } = new BlockStyle
        {
            Attributes = FontAttributes.Bold,
            FontSize = 20
        };

        public IBlockStyle Heading4 { get; set; } = new BlockStyle
        {
            Attributes = FontAttributes.Bold,
            FontSize = 18
        };

        public IBlockStyle Heading5 { get; set; } = new BlockStyle
        {
            Attributes = FontAttributes.Bold,
            FontSize = 16
        };

        public IBlockStyle Heading6 { get; set; } = new BlockStyle
        {
            Attributes = FontAttributes.Bold,
            FontSize = 14
        };

        public IBlockStyle Quote { get; set; } = new BlockStyle
        {
            Attributes = FontAttributes.None,
            BorderSize = 4,
            FontSize = 12,
            BackgroundColor = Color.Accent.MultiplyAlpha(.1)
        };

        public IBlockStyle Separator { get; set; } = new BlockStyle
        {
            BorderSize = 2
        };

        public IBlockStyle Link { get; set; } = new BlockStyle
        {
            Attributes = FontAttributes.None,
            FontSize = 12
        };

        public IBlockStyle Code { get; set; } = new BlockStyle
        {
            Attributes = FontAttributes.None,
            FontSize = 12,
            FontFamily = Device.RuntimePlatform == Device.Android ? "monospace" : "Courier"
        };

        #endregion
    }
}