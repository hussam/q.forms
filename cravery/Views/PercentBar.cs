using Xamarin.Forms;

namespace cravery
{
	public class PercentBar : View
	{
		Label label, bar, percentage;

		public PercentBar ()
		{
			ShowText = true;
			Text = "PercentBar";
			BarColor = Color.Black;
			Percentage = 0.5;

			label = new Label ();
			label.Text = Text;
			label.TextColor = TextColor;
			label.LineBreakMode = LineBreakMode.TailTruncation;

			bar = new Label ();
			bar.BackgroundColor = BarColor;

			var layout = new AbsoluteLayout ();
			AbsoluteLayout.SetLayoutFlags (label, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutFlags (bar, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutFlags (percentage, AbsoluteLayoutFlags.All);

			AbsoluteLayout.SetLayoutBounds (label, new Rectangle (0, 0, 0.25, 1));
			AbsoluteLayout.SetLayoutBounds (bar, new Rectangle (0, 0, Percentage, 1));
		}

		public bool ShowText {
			get;
			set;
		}

		public string Text {
			get;
			set;
		}

		public double Percentage {
			get;
			set;
		}

		public Color BarColor {
			get;
			set;
		}

		public Color TextColor {
			get;
			set;
		}
	 }
}

