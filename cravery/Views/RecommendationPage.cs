using Xamarin.Forms;
using Parse;
using System.Collections.Generic;

namespace cravery
{
	public class RecommendationPage : ContentPage
	{
		Label cravingProfile, recipeName;
		Image recipePicture;

		public RecommendationPage ()
		{
			Title = "Recommendations";

			cravingProfile = new Label ();
			recipeName = new Label ();

			recipePicture = new Image {
				Aspect = Aspect.AspectFill,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			Content = new StackLayout {
				Padding = new Thickness(0, Device.OnPlatform(20,0,0), 0, 0),
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					new Label { Text = "Craving Profile", FontAttributes = FontAttributes.Bold },
					cravingProfile,
					new Label { Text = "Suggested Recipe", FontAttributes = FontAttributes.Bold },
					recipeName,
					recipePicture
				}
			};
		}

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();
			var result = await ParseCloud.CallFunctionAsync<Dictionary<string, object>> ("getRecommendation", new Dictionary<string, object> ());

			var recipe = (Recipe) result ["recipe"];
			recipePicture.Source = recipe.Picture;
			recipeName.Text = recipe.Name;

			var profile = (Dictionary<string, object>)result ["profile"];
			foreach (var k in profile.Keys) {
				cravingProfile.Text += string.Format ("{0} - {1:P0}\n", k, (double) profile[k]);
			}
		}
	}
}

