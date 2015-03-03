using Parse;
using System.Threading.Tasks;

namespace cravery
{
	public static class AccountManager
	{
		public static bool IsLoggedIn {
			get { return ParseUser.CurrentUser != null; }
		}

		public static User CurrentUser {
			get { return (User) ParseUser.CurrentUser; }
		}

		public static string CurrentUsername {
			get {
				return ParseUser.CurrentUser != null ? ParseUser.CurrentUser.Username : null;
			}
		}



		public async static Task<bool> LoginUser (string phoneNumber, string password) {
			try {
				await ParseUser.LogInAsync(phoneNumber, password);
				if (App.Installation.Id != null) {
					App.Installation.Owner = CurrentUser;
					await App.Installation.SaveAsync ();
				}
				return true;
			} catch {
				// The login failed. Check the error to see why.
				return false;
			}
		}

		public static async Task Logout() {
			ParseUser.LogOut ();
			App.Installation.Owner = null;
			if (App.Installation.Id != null) {
				await App.Installation.SaveAsync ();
			}
		}
	}
}

