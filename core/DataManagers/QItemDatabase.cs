using SQLite;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Q {
	/// <summary>
	/// CacheDB builds on SQLite.Net and represents the app's core db.
	/// It contains methods for retrieval and persistance as well as db creation, all based on the 
	/// underlying ORM.
	/// </summary>
	public class QItemDatabase : SQLiteAsyncConnection  {
		object locker = new object ();

		public static ObservableCollection<QItem> QItems { get; set; }

		/// <summary>
		/// Initializes a new instance of the Q Item Database. 
		/// if the database doesn't exist, it will create the database and all the tables.
		/// </summary>
		/// <param name='path'>
		/// Path.
		/// </param>
		public QItemDatabase(string path) : base(path)
		{
		}

		public Task<CreateTablesResult> CreateTable()
		{
			lock (locker) {
				return CreateTableAsync<QItem> ();
			}
		}

		public Task<ObservableCollection<QItem>> GetItems ()
		{
			AsyncTableQuery<QItem> table;
			lock (locker) {
				table = Table<QItem> ().OrderByDescending<int> (c => c.LocalId);
			}
			return table.ToListAsync ().ContinueWith (t => {
				QItems = new ObservableCollection<QItem> (t.Result);
				return QItems;
			});
		}

		public void SaveItem(QItem item)
		{
			Task<int> task;
			lock (locker) {
				task = InsertAsync (item);
			}
			task.ContinueWith (t => QItems.Insert (0, item));
		}
	}
}