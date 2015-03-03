﻿#if __IOS__
using Foundation;
#endif


namespace cravery
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
	}
}

