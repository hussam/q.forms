using Xamarin.Forms;
using System;

namespace Q
{
	public class QCardDeckView : ScrollView
	{
		readonly StackLayout layout;
		public QCardDeckView ()
		{
			Orientation = ScrollOrientation.Horizontal;
			HorizontalOptions = LayoutOptions.FillAndExpand;
			VerticalOptions = LayoutOptions.FillAndExpand;
			layout = new StackLayout {
				Spacing = 0,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};
			Content = layout;
		}

		protected override void OnSizeAllocated (double width, double height)
		{
			base.OnSizeAllocated (width, height);
			foreach (var child in layout.Children) {
				child.WidthRequest = width;
			}
		}

		public void AddQItem (QItem item)
		{
			var card = new QCardView (item);
			card.WidthRequest = Width;
			layout.Children.Add (card);
		}

		public void Clear()
		{
			layout.Children.Clear ();
		}
	}
}

