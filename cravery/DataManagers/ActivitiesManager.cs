using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Parse;
using System.Threading.Tasks;

namespace cravery
{
	public static class ActivitiesManager
	{
		const int PAGINATION = 50;

		static ObservableCollection<Recipe> inspiration = null;

		public static async Task<ObservableCollection<Recipe>> GetInspirationList()
		{
			if (inspiration == null) {
				var query = new ParseQuery<Recipe> ().Limit(PAGINATION);
				var results = await query.FindAsync ();
				inspiration = new ObservableCollection<Recipe> (results);
				inspiration.CollectionChanged += async (sender, e) => {
					if (inspiration.Count == 25) {
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
	}
}

