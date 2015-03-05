using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Parse;
using System.Threading.Tasks;
using System;

namespace cravery
{
	public static class ActivitiesManager
	{
		const int PAGINATION = 10;
		const int REFRESH_MARK = PAGINATION / 2;

		static ObservableCollection<Recipe> inspiration = null;

		public static async Task<ObservableCollection<Recipe>> GetInspirationList()
		{
			if (inspiration == null) {
				var query = new ParseQuery<Recipe> ().Limit(PAGINATION);
				var results = await query.FindAsync ();
				inspiration = new ObservableCollection<Recipe> (results);
				inspiration.CollectionChanged += async (sender, e) => {
					if (inspiration.Count == REFRESH_MARK) {
						query = query.Skip(PAGINATION);
						var refresh = await query.FindAsync();
						foreach (var recipe in refresh) {
							inspiration.Add(recipe);
						}
					}
				};
			}
			return inspiration;
		}

		public static async void InterestedInActivity(Recipe recipe)
		{
			inspiration.Remove (recipe);

			var obj = new ActivityInterest {
				Recipe = recipe,
				User = AccountManager.CurrentUser
			};
			await obj.SaveAsync ();
		}

		public static async void NotInterestedInActivity(Recipe recipe)
		{
			inspiration.Remove (recipe);

			var obj = new ActivityNoInterest {
				Recipe = recipe,
				User = AccountManager.CurrentUser
			};
			await obj.SaveAsync ();
		}

		public static async Task<ObservableCollection<ActivityInterest>> GetLikedRecipes()
		{
			var query = new ParseQuery<ActivityInterest> ()
				.Include ("recipe")
				.WhereEqualTo ("user", AccountManager.CurrentUser)
				.WhereGreaterThanOrEqualTo ("createdAt", DateTime.UtcNow - TimeSpan.FromHours (1))
				.OrderByDescending ("createdAt");
			var results = await query.FindAsync ();
			return new ObservableCollection<ActivityInterest> (results);
		}
	}
}

