using System;
using Xamarin.Forms;

namespace Q
{
	public class QCardView : StackLayout
	{
		public QCardView (QItem item)
		{
			Spacing = 0;
			Padding = new Thickness (10, 0, 0, 5);
			BackgroundColor = Color.FromHex ("F6EBC9");

			var title = new Label ();
			title.VerticalOptions = LayoutOptions.FillAndExpand;
			title.YAlign = TextAlignment.End;
			title.Text = item.Text;
			title.FontFamily = Settings.FontNameBoldItalic;
			title.FontSize = 24;
			title.HeightRequest = 40;
			Children.Add (title);



			var hashtag = new Label ();
			hashtag.VerticalOptions = LayoutOptions.FillAndExpand;
			hashtag.YAlign = TextAlignment.Start;
			hashtag.Text = item.Hashtag;
			hashtag.FontFamily = Settings.FontNameItalic;
			hashtag.FontSize = 20;
			Children.Add (hashtag);
		}
	}
}

