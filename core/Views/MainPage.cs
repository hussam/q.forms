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

		public ObservableCollection<QItem> QItems {get; set;}

		int nextIndex;
		QCardDeckView[] QItemContainers;

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
					

			QItemContainers = new QCardDeckView[3];
			for (var i = 0; i < 3; i++) {
				QItemContainers [i] = new QCardDeckView ();
			}

			nextIndex = 0;

			/* XXX FIXME : I should be doing this instead of refreshing OnAppear()
			App.Database.GetItems ().ContinueWith ((t) => {
				nextIndex = 0;
				QItems = t.Result;
				foreach (var q in QItems) {
					Console.WriteLine(q.Text);
					QItemContainers[nextIndex].AddQItem(q);
					nextIndex = ++nextIndex % 3;
				}
				QItems.CollectionChanged += (sender, e) => {
					foreach (var item in e.NewItems) {
						QItem q = (QItem) item;
						Console.WriteLine("Adding {0} to {1}", q.Text, nextIndex);
						QItemContainers[nextIndex].AddQItem(q);
						nextIndex = ++nextIndex % 3;
					}
				};
			});
			*/
			

			Content = new StackLayout {
				Padding = new Thickness(0,0,0,10),
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					topNav,
					new Label {
						Text = "Now",
						FontSize = 20,
						FontFamily = Settings.FontName,
						HeightRequest = 50,
						YAlign = TextAlignment.End,
						HorizontalOptions = LayoutOptions.Start
					},
					QItemContainers[0],

					new Label {
						Text = "Later",
						FontSize = 20,
						FontFamily = Settings.FontName,
						HeightRequest = 50,
						YAlign = TextAlignment.End,
						HorizontalOptions = LayoutOptions.Start
					},
					QItemContainers[1],

					new Label {
						Text = "Much Later",
						FontSize = 20,
						FontFamily = Settings.FontName,
						HeightRequest = 50,
						YAlign = TextAlignment.End,
						HorizontalOptions = LayoutOptions.Start
					},
					QItemContainers[2],
				}
			};
		}

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();
			// XXX HACK!!!
			foreach (var qic in QItemContainers) {
				qic.Clear ();
			}

			QItems = await App.Database.GetItems ();
			foreach (var q in QItems) {
				QItemContainers[nextIndex].AddQItem(q);
				nextIndex = ++nextIndex % 3;
			}
		}
	}
}

