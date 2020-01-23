using Markdig.Syntax;
using Xamarin.Forms;

namespace MarkdownView.Factories.Views
{
    /// <summary>
    /// TODO
    /// </summary>
    public static class HtmlBlockFactory
    {
        /// <summary>
        /// TODO
        /// </summary>
        public static View Create(HtmlBlock _)
        {
            var htmlSource = new HtmlWebViewSource { Html = _.ToString() };

            var webView = new WebView { Source = htmlSource, };

            return webView;
        }
    }
}
