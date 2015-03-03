#if __IOS__
using Foundation;
#endif


namespace cravery
{
	public class Settings
	{
		internal static void Clear()
		{
			#if __IOS__
			var defaults = NSUserDefaults.StandardUserDefaults;
			defaults.RemoveObject("currentInstallation");
			defaults.RemoveObject("contactsFetched");
			defaults.RemoveObject("contactsAlertDisplayed");
			#endif
		}


		public static string InstallationID 
		{
			get {
				#if __IOS__
				return NSUserDefaults.StandardUserDefaults.StringForKey ("currentInstallation");
				#endif
			}
			set {
				#if __IOS__
				NSUserDefaults.StandardUserDefaults.SetString(value, "currentInstallation");
				#endif
			}
		}

		public static bool ContactsFetched
		{
			get {
				#if __IOS__
				return NSUserDefaults.StandardUserDefaults.BoolForKey ("contactsFetched");
				#endif
			}
			set {
				#if __IOS__
				NSUserDefaults.StandardUserDefaults.SetBool(value, "contactsFetched");
				#endif
			}
		}

		public static bool ContactsAlertDisplayed
		{
			get {
				#if __IOS__
				return NSUserDefaults.StandardUserDefaults.BoolForKey ("contactsAlertDisplayed");
				#endif
			}
			set {
				#if __IOS__
				NSUserDefaults.StandardUserDefaults.SetBool(value, "contactsAlertDisplayed");
				#endif
			}
		}
	}
}

