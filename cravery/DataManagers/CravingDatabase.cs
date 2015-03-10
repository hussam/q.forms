using SQLite;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace cravery {
	/// <summary>
	/// CacheDB builds on SQLite.Net and represents the app's core db.
	/// It contains methods for retrieval and persistance as well as db creation, all based on the 
	/// underlying ORM.
	/// </summary>
	public class CravingDatabase : SQLiteAsyncConnection  {
		object locker = new object ();

		public static ObservableCollection<Craving> Cravings { get; set; }

		/// <summary>
		/// Initializes a new instance of the Craving Database. 
		/// if the database doesn't exist, it will create the database and all the tables.
		/// </summary>
		/// <param name='path'>
		/// Path.
		/// </param>
		public CravingDatabase(string path) : base(path)
		{
		}

		public Task<CreateTablesResult> CreateTable()
		{
			lock (locker) {
				return CreateTableAsync<Craving> ();
			}
		}

		public Task<ObservableCollection<Craving>> GetCravings ()
		{
			AsyncTableQuery<Craving> table;
			lock (locker) {
				table = Table<Craving> ().OrderByDescending<int> (c => c.LocalId);
			}
			return table.ToListAsync ().ContinueWith (t => {
				Cravings = new ObservableCollection<Craving> (t.Result);
				return Cravings;
			});
		}

		public void SaveCraving(Craving craving)
		{
			Task<int> task;
			lock (locker) {
				task = InsertAsync (craving);
			}
			task.ContinueWith (t => Cravings.Insert (0, craving));
		}
	}
}