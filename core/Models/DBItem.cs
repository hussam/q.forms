﻿using SQLite;

namespace Q
{
	public class DBItem
	{
		[PrimaryKey, AutoIncrement]
		public int LocalId { get; set; }

		public DBItem() {}
	}
}