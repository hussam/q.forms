using SQLite;

namespace cravery
{
	public class DBItem
	{
		[PrimaryKey, AutoIncrement]
		public int LocalId { get; set; }

		public DBItem() {}
	}
}