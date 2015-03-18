using Xamarin.Forms;

namespace Q
{
	public delegate void SuggestionClickedHandler(object suggestion);

	public class AutoSuggestionsView<T> : StackLayout
	{
		SuggestionCell<T>[] cells;

		public event SuggestionClickedHandler SuggestionClicked;
		
		public AutoSuggestionsView (int numSuggestions)
		{
			cells = new SuggestionCell<T>[numSuggestions];

			Spacing = 2;
			for (int i = 0; i < numSuggestions; i++) {
				var cell = new SuggestionCell<T> ();
				cell.GestureRecognizers.Add (new TapGestureRecognizer {
					Command = new Command ((obj) => {
						SuggestionClicked(cell.Object);
					})
				});
				cells [i] = cell;
				Children.Add (cell);
			}
		}

		public void SetAtIndex(int i, string masterText, string detailText, T obj)
		{
			cells [i].Master.Text = masterText;
			cells [i].Detail.Text = detailText;
			cells [i].Object = obj;
		}

		public T getObjAtIndex(int i)
		{
			return cells [i].Object;
		}

		public void Clear()
		{
			for (int i = 0; i < cells.Length; i++) {
				cells [i].Master.Text = "";
				cells [i].Detail.Text = "";
			}
		}

		private class SuggestionCell<W> : StackLayout
		{
			public Label Master;
			public Label Detail;
			public W Object;

			public SuggestionCell()
			{
				Spacing = 0;

				Master = new Label {
					FontSize = 14,
					HeightRequest = 16
				};
				Children.Add(Master);

				Detail = new Label {
					FontSize = 12,
					HeightRequest = 14,
					TextColor = Color.Gray.WithLuminosity(0.75)
				};
				Children.Add(Detail);
			}
		}
	}
}

