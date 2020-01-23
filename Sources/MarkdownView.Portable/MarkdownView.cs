using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownView.Extensions;
using MarkdownView.Factories.Spans;
using MarkdownView.Factories.Views;
using MarkdownView.Theming;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using MarkdigParser = Markdig.Markdown;

namespace MarkdownView
{
    [ContentProperty(nameof(Markdown))]
    public class MarkdownView : ContentView
    {
        private List<View> QueuedViews { get; } = new List<View>();

        private IDictionary<string, string> Links { get; } = new Dictionary<string, string>();

        private bool IsQuoted { get; set; }

        public static Func<string, Task> LinkCallbackFunc { get; set; } = async uriString => await Launcher.OpenAsync(new Uri(uriString));

        public static readonly BindableProperty MarkdownProperty = BindableProperty.Create(nameof(Markdown), typeof(string), typeof(MarkdownView), null, propertyChanged: OnMarkdownChanged);
        public string Markdown { get => (string)GetValue(MarkdownProperty); set => SetValue(MarkdownProperty, value); }

        public static readonly BindableProperty RelativeUrlHostProperty = BindableProperty.Create(nameof(RelativeUrlHost), typeof(string), typeof(MarkdownView), null, propertyChanged: OnMarkdownChanged);
        public string RelativeUrlHost { get => (string)GetValue(RelativeUrlHostProperty); set => SetValue(RelativeUrlHostProperty, value); }

        public static BaseTheme DefaultTheme = new LightTheme();

        public static readonly BindableProperty ThemeProperty = BindableProperty.Create(nameof(Theme), typeof(BaseTheme), typeof(MarkdownView), DefaultTheme, propertyChanged: OnMarkdownChanged);
        public BaseTheme Theme { get => (BaseTheme)GetValue(ThemeProperty); set => SetValue(ThemeProperty, value); }

        private static void OnMarkdownChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue.ToString()) || !(bindable is MarkdownView markdownView))
                return;

            markdownView.Padding = markdownView.Theme.Margin;
            markdownView.BackgroundColor = markdownView.Theme.BackgroundColor;

            var stackLayout = new StackLayout { Spacing = markdownView.Theme.Margin };
            var document = MarkdigParser.Parse(newValue.ToString());
            foreach (var block in document.AsEnumerable())
            {
                var view = markdownView.Render(block, markdownView.Theme, markdownView.Links);
                stackLayout.Children.Add(view);
            }
            markdownView.Content = stackLayout;
        }

        private static void AttachLinks(View view, IDictionary<string, string> links)
        {
            if (view == null || !links.Any())
                return;

            var cmd = new Command(async () => await OnLinkClickedCommand(links));
            view.GestureRecognizers.Add(new TapGestureRecognizer { Command = cmd });
        }

        private static async Task OnLinkClickedCommand(IDictionary<string, string> linkDict)
        {
            try
            {
                if (linkDict.Count <= 1)
                    await LinkCallbackFunc(linkDict.First().Value);
                else
                {
                    var options = linkDict.Select(x => x.Key).ToArray();
                    var result = await Application.Current.MainPage.DisplayActionSheet("Open link", "Cancel", null, options);
                    var link = linkDict.FirstOrDefault(x => x.Key == result);
                    await LinkCallbackFunc(link.Value);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        #region Rendering blocks

        private View Render(Block block, IMarkdownTheme theme, IDictionary<string, string> links)
        {
            View view = null;
            switch (block)
            {
                case HeadingBlock heading:
                    view = Render(heading, links, theme, IsQuoted);
                    break;
                case ParagraphBlock paragraph:
                    view = Render(paragraph, theme);
                    break;
                case QuoteBlock quote:
                    view = Render(quote, theme);
                    break;
                case CodeBlock code:
                    view = Render(code, theme.Code);
                    break;
                case ListBlock list:
                    view = Render(list, theme);
                    break;
                case ThematicBreakBlock thematicBreak:
                    view = Render(thematicBreak, theme.Separator);
                    break;
                case HtmlBlock html:
                    view = HtmlBlockFactory.Create(html);
                    break;
                default:
                    Debug.WriteLine($"Can't render {block.GetType()} blocks.");
                    break;
            }
            var stackLayout = new StackLayout();
            stackLayout.Children.Add(view);

            foreach (var queuedView in QueuedViews)
                stackLayout.Children.Add(queuedView);

            QueuedViews.Clear();
            return stackLayout;
        }

        private View Render(ParagraphBlock block, IMarkdownTheme theme)
        {
            var label = new Label();
            label.TextColor = IsQuoted ? theme.Quote.ForegroundColor : theme.Paragraph.ForegroundColor;
            label.FormattedText = CreateFormatted(block.Inline, theme.Paragraph);

            AttachLinks(label, Links);

            return label;
        }

        private View Render(ThematicBreakBlock _, IBlockStyle style)
        {
            var boxView = new BoxView();
            boxView.HeightRequest = style.BorderSize;
            boxView.BackgroundColor = style.BorderColor;

            return boxView;
        }

        private View Render(ListBlock block, IMarkdownTheme theme)
        {
            var currentListScope = 1;
            var stackLayout = new StackLayout();
            for (var i = 0; i < block.Count(); i++)
                if (block.ElementAt(i) is ListItemBlock itemBlock)
                    stackLayout.Children.Add(Render(block, i + 1, itemBlock, theme, currentListScope));

            return stackLayout;

        }

        private View Render(ListBlock parent, int index, ListItemBlock block, IMarkdownTheme theme, int listScope = 1)
        {

            var horizontalStack = new StackLayout();
            horizontalStack.Orientation = StackOrientation.Horizontal;
            horizontalStack.Margin = new Thickness(listScope * theme.Paragraph.BorderSize, 0, 0, 0);

            var bullet = parent.IsOrdered ? new Label
            {
                Text = $"{index}.",
                FontSize = theme.Paragraph.FontSize,
                TextColor = theme.Paragraph.ForegroundColor,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End,
            } : (View)new BoxView
            {
                WidthRequest = 4,
                HeightRequest = 4,
                Margin = new Thickness(0, 6, 0, 0),
                BackgroundColor = theme.Paragraph.ForegroundColor,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
            };
            horizontalStack.Children.Add(bullet);

            var contentStack = new StackLayout { Orientation = StackOrientation.Horizontal };
            foreach (var subBlock in block)
            {
                var subView = Render(subBlock, theme, Links);
                contentStack.Children.Add(subView);
            }
            horizontalStack.Children.Add(contentStack);
            return horizontalStack;
        }

        private View Render(HeadingBlock block, IDictionary<string, string> links, IMarkdownTheme theme, bool isQuoted)
        {
            IBlockStyle blockStyle;
            switch (block.Level)
            {
                case 1:
                    blockStyle = theme.Heading1;
                    break;
                case 2:
                    blockStyle = theme.Heading2;
                    break;
                case 3:
                    blockStyle = theme.Heading3;
                    break;
                case 4:
                    blockStyle = theme.Heading4;
                    break;
                case 5:
                    blockStyle = theme.Heading5;
                    break;
                default:
                    blockStyle = theme.Heading6;
                    break;
            }

            var foregroundColor = isQuoted ? theme.Quote.ForegroundColor : blockStyle.ForegroundColor;
            var label = new Label
            {
                TextColor = foregroundColor, // TODO: Check if FormattedText uses this Color, else we need to pass it to CreateFormatted
                FormattedText = CreateFormatted(block.Inline, blockStyle),
            };
            AttachLinks(label, links);

            if (!(blockStyle.BorderSize > 0))
                return label;

            var headingStack = new StackLayout();
            headingStack.Children.Add(label);
            headingStack.Children.Add(new BoxView
            {
                HeightRequest = blockStyle.BorderSize,
                BackgroundColor = blockStyle.BorderColor,
            });
            return headingStack;
        }

        private View Render(QuoteBlock block, IMarkdownTheme theme)
        {
            var horizontalStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Theme.Quote.BackgroundColor,
            };

            horizontalStack.Children.Add(new BoxView
            {
                WidthRequest = Theme.Quote.BorderSize,
                BackgroundColor = Theme.Quote.BorderColor,
            });

            IsQuoted = true;
            foreach (var subBlock in block.AsEnumerable())
                horizontalStack.Children.Add(Render(subBlock, theme, Links));
            IsQuoted = false;

            return horizontalStack;
        }

        private View Render(CodeBlock block, IBlockStyle style)
        {
            var label = new Label
            {
                TextColor = style.ForegroundColor,
                FontAttributes = style.Attributes,
                FontFamily = style.FontFamily,
                FontSize = style.FontSize,
                Text = string.Join(Environment.NewLine, block.Lines),
            };
            var frame = new Frame
            {
                CornerRadius = 3, // TODO: Move magic number to style
                HasShadow = false,
                Padding = Theme.Margin,
                BackgroundColor = style.BackgroundColor,
                Content = label
            };
            return frame;
        }

        private FormattedString CreateFormatted(ContainerInline inlines, IBlockStyle theme)
        {
            var fs = new FormattedString();
            var sp = inlines.SelectMany(inline => CreateSpans(inline, theme)); // TODO: Should we filter nulls here?
            foreach (var span in sp)
                fs.Spans.Add(span);

            return fs;
        }

        private IEnumerable<Span> CreateSpans(Inline inline, IBlockStyle style, string fontFamily = null)
        {
            var spans = new List<Span>();
            switch (inline)
            {
                case LiteralInline literal:
                    var literalSpan = new Span
                    {
                        Text = literal.Content.Text.Substring(literal.Content.Start, literal.Content.Length),
                        FontAttributes = style.Attributes,
                        ForegroundColor = style.ForegroundColor,
                        BackgroundColor = style.BackgroundColor,
                        FontSize = style.FontSize,
                        FontFamily = style.FontFamily,
                    };
                    spans.Add(literalSpan);
                    break;
                case EmphasisInline emphasis:
                    var childAttributes = emphasis.DelimiterCount == 2 ? FontAttributes.Bold : FontAttributes.Italic;
                    var childFamily = fontFamily ?? style.FontFamily;
                    switch (childAttributes)
                    {
                        case FontAttributes.Bold:
                            childFamily = fontFamily ?? style.FontFamilyBold;
                            break;
                        case FontAttributes.Italic:
                            childFamily = fontFamily ?? style.FontFamilyItalic;
                            break;
                    }
                    var emphasedSpans = emphasis.SelectMany(emSpan => CreateSpans(emSpan, style, childFamily));
                    spans.AddRange(emphasedSpans);
                    break;
                case LineBreakInline lineBreak:
                    var lineBreakSpans = SpanFactory.Create(lineBreak);
                    spans.AddRange(lineBreakSpans);
                    break;
                case LinkInline link:
                    var url = link.Url;
                    if (!(url.StartsWith("http://") || url.StartsWith("https://")))
                        url = $"{RelativeUrlHost?.TrimEnd('/')}/{url.TrimStart('/')}";

                    if (link.IsImage)
                    {
                        var image = new Image();
                        if (Path.GetExtension(url) == ".svg")
                            image.RenderSvg(url);
                        else
                            image.Source = url;

                        QueuedViews.Add(image);
                    }
                    else
                    {
                        spans = link.SelectMany(l => CreateSpans(l, style)).ToList();
                        Links.Add(new KeyValuePair<string, string>(string.Join(string.Empty, spans.Select(x => x.Text)), url));
                    }
                    break;
                case CodeInline code:
                    var codeSpans = SpanFactory.Create(code, style.ForegroundColor, style.BackgroundColor,
                        style.FontFamily, style.Attributes, style.FontSize);
                    spans.AddRange(codeSpans);
                    break;
                default:
                    Debug.WriteLine($"Can't render Inline of type {inline.GetType().FullName}.");
                    break;
            }
            return spans;
        }

        #endregion
    }
}
