using Xamarin.Forms;

namespace cravery
{
	public class RecommendationPage : ContentPage
	{
		ListView cravingsList;

		public RecommendationPage ()
		{
			Title = "Recommendations";

			var ingredient1 = new Label {
				Text = "Chicken - 73%"
			};
			var ingredient2 = new Label {
				Text = "Pesto - 50%"
			};
			var ingredient3 = new Label {
				Text = "Vegetable - 30%"
			};

			var suggestedRecipe = new Image {
				HeightRequest = 120,
				Aspect = Aspect.AspectFill,
				Source = "http://files.parsetfss.com/b980ac8f-cc75-47e8-878a-d54866ee1cb4/tfss-b8eff495-d560-4a3b-861f-93ed0d1a4587-recipe"
			};

			cravingsList = new ListView {
				ItemTemplate = new DataTemplate (typeof(CravingCell)),
				RowHeight = 40
			};

			Content = new StackLayout {
				Padding = new Thickness(0, Device.OnPlatform(20,0,0), 0, 0),
				Children = {
					new Label { Text = "Craving Profile", FontAttributes = FontAttributes.Bold },
					ingredient1,
					ingredient2,
					ingredient3,
					new Label { Text = "Suggested Recipe", FontAttributes = FontAttributes.Bold },
					suggestedRecipe,
					new Label { Text = "Based on these recent cravings", FontAttributes = FontAttributes.Bold },
					cravingsList
				}
			};
		}

		protected async override void OnAppearing ()
		{
			base.OnAppearing ();
			cravingsList.ItemsSource = await ActivitiesManager.GetLikedRecipes ();
		}
	}
}

