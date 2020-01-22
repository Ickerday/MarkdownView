using Markdig.Syntax;
using Xamarin.Forms;

namespace MarkdownView.Factories.Views
{
    /// <summary>
    /// TODO
    /// </summary>
    public class HtmlBlockFactory
    {
        /// <summary>
        /// TODO
        /// </summary>
        private View Create(HtmlBlock _)
        {
            var htmlSource = new HtmlWebViewSource { Html = _.ToString() };

            var webView = new WebView { Source = htmlSource, };

            return webView;
        }
    }
}
