using System;
using Xamarin.Forms;
using Parse;

#if __IOS__
using cravery.iOS;
#endif

namespace cravery
{
	public class App : Xamarin.Forms.Application
	{
		public static Installation Installation { get; set; }

		public static void Init ()
		{
			ParseObject.RegisterSubclass<User> ();
			ParseObject.RegisterSubclass<Recipe> ();
			ParseObject.RegisterSubclass<ActivityInterest> ();
			ParseObject.RegisterSubclass<Installation> ();
			ParseClient.Initialize ("kdY8lN9d4i4mhR4GJ09AxoIrFYb9Z4NFQUSAdZcc", "1qWHpwLM979tZEo1kpXXZ47a1c8wEhQCJ0ROPGSj");

			var storedInstallation = Settings.InstallationID;
			if (storedInstallation != null) {
				Installation = ParseObject.CreateWithoutData<Installation> (storedInstallation);
				Installation.Init ();
			} else {
				Installation = new Installation ();
				Installation.Init ();
				Installation.Owner = AccountManager.CurrentUser;
				#if __IOS__
				//AppDelegate.RegisterForNotifications();
				#endif
			}
		}

		public App()
		{
			Init ();

			MainPage = new TabbedPage {
				Children = {
					new InspirationPage {Icon = "world_times"},
					new DebugPage() { Icon = "sliders_up_2" }
				},

			};

			if (!AccountManager.IsLoggedIn) {
				MainPage.Navigation.PushModalAsync (new RegistrationPage ());
			}
		}
	}
}

