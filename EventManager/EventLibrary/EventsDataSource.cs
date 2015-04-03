using System;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;

namespace EventLibrary
{
	public class EventsDataSource
	{
		private SQLiteDatabase database;
		public EventSqlLiteHelper dbHelper;
		private String[] allColumns = { EventSqlLiteHelper.EventIdColumnName, EventSqlLiteHelper.UserName, EventSqlLiteHelper.EventColumnName, EventSqlLiteHelper.LocationColumnName , 
			EventSqlLiteHelper.DateColumnName, EventSqlLiteHelper.TimeColumnName, EventSqlLiteHelper.HallColumnName};

		public EventsDataSource(Context context) 
		{
			dbHelper = new EventSqlLiteHelper(context);
		}

		public void open()
		{
			database = dbHelper.WritableDatabase;
			//database.ExecSQL("DROP TABLE IF EXISTS " + EventSqlLiteHelper.TableName);
			database.ExecSQL (EventSqlLiteHelper.DatabaseCreateSql);
		}

		public void close() 
		{
			dbHelper.Close();
		}

		public ICursor getAllEvents(String Username)
		{
			return database.Query (EventSqlLiteHelper.TableName, allColumns, EventSqlLiteHelper.UserName + " = '" + Username + "'", null, null, null, null);
		}

		public int insertEvent(EventClass eventClass)
		{
			ContentValues values = new ContentValues ();
			values.Put (EventSqlLiteHelper.UserName, eventClass.Username);
			values.Put (EventSqlLiteHelper.EventColumnName, eventClass.EventName);
			values.Put (EventSqlLiteHelper.LocationColumnName, eventClass.Location);
			values.Put (EventSqlLiteHelper.DateColumnName, eventClass.Date);
			values.Put (EventSqlLiteHelper.TimeColumnName, eventClass.Time);
			values.Put (EventSqlLiteHelper.HallColumnName, eventClass.hall);

			int insertId = (int)database.Insert (EventSqlLiteHelper.TableName, null, values);

			return insertId;
		}

		public bool updateEvent(EventClass eventClass)
		{
			int eventid = eventClass.EventId;
			String username = eventClass.Username;

			ContentValues values = new ContentValues ();
			values.Put (EventSqlLiteHelper.EventColumnName, eventClass.EventName);
			values.Put (EventSqlLiteHelper.LocationColumnName, eventClass.Location);
			values.Put (EventSqlLiteHelper.DateColumnName, eventClass.Date);
			values.Put (EventSqlLiteHelper.TimeColumnName, eventClass.Time);
			values.Put (EventSqlLiteHelper.HallColumnName, eventClass.hall);

			int rowsUpdated = database.Update(EventSqlLiteHelper.TableName, values, EventSqlLiteHelper.EventIdColumnName + "=" + eventid + " and " + EventSqlLiteHelper.UserName + "='" + eventClass.Username  + "'", null);

			return rowsUpdated > 0 ? true : false;
		}

		public bool deleteEvent(EventClass eventClass)
		{
			int deletedRows = database.Delete (EventSqlLiteHelper.TableName, EventSqlLiteHelper.EventIdColumnName + "=" + eventClass.EventId + " and " + EventSqlLiteHelper.UserName + "='" + eventClass.Username + "'",null);
			return deletedRows > 0 ? true : false;
		}
	}
}

