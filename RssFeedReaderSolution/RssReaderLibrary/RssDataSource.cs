using System;
using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.Util;

namespace RssReaderLibrary
{
	public class RssDataSource
	{
		RssSqlLiteHelperClass rssSqlHelper;
		private SQLiteDatabase database;

		public static String[] allColumns = {
			RssSqlLiteHelperClass.RSS_FEED_NAME,
			RssSqlLiteHelperClass.RSS_FEED_URL
		};

		public RssDataSource (Context context)
		{
			rssSqlHelper = new RssSqlLiteHelperClass (context);
			open ();
		}

		public void open()
		{
			database = rssSqlHelper.WritableDatabase;
		}

		public void close() 
		{
			rssSqlHelper.Close();
		}

		public ICursor getAllRssFeeds()
		{
			return executeQuery ("", "-1");
		}

		public ICursor getRssFeeds(String rssName)
		{
			return executeQuery (RssSqlLiteHelperClass.RSS_FEED_NAME + "='" + rssName + "'", "-1");
		}

		public int addRssFeed(RssFeed rssFeed)
		{
			String query = RssSqlLiteHelperClass.RSS_FEED_NAME + "='" + rssFeed.Name + "'";
			if (executeQuery (query, "1").Count == 0) 
			{
				ContentValues values = new ContentValues ();
				values.Put(RssSqlLiteHelperClass.RSS_FEED_NAME ,rssFeed.Name );
				values.Put(RssSqlLiteHelperClass.RSS_FEED_URL , rssFeed.Url );
				return (int)database.Insert (RssSqlLiteHelperClass.TABLE_NAME, null, values);
			}
			return -2;
		}

		private ICursor executeQuery(String query, String limit)
		{
			ICursor cursor;
			if (limit == "-1")
				cursor = database.Query (RssSqlLiteHelperClass.TABLE_NAME, allColumns, query, null, null, null, null);
			else
				cursor = database.Query (RssSqlLiteHelperClass.TABLE_NAME, allColumns, query, null, null, null, null, limit);
			Log.WriteLine (LogPriority.Info, "Feed Counnnnnnt", " Feed count : " + cursor.Count + " :: query = " + query + " : limit = " + limit);
			return cursor;
		}
	}
}

