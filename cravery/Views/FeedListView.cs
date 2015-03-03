using Xamarin.Forms;

namespace cravery
{
	public class FeedListView : ListView
	{
		public FeedListView()
		{
			ItemTemplate = ItemTemplate = new DataTemplate (typeof(FeedCell));
			RowHeight = 120;
		}
	}
}

