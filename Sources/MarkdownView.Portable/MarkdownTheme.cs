using Xamarin.Forms;

namespace MarkdownView
{
    public class MarkdownTheme
    {
        #region PROPS

        public Color BackgroundColor { get; set; }

        public MarkdownStyle Paragraph { get; set; }

        public MarkdownStyle Heading1 { get; set; }

        public MarkdownStyle Heading2 { get; set; }

        public MarkdownStyle Heading3 { get; set; }

        public MarkdownStyle Heading4 { get; set; }

        public MarkdownStyle Heading5 { get; set; }

        public MarkdownStyle Heading6 { get; set; }

        public MarkdownStyle Quote { get; set; }

        public MarkdownStyle Separator { get; set; }

        public MarkdownStyle Link { get; set; }

        public MarkdownStyle Code { get; set; }

        public float Margin { get; set; } = 5f;

        public string FontFamily { get; set; }

        public string FontFamilyItalic { get; set; }

        public string FontFamilyBold { get; set; }

        #endregion

        public MarkdownTheme()
        {
            Paragraph = new MarkdownStyle
            {
                Attributes = FontAttributes.None,
                FontSize = 12,
            };

            Heading1 = new MarkdownStyle
            {
                Attributes = FontAttributes.Bold,
                BorderSize = 1,
                FontSize = 26,
            };

            Heading2 = new MarkdownStyle
            {
                Attributes = FontAttributes.Bold,
                BorderSize = 1,
                FontSize = 22,
            };

            Heading3 = new MarkdownStyle
            {
                Attributes = FontAttributes.Bold,
                FontSize = 20,
            };

            Heading4 = new MarkdownStyle
            {
                Attributes = FontAttributes.Bold,
                FontSize = 18,
            };

            Heading5 = new MarkdownStyle
            {
                Attributes = FontAttributes.Bold,
                FontSize = 16,
            };

            Heading6 = new MarkdownStyle
            {
                Attributes = FontAttributes.Bold,
                FontSize = 14,
            };

            Link = new MarkdownStyle
            {
                Attributes = FontAttributes.None,
                FontSize = 12,
            };

            Code = new MarkdownStyle
            {
                Attributes = FontAttributes.None,
                FontSize = 12,
            };

            Quote = new MarkdownStyle
            {
                Attributes = FontAttributes.None,
                BorderSize = 4,
                FontSize = 12,
                BackgroundColor = Color.Gray.MultiplyAlpha(.1),
            };

            Separator = new MarkdownStyle
            {
                BorderSize = 2,
            };

            // Platform specific properties
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Code.FontFamily = "Courier";
                    break;

                case Device.Android:
                    Code.FontFamily = "monospace";
                    break;
            }
        }
    }
}
