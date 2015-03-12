using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using Parse;

namespace Q.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// store the database in the Application Support folder
			var appSupportDir = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User) [0];
			App.Init (appSupportDir.Path);

			global::Xamarin.Forms.Forms.Init ();

			LoadApplication (new App ());

			// Process any potential notification data from launch
			//ProcessNotification (options, true);

			return base.FinishedLaunching (app, options);
		}


		public static void RegisterForNotifications ()
		{
			var settings = UIUserNotificationSettings.GetSettingsForTypes (
				UIUserNotificationType.Sound |
				UIUserNotificationType.Alert |
				UIUserNotificationType.Badge, new NSSet ());

			UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
			UIApplication.SharedApplication.RegisterForRemoteNotifications ();
		}

		public override async void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			App.Installation.AppIdentifier = NSBundle.MainBundle.BundleIdentifier;
			App.Installation.AppName = NSBundle.MainBundle.InfoDictionary ["CFBundleDisplayName"].ToString();
			App.Installation.AppVersion = NSBundle.MainBundle.InfoDictionary ["CFBundleVersion"].ToString();

			App.Installation.DeviceType = "ios";
			App.Installation.DeviceToken = deviceToken.ToString ().Replace ("<", "").Replace (">", "").Replace (" ", "");

			await App.Installation.SaveAsync ().ContinueWith (t => {
				if (t.IsFaulted) {
					using (IEnumerator<System.Exception> enumerator = t.Exception.InnerExceptions.GetEnumerator()) {
						if (enumerator.MoveNext()) {
							ParseException error = (ParseException) enumerator.Current;
							Console.WriteLine (error.Message);
						}
					}
				} else {
					Settings.InstallationID = App.Installation.Id;
				}
			});
		}

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			// Process a notification received while the app was already open
			ProcessNotification (userInfo, false);
		}

		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			Console.WriteLine (error.Description);
		}

		void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
		{
			// Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (null != options && options.ContainsKey(new NSString("aps")))
			{
				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

				string alert = string.Empty;

				//Extract the alert text
				// NOTE: If you're using the simple alert by just specifying 
				// "  aps:{alert:"alert msg here"}  " this will work fine.
				// But if you're using a complex alert with Localization keys, etc., 
				// your "alert" object from the aps dictionary will be another NSDictionary. 
				// Basically the json gets dumped right into a NSDictionary, 
				// so keep that in mind.
				if (aps.ContainsKey(new NSString("alert")))
					alert = (aps [new NSString("alert")] as NSString).ToString();

				//If this came from the ReceivedRemoteNotification while the app was running,
				// we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching)
				{
					//Manually show an alert
					if (!string.IsNullOrEmpty(alert))
					{
						UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
						avAlert.Show();
					}
				}           
			}
		}
	}
}

