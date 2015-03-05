using Xamarin.Forms;
using System;

namespace cravery
{
	public class CravingCell : ViewCell
	{
		readonly Label elapsedCover;

		public CravingCell ()
		{
			var image = new Image {
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};
			image.SetBinding (Image.SourceProperty, "Recipe.Picture");

			elapsedCover = new Label {
				BackgroundColor = Color.White,
				Opacity = 0.9
			};

			var layout = new AbsoluteLayout ();
			AbsoluteLayout.SetLayoutFlags (image, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (image, new Rectangle (0, 0, 1, 1));
			AbsoluteLayout.SetLayoutFlags (elapsedCover, AbsoluteLayoutFlags.All);

			layout.Children.Add (image);
			layout.Children.Add (elapsedCover);

			View = layout;
		}

		protected override void OnBindingContextChanged ()
		{
			base.OnBindingContextChanged ();
			var carving = (ActivityInterest)BindingContext;
			double elapsedPercent = (DateTime.UtcNow - carving.CreatedAt).Value.TotalHours / 1.0F;
			AbsoluteLayout.SetLayoutBounds (elapsedCover, new Rectangle (1, 0, elapsedPercent, 1));
		}
	}
}

