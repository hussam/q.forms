using Xamarin.Forms;

using cravery;
using cravery.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRendererAttribute(typeof(FeedListView), typeof(InspirationListViewRenderer))]
namespace cravery.iOS
{
	public class InspirationListViewRenderer : ListViewRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<ListView> e)
		{
			base.OnElementChanged (e);

			if (Control == null) {
				return;
			} else {
				Control.ScrollEnabled = false;
				Control.AllowsSelection = false;
			}
		}
	}
}

