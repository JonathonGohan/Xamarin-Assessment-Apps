using System;
using Android.Database.Sqlite;
using Android.Database;
using Android.Content;

namespace EventLibrary
{
	public class LoginDataSource
	{
		private SQLiteDatabase database;
		public LoginSqlLiteHelper dbHelper;
		private String[] allColumns = { LoginSqlLiteHelper.LOGIN_NAME, LoginSqlLiteHelper.PASSWORD};

		public LoginDataSource(Context context) 
		{
			dbHelper = new LoginSqlLiteHelper(context);
		}

		public void open()
		{
			database = dbHelper.WritableDatabase;
		}

		public void close() 
		{
			dbHelper.Close();
		}

		public bool checkLoginDetails(String Username, String Password)
		{
			ICursor cursor = database.Query (LoginSqlLiteHelper.TableName, allColumns, LoginSqlLiteHelper.LOGIN_NAME + " = '" + Username + "' and " + LoginSqlLiteHelper.PASSWORD + "='" + Password + "'", null, null, null, null);
			if (cursor != null && cursor.Count > 0)
				return true;
			return false;
		}

		public bool checkUserNameExists(String Username)
		{
			ICursor cursor = database.Query (LoginSqlLiteHelper.TableName, allColumns, LoginSqlLiteHelper.LOGIN_NAME +  "='" +  Username + "'", null, null, null, null);
			if (cursor != null && cursor.Count > 0)
				return true;
			return false;
		}

		public bool insertLoginDetails(String Username, String Password)
		{
			ContentValues values = new ContentValues ();
			values.Put (LoginSqlLiteHelper.LOGIN_NAME, Username);
			values.Put (LoginSqlLiteHelper.PASSWORD, Password);

			long insertId = database.Insert (LoginSqlLiteHelper.TableName, null, values);

			return insertId > 0 ? true : false;
		}
	}
}

