using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace Q
{
	public class MainPage : ContentPage
	{
		//const string MenuArrow = " ▾";
		const string MenuArrow = "⌄";

		public string ContentTitle { get; set; }
		ListView qItems;

		public ObservableCollection<QItem> QItems {get; set;}

		public MainPage ()
		{
			ContentTitle = "Feed";

			var titleLabel = new Label {
				Text = ContentTitle + MenuArrow,
				TextColor = Color.White,
				FontFamily = Settings.FontNameBold,
				FontSize = 30,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.End
			};
			AbsoluteLayout.SetLayoutFlags (titleLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (titleLabel, new Rectangle (0, 0, 1, 1));

			var create = new Label {
				Text = "//",
				TextColor = Color.White,
				FontFamily = Settings.FontNameBoldItalic,
				FontSize = 30,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.End
			};
			create.GestureRecognizers.Add( new TapGestureRecognizer {
				Command = new Command(() => Navigation.PushModalAsync(new QItemCreationPage()))
			});
			AbsoluteLayout.SetLayoutFlags (create, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.HeightProportional);
			AbsoluteLayout.SetLayoutBounds (create, new Rectangle (0.99, 0, 50, 1));

			var topNav = new AbsoluteLayout {
				VerticalOptions = LayoutOptions.Start,
				Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0),
				BackgroundColor = Color.Green.WithSaturation(0.5).WithLuminosity(0.75),
				HeightRequest = 50,
				Children = {titleLabel, create}
			};

			qItems = new ListView ();
			qItems.ItemTemplate = new DataTemplate (typeof(TextCell));
			qItems.ItemTemplate.SetBinding (TextCell.TextProperty, "Text");
			qItems.ItemTemplate.SetBinding (TextCell.DetailProperty, "Hashtag");
			App.Database.GetItems().ContinueWith(t =>
				qItems.ItemsSource = t.Result
			);

			Content = new StackLayout {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					topNav,
					qItems
				}
			};
		}
	}
}

