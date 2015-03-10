using System;
using Xamarin.Forms;

namespace cravery
{
	public class MainPage : ContentPage
	{
		public string ContentTitle { get; set; }

		public MainPage ()
		{
			ContentTitle = "Feed ▾";

			var titleLabel = new Label {
				Text = ContentTitle,
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

			Content = new StackLayout {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					topNav
				}
			};
		}
	}
}

