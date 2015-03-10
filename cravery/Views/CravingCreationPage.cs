using Xamarin.Forms;
using System;

namespace cravery
{
	public class CravingCreationPage : ContentPage
	{
		readonly StyledEntry entry, hashtag;
		readonly Label counter;

		Color[] colors = { Color.Aqua, Color.Blue, Color.Fuchsia, Color.Green, Color.Lime, Color.Maroon, Color.Navy, Color.Olive, Color.Pink, Color.Purple, Color.Red, Color.Teal, Color.Yellow };
		private static Color HashtagColor = Color.FromHex("FFD300");

		public CravingCreationPage ()
		{
			var prompt = new Label ();
			var dismiss = new Label ();
			entry = new StyledEntry ();
			hashtag = new StyledEntry ();
			counter = new Label ();

			var random = new Random ();
			var highlightColor = colors [random.Next (0, colors.Length)].WithLuminosity(0.75);

			prompt.HorizontalOptions = LayoutOptions.Start;
			prompt.FontFamily = Settings.FontNameItalic;
			prompt.BackgroundColor = highlightColor;
			prompt.FontSize = 24;
			prompt.Text = " WHAT ARE YOU CRAVING? ";

			dismiss.HorizontalOptions = LayoutOptions.EndAndExpand;
			dismiss.FontFamily = Settings.FontNameBold;
			dismiss.FontSize = 24;
			dismiss.TextColor = highlightColor.WithLuminosity(0.5).WithSaturation(0.5);
			dismiss.Text = "X";
			dismiss.XAlign = TextAlignment.End;
			dismiss.GestureRecognizers.Add (
				new TapGestureRecognizer {
					Command = new Command (() => Navigation.PopModalAsync ())
				}
			);

			entry.FontSize = 24;
			entry.LeftSpacing = 20;
			entry.Placeholder = "...";
			entry.MaxTextLength = 20;
			entry.TintColor = highlightColor;
			entry.ReturnKey = StyledEntry.KeyboardReturnKey.Next;
			entry.TextChanged += counterUpdater;
			entry.Focused += (sender, e) => {
				counterUpdater(sender, null);
			};


			hashtag.FontSize = 20;
			hashtag.Placeholder = "_______";
			hashtag.MaxTextLength = 10;
			hashtag.TintColor = HashtagColor;
			hashtag.ReturnKey = StyledEntry.KeyboardReturnKey.Done;
			hashtag.HorizontalOptions = LayoutOptions.FillAndExpand;
			hashtag.TextChanged += counterUpdater;
			hashtag.Focused += (sender, e) => {
				counterUpdater(sender, null);
			};

			counter.HorizontalOptions = LayoutOptions.End;
			counter.FontFamily = Settings.FontNameBold;
			counter.FontSize = 24;
			counter.Text = entry.MaxTextLength.ToString ();
			counter.YAlign = TextAlignment.Center;

			var hashtagLayout = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(0,0,0,40),
				Children = {
					new Label {
						HorizontalOptions = LayoutOptions.Start,
						Text = " #",
						TextColor = HashtagColor,
						FontFamily = Settings.FontNameBold,
						FontSize = 40,
						XAlign = TextAlignment.End,
						YAlign = TextAlignment.End
					},
					hashtag,
					counter
				}
			};

			var topLayout = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Spacing = 20,
				Children = { prompt, dismiss }
			};

			var save = new Label ();
			save.HorizontalOptions = LayoutOptions.End;
			save.FontFamily = Settings.FontNameBoldItalic;
			save.FontSize = 24;
			save.BackgroundColor = highlightColor;
			save.Text = "SAVE IT!";
			save.WidthRequest = 160;
			save.XAlign = TextAlignment.Center;
			save.GestureRecognizers.Add (
				new TapGestureRecognizer {
					Command = new Command ((obj) => {
						App.Database.SaveCraving( new Craving {
							Text = entry.Text,
							Hashtag = hashtag.Text
						});
						Navigation.PopModalAsync ();
					})
				}
			);
					
			Content = new StackLayout {
				Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 10, 0),
				Spacing = 10,
				Children = {
					topLayout,
					entry,
					hashtagLayout,
					save
				}
			};
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			entry.Focus ();
		}

		void counterUpdater (object sender, TextChangedEventArgs e)
		{
			var _sender = (StyledEntry)sender;

			int remainingCharacters = _sender.MaxTextLength;
			if (_sender.Text != null) {
				remainingCharacters = _sender.MaxTextLength - _sender.Text.Length;
			}

			counter.Text = remainingCharacters.ToString();
			if (remainingCharacters > 5) {
				counter.TextColor = Color.Black;
				counter.BackgroundColor = Color.Transparent;
			} else {
				counter.TextColor = Color.White;
				counter.BackgroundColor = Color.Red.WithSaturation(0.5);
			}
		}
	}
}

