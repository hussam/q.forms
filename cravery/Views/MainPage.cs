using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace cravery
{
	public class MainPage : ContentPage
	{
		const string MenuArrow = " ▾";

		public string ContentTitle { get; set; }
		ListView cravings;

		public ObservableCollection<Craving> Cravings {get; set;}

		public MainPage ()
		{
			ContentTitle = "Feed";

			var titleLabel = new Label {
				Text = ContentTitle + MenuArrow,
				TextColor = Color.White,
				FontFamily = Settings.FontNameBold,
				FontSize = 24,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				XAlign = TextAlignment.Center
			};
			AbsoluteLayout.SetLayoutFlags (titleLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (titleLabel, new Rectangle (0, 0, 1, 1));

			var create = new Label {
				Text = "/",
				TextColor = Color.White,
				FontFamily = Settings.FontNameBoldItalic,
				FontSize = 24,
				HorizontalOptions = LayoutOptions.End,
				XAlign = TextAlignment.End
			};
			create.GestureRecognizers.Add( new TapGestureRecognizer {
				Command = new Command(() => Navigation.PushModalAsync(new CravingCreationPage()))
			});
			AbsoluteLayout.SetLayoutFlags (create, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.HeightProportional);
			AbsoluteLayout.SetLayoutBounds (create, new Rectangle (1, 0, 20, 1));

			var topNav = new AbsoluteLayout {
				VerticalOptions = LayoutOptions.Start,
				Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 0),
				BackgroundColor = Color.Green.WithSaturation(0.5).WithLuminosity(0.75),
				Children = {titleLabel, create}
			};

			cravings = new ListView ();
			cravings.ItemTemplate = new DataTemplate (typeof(TextCell));
			cravings.ItemTemplate.SetBinding (TextCell.TextProperty, "Text");
			cravings.ItemTemplate.SetBinding (TextCell.DetailProperty, "Hashtag");
			App.Database.GetCravings().ContinueWith(t =>
				cravings.ItemsSource = t.Result
			);

			Content = new StackLayout {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					topNav,
					cravings
				}
			};
		}
	}
}

