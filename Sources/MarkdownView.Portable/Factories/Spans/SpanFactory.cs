using Markdig.Syntax.Inlines;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MarkdownView.Factories.Spans
{
    public static class SpanFactory
    {
        public static List<Span> Create(LineBreakInline _)
        {
            var spans = new List<Span> { new Span { Text = "\n" }, };
            return spans;
        }

        public static List<Span> Create(CodeInline code, Color foregroundColor, Color backgroundColor, string fontFamily, FontAttributes fontAttributes, double fontSize)
        {
            var spans = new List<Span> {
                new Span
                {
                    Text = "\u2002",
                    FontSize = fontSize,
                    FontFamily = fontFamily,
                    ForegroundColor = foregroundColor,
                    BackgroundColor = backgroundColor
                },
                new Span
                {
                    Text = code.Content,
                    FontAttributes = fontAttributes,
                    FontSize = fontSize,
                    FontFamily = fontFamily,
                    ForegroundColor = foregroundColor,
                    BackgroundColor = backgroundColor
                },
                new Span
                {
                    Text = "\u2002",
                    FontSize = fontSize,
                    FontFamily = fontFamily,
                    ForegroundColor = foregroundColor,
                    BackgroundColor = backgroundColor
                },
            };
            return spans;
        }
    }
}