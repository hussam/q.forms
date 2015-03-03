using System;
using Parse;
using System.Collections.Generic;

namespace cravery
{
	[ParseClassName("_Installation")]
	public class Installation : ParseObject
	{
		public void Init()
		{
			// TODO: do it programatically
			this["timeZone"] = "UTC";
			this["parseVersion"] = "1.3.1";
		}

		public string Id
		{
			get { return ObjectId; }
			set { ObjectId = value; }
		}

		[ParseFieldName("appIdentifier")]
		public string AppIdentifier
		{
			get { return GetProperty<string>(); }
			set { SetProperty<string> (value); }
		}

		[ParseFieldName("appName")]
		public string AppName
		{
			get { return GetProperty<string>(); }
			set { SetProperty<string> (value); }
		}

		[ParseFieldName("appVersion")]
		public string AppVersion
		{
			get { return GetProperty<string>(); }
			set { SetProperty<string> (value); }
		}

		[ParseFieldName("channels")]
		public List<object> Channels
		{
			get {
				var p = GetProperty<List<Object>> ();
				if (p == null) {
					p = new List<Object> ();
					SetProperty<List<Object>> (p);
				}
				return p;
			}

			set { SetProperty<List<Object>> (value); }
		}

		[ParseFieldName("deviceType")]
		public string DeviceType
		{
			get { return GetProperty<string>(); }
			set { SetProperty<string> (value); }
		}
			

		[ParseFieldName("deviceToken")]
		public string DeviceToken
		{
			get { return GetProperty<string>(); }
			set { SetProperty<string> (value); }
		}



		[ParseFieldName("owner")]
		public User Owner
		{
			get { return GetProperty<User> (); }
			set { SetProperty<User> (value); }
		}
	}
}

