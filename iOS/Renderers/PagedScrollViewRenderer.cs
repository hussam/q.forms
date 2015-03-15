using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

using Q;
using Q.iOS;

[assembly: ExportRenderer(typeof(QCardDeckView), typeof(PagedScrollViewRenderer))]
namespace Q.iOS
{
	public class PagedScrollViewRenderer : ScrollViewRenderer
	{
		protected override void OnElementChanged (VisualElementChangedEventArgs e)
		{
			base.OnElementChanged (e);

			var sv = (UIScrollView) NativeView;
			sv.PagingEnabled = true;
			sv.ShowsHorizontalScrollIndicator = false;

			return;
		}
	}
}