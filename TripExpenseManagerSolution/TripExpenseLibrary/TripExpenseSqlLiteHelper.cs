using System;
using Android.Content;
using Android.Database.Sqlite;

namespace TripExpenseLibrary
{
	public class TripSqlLiteHelper : SQLiteOpenHelper
	{
		public static readonly String TABLE_NAME = "Trip";
		public static readonly String COLUMN_TRIP_ID = "TripId";
		public static readonly String COLUMN_TRIP_DESC = "TripDesc";
		public static readonly String COLUMN_TRIP_NAME = "TripName";
		public static readonly String COLUMN_FROM = "FromPlace";
		public static readonly String COLUMN_TO = "ToPlace";
		public static readonly String COLUMN_FROM_DATE = "FromDate";
		public static readonly String COLUMN_TO_DATE = "ToDate";
		public static readonly String COLUMN_ALLOCATED = "Allocated";
		public static readonly String COLUMN_SPENT = "Spent";

		public static readonly String Database_Name = "TripExpensemanager.db";
		public static readonly int DatabaseVersion = 1;

		public static readonly String DatabaseCreateSql = "Create table IF NOT EXISTS "
		                                                  + TABLE_NAME
		                                                  + "("
														  + COLUMN_TRIP_ID + " integer primary key autoincrement,"
		                                                  + COLUMN_TRIP_NAME + " text not null,"
														  + COLUMN_TRIP_DESC + " text,"
		                                                  + COLUMN_FROM + " text,"
		                                                  + COLUMN_TO + " text,"
		                                                  + COLUMN_FROM_DATE + " text,"
		                                                  + COLUMN_TO_DATE + " text,"
		                                                  + COLUMN_ALLOCATED + " double,"
		                                                  + COLUMN_SPENT + " double"
		                                                  + ");";

		public TripSqlLiteHelper (Context context) : base (context, Database_Name, null, DatabaseVersion)
		{
		}

		public override void OnCreate (SQLiteDatabase db)
		{
			db.ExecSQL (DatabaseCreateSql);
		}

		public override void OnUpgrade (SQLiteDatabase db, int oldVersion, int newVersion)
		{
			db.ExecSQL("DROP TABLE IF EXISTS " + TABLE_NAME);
			OnCreate(db);
		}
	}
}

