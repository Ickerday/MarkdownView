using Xamarin.Forms;

namespace Xam.Forms.MarkdownView.Themes
{
    public class DarkMarkdownTheme : MarkdownTheme
    {
        public static readonly Color DefaultBackgroundColor = Color.FromHex("#2b303b");

        public static readonly Color DefaultAccentColor = Color.FromHex("#d08770");

        public static readonly Color DefaultTextColor = Color.FromHex("#eff1f5");

        public static readonly Color DefaultCodeBackground = Color.FromHex("#4f5b66");

        public static readonly Color DefaultSeparatorColor = Color.FromHex("#65737e");

        public static readonly Color DefaultQuoteTextColor = Color.FromHex("#a7adba");

        public static readonly Color DefaultQuoteBorderColor = Color.FromHex("#a7adba");

        public DarkMarkdownTheme()
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
