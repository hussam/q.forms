#if __IOS__
using Foundation;
#endif


namespace Q
{
	public static class Settings
	{
		internal static void Clear()
		{
			#if __IOS__
			var defaults = NSUserDefaults.StandardUserDefaults;
			defaults.RemoveObject("currentInstallation");
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

		public static string FontName
		{
			get {
				#if __IOS__
				return "AvenirNextCondensed-Medium";
				#endif
			}
		}

		public static string FontNameItalic
		{
			get {
				#if __IOS__
				return "AvenirNextCondensed-MediumItalic";
				#endif
			}
		}



		public static string FontNameBold
		{
			get {
				#if __IOS__
				return "AvenirNextCondensed-DemiBold";
				#endif
			}
		}

		public static string FontNameBoldItalic
		{
			get {
				#if __IOS__
				return "AvenirNextCondensed-DemiBoldItalic";
				#endif
			}
		}
	}
}

