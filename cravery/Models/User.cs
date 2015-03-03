using Parse;

namespace cravery
{
	[ParseClassName("_User")]
	public class User : ParseUser
	{
		public string Id {
			get { return ObjectId; }
		}
	}
}

