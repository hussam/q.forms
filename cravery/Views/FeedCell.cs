using Xamarin.Forms;

namespace cravery
{
	public class FeedCell : ViewCell
	{
		public FeedCell ()
		{
			var image = new Image {
				Aspect = Aspect.AspectFill,
				HeightRequest = 100,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};
			image.SetBinding (Image.SourceProperty, "Picture");

			var description = new Label {
				//FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.End,
				BackgroundColor = Color.White.MultiplyAlpha(0.65)
			};
			description.SetBinding (Label.TextProperty, "Name");

			var layout = new AbsoluteLayout ();
			AbsoluteLayout.SetLayoutFlags (image, AbsoluteLayoutFlags.SizeProportional);
			AbsoluteLayout.SetLayoutBounds (image, new Rectangle (0, 0, 1, 1));
			AbsoluteLayout.SetLayoutFlags (description, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (description, new Rectangle (0, 0.5, 0.25, 0.4));

			layout.Children.Add (image);
			layout.Children.Add (description);

			View = layout;
		}

		public void InterestedInActivity()
		{
			ActivitiesManager.InterestedInActivity((Recipe) BindingContext);
		}

		public void NotInterestedInActivity()
		{
			ActivitiesManager.NotInterestedInActivity((Recipe) BindingContext);
		}
	}
}

