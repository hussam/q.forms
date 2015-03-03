using Xamarin.Forms;

namespace cravery
{
	public class DebugPage : ContentPage
	{
		Label currentUserLbl;
		public DebugPage ()
		{
			Title = "Debugging";

			var resetBtn = new Button { Text = "Reset Settings" };
			resetBtn.Clicked += async (sender, e) => {
				if (await DisplayAlert("Clear Settings", "Are you sure you want to reset?", "OK", "NO")) {
					Settings.Clear();
				}
			};

			currentUserLbl = new Label {
				FontAttributes = FontAttributes.Bold
			};

			var logoutBtn = new Button { Text = "Log out" };
			logoutBtn.Clicked += async (sender, e) => {
				await AccountManager.Logout();
				var regPage = new RegistrationPage();
				await Navigation.PushModalAsync(regPage);
			};
				
			Content = new StackLayout {
				Spacing = 20,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Children = {
					resetBtn,
					currentUserLbl,
					logoutBtn
				}
			};
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			currentUserLbl.Text = "Current User = " + AccountManager.CurrentUsername;
		}
	}
}

