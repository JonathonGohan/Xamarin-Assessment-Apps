using System;
using Android.Content;
using Android.Database.Sqlite;

namespace TripExpenseLibrary
{
	public class ExpenseSqlLiteHelper : SQLiteOpenHelper
	{
		public static readonly String TABLE_NAME = "Expense";
		public static readonly String COLUMN_TRIP_ID = "TripId";
		public static readonly String COLUMN_EXPENSE_ID = "ExpenseId";
		public static readonly String COLUMN_EXPENSE_NAME = "ExpenseName";
		public static readonly String COLUMN_EXPENSE_COST = "ExpenseCost";

		public static readonly String Database_Name = "TripExpensemanager.db";
		public static readonly int DatabaseVersion = 1;

		public static readonly String DatabaseCreateSql = "Create table IF NOT EXISTS "
			+ TABLE_NAME
			+ "("
			+ COLUMN_TRIP_ID + " integer,"
			+ COLUMN_EXPENSE_ID + " integer primary key autoincrement,"
			+ COLUMN_EXPENSE_NAME + " text,"
			+ COLUMN_EXPENSE_COST + " double,"
			+ "FOREIGN KEY (" +  COLUMN_TRIP_ID + ") REFERENCES " + TripSqlLiteHelper.TABLE_NAME + "(" + TripSqlLiteHelper.COLUMN_TRIP_ID + ")"
			+ ");";

		public ExpenseSqlLiteHelper (Context context) : base (context, Database_Name, null, DatabaseVersion)
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

