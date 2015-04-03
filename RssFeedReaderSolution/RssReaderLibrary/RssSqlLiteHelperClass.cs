using System;
using Android.Database.Sqlite;
using Android.Content;

namespace RssReaderLibrary
{
	public class RssSqlLiteHelperClass : SQLiteOpenHelper
	{
		public static readonly String TABLE_NAME = "RssFeeds";
		public static readonly String RSS_FEED_NAME = "Name";
		public static readonly String RSS_FEED_URL = "Url";

		public static readonly String Database_Name = "RssFeed.db";
		public static readonly int DatabaseVersion = 1;

		public static readonly String DatabaseCreateSql = "create table IF NOT EXISTS "
			+ TABLE_NAME + "(" 
			+ RSS_FEED_NAME	+ " text not null collate nocase, " 
			+ RSS_FEED_URL	+ " text" 
			+ ");";

		public RssSqlLiteHelperClass (Context context) : base (context, Database_Name, null, DatabaseVersion)
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

