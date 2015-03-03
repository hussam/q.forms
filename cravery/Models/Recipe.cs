using System;
using Parse;

namespace cravery
{
	[ParseClassName("Recipe")]
	public class Recipe : ParseObject
	{
		public string Id {
			get { return ObjectId; }
			set { ObjectId = value; }
		}

		[ParseFieldName("picture")]
		public Uri Picture {
			get {
				return GetProperty<ParseFile> ().Url;
			}
		}

		[ParseFieldName("name")]
		public string Name {
			get { return GetProperty<string> (); }
			set { SetProperty<string> (value); }
		}
	}
}

