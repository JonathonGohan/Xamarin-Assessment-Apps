using System;
using Android.Database.Sqlite;
using Android.Content;

namespace EventLibrary
{
	public class LoginSqlLiteHelper: SQLiteOpenHelper
	{
		public static readonly String TableName = "LoginDetails";
		public static readonly String LOGIN_NAME = "Loginname";
		public static readonly String PASSWORD = "Password";

		public static readonly String Database_Name = "EventManagerAssessment.db";
		public static readonly int DatabaseVersion = 1;

		public static readonly String DatabaseCreateSql = "create table IF NOT EXISTS "
			+ TableName 
			+ "(" 
			+ LOGIN_NAME + " text primary key collate nocase," 
			+ PASSWORD + " text not null" 
			+ ");";

		public LoginSqlLiteHelper (Context context) : base (context, Database_Name, null, DatabaseVersion)
		{
		}

		#region commented
		/*private void CreateLocationHallTables(SQLiteDatabase db)
		{
			db.ExecSQL("DROP TABLE IF EXISTS Location");
			String locationCreateSql = "create table IF NOT EXISTS Location"
			                           + "( LocationId integer primary key , LocationName text not null);";
			db.ExecSQL (locationCreateSql);

			db.ExecSQL("DROP TABLE IF EXISTS Hall");
			String hallCreateSql = "create table IF NOT EXISTS Hall"
			                       + "( HallId integer primary key, HallName text not null , LocationId integer not null,"
			                       + "FOREIGN KEY (LocationId) REFERENCES Location(LocationId));";
			db.ExecSQL (hallCreateSql);

			insertDataintoLocationHall (db);
		}

		private void insertDataintoLocationHall(SQLiteDatabase db)
		{
			ContentValues values = new ContentValues ();
			values.Put ("LocationName", "Chennai");
			db.Insert ("Location", null, values);
			values = new ContentValues ();
			values.Put ("LocationName", "Bangalore");
			db.Insert ("Location", null, values);
			values = new ContentValues ();
			values.Put ("LocationName", "Chandigarh");
			db.Insert ("Location", null, values);


			values = new ContentValues ();
			values.Put ("HallName", "Chennai 1");
			values.Put ("LocationId", "1");
			db.Insert ("Hall", null, values);
			values = new ContentValues ();
			values.Put ("HallName", "Chennai 2");
			values.Put ("LocationId", "1");
			db.Insert ("Hall", null, values);
			values = new ContentValues ();
			values.Put ("HallName", "Chennai 3");
			values.Put ("LocationId", "1");
			db.Insert ("Hall", null, values);
			values = new ContentValues ();
			values.Put ("HallName", "Bangalore 1");
			values.Put ("LocationId", "2");
			db.Insert ("Hall", null, values);
			values = new ContentValues ();
			values.Put ("HallName", "Bangalore 2");
			values.Put ("LocationId", "2");
			db.Insert ("Hall", null, values);
			values = new ContentValues ();
			values.Put ("HallName", "Chandigarh 1");
			values.Put ("LocationId", "3");
			db.Insert ("Hall", null, values);
			values = new ContentValues ();
			values.Put ("HallName", "Chandigarh 2");
			values.Put ("LocationId", "3");
			db.Insert ("Hall", null, values);
		}*/

		#endregion

		public override void OnCreate (SQLiteDatabase db)
		{
			//db.ExecSQL("DROP TABLE IF EXISTS " + TableName);
			db.ExecSQL (DatabaseCreateSql);
			//CreateLocationHallTables (db);
		}

		public override void OnUpgrade (SQLiteDatabase db, int oldVersion, int newVersion)
		{
			db.ExecSQL("DROP TABLE IF EXISTS " + TableName);
			OnCreate(db);
		}

	}
}

