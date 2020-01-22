using Xamarin.Forms;

namespace Xam.Forms.MarkdownView.Themes
{
    public class LightMarkdownTheme : MarkdownTheme
    {
        public static readonly Color DefaultBackgroundColor = Color.FromHex("#ffffff");

        public static readonly Color DefaultAccentColor = Color.FromHex("#0366d6");

        public static readonly Color DefaultTextColor = Color.FromHex("#24292e");

        public static readonly Color DefaultCodeBackground = Color.FromHex("#f6f8fa");

        public static readonly Color DefaultSeparatorColor = Color.FromHex("#eaecef");

        public static readonly Color DefaultQuoteTextColor = Color.FromHex("#6a737d");

        public static readonly Color DefaultQuoteBorderColor = Color.FromHex("#dfe2e5");

        public LightMarkdownTheme()
        {
            BackgroundColor = DefaultBackgroundColor;
            Paragraph.ForegroundColor = DefaultTextColor;
            Heading1.ForegroundColor = DefaultTextColor;
            Heading1.BorderColor = DefaultSeparatorColor;
            Heading2.ForegroundColor = DefaultTextColor;
            Heading2.BorderColor = DefaultSeparatorColor;
            Heading3.ForegroundColor = DefaultTextColor;
            Heading4.ForegroundColor = DefaultTextColor;
            Heading5.ForegroundColor = DefaultTextColor;
            Heading6.ForegroundColor = DefaultTextColor;
            Link.ForegroundColor = DefaultAccentColor;
            Code.ForegroundColor = DefaultTextColor;
            Code.BackgroundColor = DefaultCodeBackground;
            Quote.ForegroundColor = DefaultQuoteTextColor;
            Quote.BorderColor = DefaultQuoteBorderColor;
            Separator.BorderColor = DefaultSeparatorColor;
        }
    }
}
