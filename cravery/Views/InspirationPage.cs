using Xamarin.Forms;

namespace cravery
{
	public class InspirationPage : ContentPage
	{
		InspirationListView listView;

		public InspirationPage ()
		{
			Title = "Inspiration";

			listView = new InspirationListView ();

			Content = new StackLayout {
				Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0),
				Children = { listView }
			};
		}

		protected async override void OnAppearing ()
		{
			base.OnAppearing ();
			listView.ItemsSource = await ActivitiesManager.GetInspirationList ();
		}
	}
}

