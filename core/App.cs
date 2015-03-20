using System.IO;
using System;
using Xamarin.Forms;

#if __IOS__
using Q.iOS;
#endif

namespace Q
{
	public class App : Xamarin.Forms.Application
	{
		public static Installation Installation { get; set; }
		public static QItemDatabase Database;

		public static async void Init (string dbPath)
		{
			#if DEBUG
			Console.WriteLine(dbPath);
			#endif
			Directory.CreateDirectory (dbPath);
			Database = new QItemDatabase (Path.Combine(dbPath, "queue.sqlite"));
			await Database.CreateTable ();

			/*
			ParseObject.RegisterSubclass<User> ();
			ParseObject.RegisterSubclass<Recipe> ();
			ParseObject.RegisterSubclass<ActivityInterest> ();
			ParseObject.RegisterSubclass<Installation> ();
			ParseClient.Initialize("CMqy2LTcmnriJJXbFKuJJjpykJ66DPApFeQGAC5A", "ujucyDXCXn6z4E9wvQJNib6SyhKfuAjpqkzM2J88");


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
			*/
		}

		public App()
		{
			MainPage = new MainPage ();
		}
	}
}

