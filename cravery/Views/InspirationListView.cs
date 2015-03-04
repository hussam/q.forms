using Xamarin.Forms;

namespace cravery
{
	public class InspirationListView : ListView
	{
		public InspirationListView()
		{
			ItemTemplate = ItemTemplate = new DataTemplate (typeof(InspirationCell));
			RowHeight = 160;
		}
	}
}

