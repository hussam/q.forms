using Xamarin.Forms;
using System;
using Q.Foursquare;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;

namespace Q
{
	public class QItemCreationPage : ContentPage
	{
		StyledEntry entry = new StyledEntry();
		Label counter = new Label();
		Label hashtag = new Label();

		Color[] colors = { Color.Aqua, Color.Blue, Color.Fuchsia, Color.Green, Color.Lime, Color.Maroon, Color.Navy, Color.Olive, Color.Pink, Color.Purple, Color.Red, Color.Teal, Color.Yellow };
		private static Color HashtagColor = Color.FromHex("FFD300");

		Grid hashtagsGrid;
		StackLayout mainLayout, entryLayout;

		AutoSuggestionsView<Venue> suggestions;

		Venue selectedVenue;

		public QItemCreationPage ()
		{
			var random = new Random ();
			var highlightColor = colors [random.Next (0, colors.Length)].WithLuminosity(0.75);

			var prompt = new Label ();
			prompt.HorizontalOptions = LayoutOptions.Start;
			prompt.FontFamily = Settings.FontNameItalic;
			prompt.BackgroundColor = highlightColor;
			prompt.FontSize = 24;
			prompt.Text = " REMIND ME TO GO FOR A ";

			var dismiss = new Label ();
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
			entry.TextChanged += fetchSuggestions;

			counter.HorizontalOptions = LayoutOptions.End;
			counter.FontFamily = Settings.FontNameBold;
			counter.FontSize = 24;
			counter.Text = entry.MaxTextLength.ToString ();
			counter.YAlign = TextAlignment.Center;

			suggestions = new AutoSuggestionsView<Venue> (3);
			suggestions.SuggestionClicked += (suggestion) => {
				var venue = (Venue) suggestion;
				counter.IsVisible = false;
				entry.EnforceMaxTextLength = false;
				entry.Text = venue.Name;
				selectedVenue = venue;
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
						if (selectedVenue != null) {
							FoursquareClient.VenuePhotos(selectedVenue.Id, 1).ContinueWith( t => {
								App.Database.SaveItem( new QItem {
									Text = entry.Text,
									Hashtag = hashtag.Text,
									VenueId = selectedVenue.Id,
									PhotoUrl = t.Result[0].Url(500)
								});
							});
						} else {
							App.Database.SaveItem( new QItem {
								Text = entry.Text,
								Hashtag = hashtag.Text
							});
						}
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
						Padding = new Thickness(10,0,10,0),
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
					suggestions,
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
			int remainingCharacters = entry.MaxTextLength;
			if (entry.Text != null) {
				remainingCharacters = entry.MaxTextLength - entry.Text.Length;
			}

			if (remainingCharacters >= 0) {
				counter.IsVisible = true;
				entry.EnforceMaxTextLength = true;
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

		async void fetchSuggestions (object sender, TextChangedEventArgs e)
		{
			if (entry.Text != null && entry.Text.Length >= 3) {
				var results = await FoursquareClient.SuggestCompletion (e.NewTextValue);
				for (int i = 0; i < results.Count; i++) {
					suggestions.SetAtIndex (i, results [i].Name, results [i].Location.Address + ", " + results [i].Location.City + " " + results [i].Location.State, results [i]);
				}
			} else {
				suggestions.Clear ();
				selectedVenue = null;
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

