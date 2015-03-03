using Parse;

namespace cravery
{
	[ParseClassName("ActivityInterest")]
	public class ActivityInterest : ParseObject
	{
		[ParseFieldName("user")]
		public User User {
			get { return GetProperty<User>(); }
			set { SetProperty<User> (value); }
		}

		[ParseFieldName("recipe")]
		public Recipe Recipe {
			get { return GetProperty<Recipe>(); }
			set { SetProperty<Recipe> (value); }
		}
	}
}

