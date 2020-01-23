using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownView.Extensions;
using MarkdownView.Theming;
using Xamarin.Essentials;
using Xamarin.Forms;
using MarkdigParser = Markdig.Markdown;

namespace MarkdownView
{
    public class MarkdownView : ContentView
    {
        private IDictionary<string, string> _links = new Dictionary<string, string>();
        private readonly List<View> _queuedViews = new List<View>();
        private StackLayout _stackLayout;
        private bool _isQuoted;
        private int _listScope;

        public Action<string> NavigateToLink { get; set; } = uriString => Launcher.OpenAsync(new Uri(uriString));

        public string Markdown { get => (string)GetValue(MarkdownProperty); set => SetValue(MarkdownProperty, value); }
        public static readonly BindableProperty MarkdownProperty = BindableProperty.Create(nameof(Markdown), typeof(string), typeof(MarkdownView), null, propertyChanged: OnMarkdownChanged);

        public string RelativeUrlHost { get => (string)GetValue(RelativeUrlHostProperty); set => SetValue(RelativeUrlHostProperty, value); }
        public static readonly BindableProperty RelativeUrlHostProperty = BindableProperty.Create(nameof(RelativeUrlHost), typeof(string), typeof(MarkdownView), null, propertyChanged: OnMarkdownChanged);

        public static MarkdownTheme DefaultTheme = new LightTheme();
        public MarkdownTheme Theme { get => (MarkdownTheme)GetValue(ThemeProperty); set => SetValue(ThemeProperty, value); }
        public static readonly BindableProperty ThemeProperty = BindableProperty.Create(nameof(Theme), typeof(MarkdownTheme), typeof(MarkdownView), DefaultTheme, propertyChanged: OnMarkdownChanged);

        private static void OnMarkdownChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as MarkdownView;
            view?.RenderMarkdown();
        }

        private void RenderMarkdown()
        {
            Padding = Theme.Margin;
            BackgroundColor = Theme.BackgroundColor;
            _stackLayout = new StackLayout { Spacing = Theme.Margin };
            if (!string.IsNullOrWhiteSpace(Markdown))
            {
                var parsed = MarkdigParser.Parse(Markdown);
                Render(parsed.AsEnumerable());
            }
            Content = _stackLayout; // Show the content only when the whole text has been parsed
        }

        private void Render(IEnumerable<Block> blocks)
        {
            foreach (var block in blocks)
                Render(block);
        }

        private void AttachLinks(View view)
        {
            if (!_links.Any())
                return;

            var blockLinks = _links;
            view.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => await OnLinkClickedCommand(blockLinks))
            });
            _links = new Dictionary<string, string>();
        }

        private async Task OnLinkClickedCommand(IDictionary<string, string> linkDict)
        {
            try
            {
                if (linkDict.Count <= 1)
                {
                    NavigateToLink(linkDict.First().Value);
                    return;
                }
                var result = await Application.Current.MainPage.DisplayActionSheet("Open link", /* TODO: Translate */
                    "Cancel", null, linkDict.Select(x => x.Key).ToArray());
                var link = linkDict.FirstOrDefault(x => x.Key == result);
                NavigateToLink(link.Value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        #region Rendering blocks

        private void Render(Block block)
        {
            switch (block)
            {
                case HeadingBlock heading:
                    Render(heading);
                    break;
                case ParagraphBlock paragraph:
                    Render(paragraph);
                    break;
                case QuoteBlock quote:
                    Render(quote);
                    break;
                case CodeBlock code:
                    Render(code);
                    break;
                case ListBlock list:
                    Render(list);
                    break;
                case ThematicBreakBlock thematicBreak:
                    Render(thematicBreak);
                    break;
                case HtmlBlock html:
                    Render(html);
                    break;
                default:
                    Debug.WriteLine($"Can't render {block.GetType()} blocks.");
                    break;
            }

            if (!_queuedViews.Any())
                return;

            foreach (var view in _queuedViews)
                _stackLayout.Children.Add(view);

            _queuedViews.Clear();
        }

        private void Render(ThematicBreakBlock _)
        {
            var separatorStyle = Theme.Separator;
            if (separatorStyle.BorderSize > 0)
                _stackLayout.Children.Add(new BoxView
                {
                    HeightRequest = separatorStyle.BorderSize,
                    BackgroundColor = separatorStyle.BorderColor,
                });
        }

        private void Render(ListBlock block)
        {
            _listScope++;
            for (var i = 0; i < block.Count(); i++)
                if (block.ElementAt(i) is ListItemBlock itemBlock)
                    Render(block, i + 1, itemBlock);

            _listScope--;
        }

        private void Render(ListBlock parent, int index, ListItemBlock block)
        {
            var initialStack = _stackLayout;
            _stackLayout = new StackLayout { Spacing = Theme.Margin };

            Render(block.AsEnumerable());

            var horizontalStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(_listScope * Theme.Margin, 0, 0, 0),
            };

            var bullet = parent.IsOrdered ? new Label
            {
                Text = $"{index}.",
                FontSize = Theme.Paragraph.FontSize,
                TextColor = Theme.Paragraph.ForegroundColor,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End,
            } : (View)new BoxView
            {
                WidthRequest = 4,
                HeightRequest = 4,
                Margin = new Thickness(0, 6, 0, 0),
                BackgroundColor = Theme.Paragraph.ForegroundColor,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
            };
            horizontalStack.Children.Add(bullet);
            horizontalStack.Children.Add(_stackLayout);
            initialStack.Children.Add(horizontalStack);

            _stackLayout = initialStack;
        }

        private void Render(HeadingBlock block)
        {
            MarkdownStyle style;
            switch (block.Level)
            {
                case 1:
                    style = Theme.Heading1;
                    break;
                case 2:
                    style = Theme.Heading2;
                    break;
                case 3:
                    style = Theme.Heading3;
                    break;
                case 4:
                    style = Theme.Heading4;
                    break;
                case 5:
                    style = Theme.Heading5;
                    break;
                default:
                    style = Theme.Heading6;
                    break;
            }

            var foregroundColor = _isQuoted ? Theme.Quote.ForegroundColor : style.ForegroundColor;

            var label = new Label
            {
                FormattedText = CreateFormatted(block.Inline, style.FontFamily, style.Attributes, foregroundColor, style.BackgroundColor, style.FontSize),
            };
            AttachLinks(label);

            if (style.BorderSize > 0)
            {
                var headingStack = new StackLayout();
                headingStack.Children.Add(label);
                headingStack.Children.Add(new BoxView
                {
                    HeightRequest = style.BorderSize,
                    BackgroundColor = style.BorderColor,
                });
                _stackLayout.Children.Add(headingStack);
            }
            else
                _stackLayout.Children.Add(label);
        }

        private void Render(LeafBlock block)
        {
            var style = Theme.Paragraph;
            var foregroundColor = _isQuoted ? Theme.Quote.ForegroundColor : style.ForegroundColor;
            var label = new Label
            {
                FormattedText = CreateFormatted(block.Inline, style.FontFamily, style.Attributes, foregroundColor, style.BackgroundColor, style.FontSize),
            };
            AttachLinks(label);
            _stackLayout.Children.Add(label);
        }

        private void Render(HtmlBlock _)
        {
            // Use WkWebView?
        }

        private void Render(QuoteBlock block)
        {
            var initialIsQuoted = _isQuoted;
            var initialStack = _stackLayout;

            _isQuoted = true;
            _stackLayout = new StackLayout { Spacing = Theme.Margin };

            var style = Theme.Quote;
            if (style.BorderSize > 0)
            {
                var horizontalStack = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    BackgroundColor = Theme.Quote.BackgroundColor,
                };

                horizontalStack.Children.Add(new BoxView
                {
                    WidthRequest = style.BorderSize,
                    BackgroundColor = style.BorderColor,
                });

                horizontalStack.Children.Add(_stackLayout);
                initialStack.Children.Add(horizontalStack);
            }
            else
            {
                _stackLayout.BackgroundColor = Theme.Quote.BackgroundColor;
                initialStack.Children.Add(_stackLayout);
            }

            Render(block.AsEnumerable());

            _isQuoted = initialIsQuoted;
            _stackLayout = initialStack;
        }

        private void Render(CodeBlock block)
        {
            var style = Theme.Code;
            var label = new Label
            {
                TextColor = style.ForegroundColor,
                FontAttributes = style.Attributes,
                FontFamily = style.FontFamily,
                FontSize = style.FontSize,
                Text = string.Join(Environment.NewLine, block.Lines),
            };
            _stackLayout.Children.Add(new Frame
            {
                CornerRadius = 3,
                HasShadow = false,
                Padding = Theme.Margin,
                BackgroundColor = style.BackgroundColor,
                Content = label
            });
        }

        private FormattedString CreateFormatted(ContainerInline inlines, string family, FontAttributes attributes, Color foregroundColor, Color backgroundColor, float size)
        {
            var fs = new FormattedString();
            var sp = inlines.Select(inline => CreateSpans(inline, family, attributes, foregroundColor, backgroundColor, size))
                .Where(spans => spans != null)
                .SelectMany(spans => spans);

            foreach (var span in sp)
                fs.Spans.Add(span);

            return fs;
        }

        private Span[] CreateSpans(Inline inline, string family, FontAttributes attributes, Color foregroundColor, Color backgroundColor, float size)
        {
            var spans = Array.Empty<Span>();
            switch (inline)
            {
                case LiteralInline literal:
                    spans = new[]
                    {
                        new Span
                        {
                            Text = literal.Content.Text.Substring(literal.Content.Start, literal.Content.Length),
                            FontAttributes = attributes,
                            ForegroundColor = foregroundColor,
                            BackgroundColor = backgroundColor,
                            FontSize = size,
                            FontFamily = family,
                        }
                    };
                    break;
                case EmphasisInline emphasis:
                    var childAttributes = attributes | (emphasis.DelimiterCount == 2 ? FontAttributes.Bold : FontAttributes.Italic);
                    switch (childAttributes)
                    {
                        case FontAttributes.None:
                            family = Theme.FontFamily;
                            //family = string.IsNullOrWhiteSpace(family) ? Theme.FontFamily : family;
                            break;
                        case FontAttributes.Bold:
                            family = Theme.FontFamilyBold;
                            //family = string.IsNullOrWhiteSpace(family) ? Theme.FontFamilyBold : family;
                            break;
                        case FontAttributes.Italic:
                            family = Theme.FontFamilyItalic;
                            //family = string.IsNullOrWhiteSpace(family) ? Theme.FontFamilyItalic : family;
                            break;
                        default:
                            family = Theme.FontFamily;
                            break;
                    }
                    spans = emphasis.SelectMany(x => CreateSpans(x, family, childAttributes, foregroundColor, backgroundColor, size)).ToArray();
                    break;
                case LineBreakInline _:
                    spans = new[] { new Span { Text = "\n" } };
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

                        _queuedViews.Add(image);
                    }
                    else
                    {
                        spans = link.SelectMany(x => CreateSpans(x, Theme.Link.FontFamily ?? family, Theme.Link.Attributes, Theme.Link.ForegroundColor, Theme.Link.BackgroundColor, size)).ToArray();
                        _links.Add(new KeyValuePair<string, string>(string.Join(string.Empty, spans.Select(x => x.Text)), url));
                    }
                    break;
                case CodeInline code:
                    spans = new[]
                    {
                        new Span
                        {
                            Text="\u2002",
                            FontSize = size,
                            FontFamily = Theme.Code.FontFamily,
                            ForegroundColor = Theme.Code.ForegroundColor,
                            BackgroundColor = Theme.Code.BackgroundColor
                        },
                        new Span
                        {
                            Text = code.Content,
                            FontAttributes = Theme.Code.Attributes,
                            FontSize = size,
                            FontFamily = Theme.Code.FontFamily,
                            ForegroundColor = Theme.Code.ForegroundColor,
                            BackgroundColor = Theme.Code.BackgroundColor
                        },
                        new Span
                        {
                            Text="\u2002",
                            FontSize = size,
                            FontFamily = Theme.Code.FontFamily,
                            ForegroundColor = Theme.Code.ForegroundColor,
                            BackgroundColor = Theme.Code.BackgroundColor
                        },
                    };
                    break;
                default:
                    Debug.WriteLine($"Can't render {inline.GetType().FullName} inlines.");
                    break;
            }
            return spans;
        }

        #endregion
    }
}
