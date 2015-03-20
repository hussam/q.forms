using System;
using Xamarin.Forms;

namespace Q
{
	public class QCardView : AbsoluteLayout
	{
		public QCardView (QItem item)
		{
			Padding = new Thickness (10, 0, 0, 5);
			BackgroundColor = Color.FromHex ("F6EBC9");

			if (item.PhotoUrl != null) {
				var img = new Image ();
				img.Source = item.PhotoUrl;
				img.Aspect = Aspect.AspectFill;

				AbsoluteLayout.SetLayoutFlags (img, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (img, new Rectangle (0, 0, 1, 1));
				Children.Add (img);
			}

			var title = new Label ();
			title.VerticalOptions = LayoutOptions.FillAndExpand;
			title.YAlign = TextAlignment.End;
			title.Text = item.Text;
			title.FontFamily = Settings.FontNameBoldItalic;
			title.FontSize = 24;
			title.HeightRequest = 40;
			title.BackgroundColor = Color.White;
			title.Opacity = 0.75;
			AbsoluteLayout.SetLayoutFlags (title, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (title, new Rectangle (0, 0.5, 0.5, 0.25));
			Children.Add (title);



			var hashtag = new Label ();
			hashtag.VerticalOptions = LayoutOptions.FillAndExpand;
			hashtag.YAlign = TextAlignment.Start;
			hashtag.Text = item.Hashtag;
			hashtag.FontFamily = Settings.FontNameItalic;
			hashtag.FontSize = 20;
			hashtag.BackgroundColor = Color.White;
			hashtag.Opacity = 0.75;
			AbsoluteLayout.SetLayoutFlags (hashtag, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (hashtag, new Rectangle (0, 0.75, 0.25, 0.125));
			Children.Add (hashtag);
		}
	}
}

