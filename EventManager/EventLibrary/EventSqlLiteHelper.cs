using System;
using Android.Database;
using Android.Database.Sqlite;
using Android.Content;

namespace EventLibrary
{
	public class EventSqlLiteHelper : SQLiteOpenHelper
	{
		public static readonly String TableName = "Events";
		public static readonly String UserName = "Username";
		public static readonly String EventIdColumnName = "EventId";
		public static readonly String EventColumnName = "EventName";
		public static readonly String LocationColumnName = "LocationName";
		public static readonly String DateColumnName = "Date";
		public static readonly String TimeColumnName = "Time";
		public static readonly String HallColumnName = "Hall";

		public static readonly String Database_Name = "EventManagerAssessment.db";
		public static readonly int DatabaseVersion = 1;

		public static readonly String DatabaseCreateSql = "create table IF NOT EXISTS "
			+ TableName + "(" 
			+ EventIdColumnName	+ " integer primary key, " 
			+ UserName	+ " text not null collate nocase, " 
			+ EventColumnName	+ " text not null," 
			+ LocationColumnName	+ " text not null," 
			+ DateColumnName	+ " text not null," 
			+ TimeColumnName	+ " text not null," 
			+ HallColumnName	+ " text not null, " 
			+ "FOREIGN KEY (Username) REFERENCES LoginDetails(Loginname)"
			+ ");";

		public EventSqlLiteHelper (Context context) : base (context, Database_Name, null, DatabaseVersion)
		{
		}

		public override void OnCreate (SQLiteDatabase db)
		{
			//db.ExecSQL("DROP TABLE IF EXISTS " + TableName);
			db.ExecSQL (DatabaseCreateSql);
		}

		public override void OnUpgrade (SQLiteDatabase db, int oldVersion, int newVersion)
		{
			db.ExecSQL("DROP TABLE IF EXISTS " + TableName);
			OnCreate(db);
		}
	}
}

