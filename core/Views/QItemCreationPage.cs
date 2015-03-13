using Xamarin.Forms;
using System;

namespace Q
{
	public class QItemCreationPage : ContentPage
	{
		readonly StyledEntry entry;
		readonly Label counter, hashtag;

		Color[] colors = { Color.Aqua, Color.Blue, Color.Fuchsia, Color.Green, Color.Lime, Color.Maroon, Color.Navy, Color.Olive, Color.Pink, Color.Purple, Color.Red, Color.Teal, Color.Yellow };
		private static Color HashtagColor = Color.FromHex("FFD300");

		Grid hashtagsGrid;
		StackLayout mainLayout, entryLayout;

		public QItemCreationPage ()
		{
			var prompt = new Label ();
			var dismiss = new Label ();
			hashtag = new Label ();
			entry = new StyledEntry ();
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
			dismiss.XAlign = TextAlignment.Center;
			dismiss.WidthRequest = 40;
			dismiss.GestureRecognizers.Add (
				new TapGestureRecognizer {
					Command = new Command (() => Navigation.PopModalAsync ())
				}
			);

			hashtag.FontSize = 28;
			hashtag.FontFamily = Settings.FontNameBoldItalic;
			hashtag.BackgroundColor = HashtagColor;
			hashtag.YAlign = TextAlignment.Center;
			hashtag.HorizontalOptions = LayoutOptions.Start;

			entry.FontSize = 24;
			entry.Placeholder = "...(optional)";
			entry.MaxTextLength = 20;
			entry.TintColor = highlightColor;
			entry.ReturnKey = StyledEntry.KeyboardReturnKey.Next;
			entry.HorizontalOptions = LayoutOptions.FillAndExpand;
			entry.TextChanged += counterUpdater;
			entry.Focused += (sender, e) => {
				counterUpdater(sender, null);
			};


			counter.HorizontalOptions = LayoutOptions.End;
			counter.FontFamily = Settings.FontNameBold;
			counter.FontSize = 24;
			counter.Text = entry.MaxTextLength.ToString ();
			counter.YAlign = TextAlignment.Center;

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
						App.Database.SaveItem( new QItem {
							Text = entry.Text,
							Hashtag = hashtag.Text
						});
						Navigation.PopModalAsync ();
					})
				}
			);
				
			var topLayout = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0),
				Children = { prompt, dismiss }
			};

			string[] hashtags = { "Coffee", "Beer", "Drinks", "Brunch", "Lunch", "Dinner", "Movie", "Walk" };
			hashtagsGrid = layoutOnGrid (hashtags, 3);

			entryLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(10, 0, 0, 0),
				Children = {
					hashtag,
					new StackLayout {
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness(10,0,10,80),
						Children = {
							new Label {
								Text = "at",
								FontFamily = Settings.FontNameBold,
								FontSize = 20,
								XAlign = TextAlignment.Center,
								YAlign = TextAlignment.Center
							},
							entry,
							counter
						}
					},
					save
				}
			};
					
			mainLayout = new StackLayout {
				Children = {
					topLayout,
					hashtagsGrid
				}
			};

			Content = mainLayout;
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

		Grid layoutOnGrid(string[] hashtags, int columns)
		{
			var grid = new Grid ();
			var buttons = new Button[hashtags.Length];


			for (var i = 0; i < hashtags.Length; i++) {
				buttons[i] = new Button {
					Text = hashtags[i],
					FontFamily = Settings.FontName,
					FontSize = 20,
					TextColor = Color.Black
				};
				buttons[i].Clicked += (sender, e) => {
					this.hashtag.Text = ((Button)sender).Text.ToUpper();
					mainLayout.Children.Remove(grid);
					mainLayout.Children.Add(entryLayout);
					entry.Focus();
				};
			}

			int rows = (hashtags.Length / columns) + (hashtags.Length % columns != 0 ? 1 : 0);
			for (var i = 0; i < rows; i++) {
				grid.RowDefinitions.Add (new RowDefinition ());
			}
			for (var i = 0; i < columns; i++) {
				grid.ColumnDefinitions.Add (new ColumnDefinition ());
			}
			for (var i = 0; i < buttons.Length; i++) {
				int r = i / columns;
				int c = i % columns;
				grid.Children.Add (buttons [i], c, r);
			}

			return grid;
		}
	}
}

