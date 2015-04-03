using System;
using System.Collections.Generic;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;

namespace TripExpenseLibrary
{
	public sealed class TripExpenseDataSource
	{
		static TripExpenseDataSource _instance = new TripExpenseDataSource();
		List<TripDetailsClass> _tripDetails;
		TripSqlLiteHelper _tripSqlHelper;
		ExpenseSqlLiteHelper _expenseSqlHelper;
		Context _context;
		SQLiteDatabase _database;
		SQLiteDatabase _expensedatabase;
		String[] tripcolumns = {
			TripSqlLiteHelper.COLUMN_TRIP_ID, TripSqlLiteHelper.COLUMN_TRIP_NAME, TripSqlLiteHelper.COLUMN_TRIP_DESC,
			TripSqlLiteHelper.COLUMN_FROM, TripSqlLiteHelper.COLUMN_TO, TripSqlLiteHelper.COLUMN_FROM_DATE, 
			TripSqlLiteHelper.COLUMN_TO_DATE, TripSqlLiteHelper.COLUMN_ALLOCATED, TripSqlLiteHelper.COLUMN_SPENT
		};

		String[] expenseColumns = {
			ExpenseSqlLiteHelper.COLUMN_TRIP_ID, ExpenseSqlLiteHelper.COLUMN_EXPENSE_ID, 
			ExpenseSqlLiteHelper.COLUMN_EXPENSE_NAME, ExpenseSqlLiteHelper.COLUMN_EXPENSE_COST
		};
		public static TripExpenseDataSource Instance
		{
			get
			{
				return _instance;
			}
		}

		public int TripCount 
		{
			get{ return _tripDetails.Count; }
		}

		public int ExpenseCount
		{
			get{ return CurrentTrip.ExpenseDetails.Count; }
		}

		private int _CurrentTripIndex;
		private int _CurrentExpenseIndex;

		public TripDetailsClass CurrentTrip
		{
			get 
			{
				if (_CurrentTripIndex < 0 || _CurrentTripIndex >=  TripCount)
					throw new IndexOutOfRangeException ("The position : " + _CurrentTripIndex + " is invalid");
				return _tripDetails [_CurrentTripIndex]; 
			}
			private set 
			{
				_tripDetails[_CurrentTripIndex] = value;
			}
		}

		public ExpenseDetailsClass CurrentExpense
		{
			get 
			{
				if (_CurrentExpenseIndex < 0 || _CurrentExpenseIndex >= ExpenseCount)
					throw new IndexOutOfRangeException ("The position : " + _CurrentExpenseIndex + " is invalid");
				return CurrentTrip.ExpenseDetails [_CurrentExpenseIndex];
			}
			private set 
			{
				CurrentExpense = value;
			}
		}

		private TripExpenseDataSource ()
		{
			_tripDetails = new List<TripDetailsClass> ();
		}

		public void openTripDatabase(Context ctx)
		{
			_context = ctx;
			if ( _tripSqlHelper == null )
				_tripSqlHelper = new TripSqlLiteHelper (_context);
			_database = _tripSqlHelper.WritableDatabase;
			openExpenseDatabase (ctx);
		}

		public void closeTripDatabase() 
		{
			_tripSqlHelper.Close();
		}


		public void openExpenseDatabase(Context ctx)
		{
			_context = ctx;
			if ( _expenseSqlHelper == null )
				_expenseSqlHelper = new ExpenseSqlLiteHelper (_context);
			_expensedatabase = _expenseSqlHelper.WritableDatabase;
			_expenseSqlHelper.OnCreate (_expensedatabase);
		}

		public void closeExpenseDatabase()
		{
			_expenseSqlHelper.Close();
		}

		public void MoveTripTo (int position)
		{
			_CurrentTripIndex = position;
		}

		public void MoveExpenseTo (int position)
		{
			_CurrentExpenseIndex = position;
		}

		public int getAllTrips()
		{
			_tripDetails = new List<TripDetailsClass> ();
			ICursor result =  _database.Query(TripSqlLiteHelper.TABLE_NAME, tripcolumns , null,null,null,null,null);
			if (result.Count > 0) 
			{
				result.MoveToFirst ();
				while (!result.IsAfterLast) 
				{
					TripDetailsClass trip = new TripDetailsClass ();
					trip.Tripname = result.GetString ( result.GetColumnIndex(TripSqlLiteHelper.COLUMN_TRIP_NAME));
					trip.TripId = result.GetInt (result.GetColumnIndex(TripSqlLiteHelper.COLUMN_TRIP_ID));
					trip.From = result.GetString (result.GetColumnIndex(TripSqlLiteHelper.COLUMN_FROM));
					trip.To = result.GetString (result.GetColumnIndex(TripSqlLiteHelper.COLUMN_TO));
					trip.FromDate = result.GetString (result.GetColumnIndex(TripSqlLiteHelper.COLUMN_FROM_DATE));
					trip.ToDate = result.GetString (result.GetColumnIndex(TripSqlLiteHelper.COLUMN_TO_DATE));
					trip.Allocated = result.GetDouble (result.GetColumnIndex(TripSqlLiteHelper.COLUMN_ALLOCATED));
					trip.Spent = result.GetDouble (result.GetColumnIndex(TripSqlLiteHelper.COLUMN_SPENT));
					_tripDetails.Add (trip);
					result.MoveToNext ();
				}
			}
			return _tripDetails.Count;
		}

		public int getAllExpense(int tripid)
		{
			CurrentTrip.ExpenseDetails = new List<ExpenseDetailsClass> ();
			ICursor expenses = _expensedatabase.Query (ExpenseSqlLiteHelper.TABLE_NAME, expenseColumns, ExpenseSqlLiteHelper.COLUMN_TRIP_ID + " = " + _tripDetails[0].TripId , null, null, null, null);
			if (expenses.Count > 0) 
			{
				expenses.MoveToFirst ();
				while (!expenses.IsAfterLast) 
				{
					ExpenseDetailsClass expense = new ExpenseDetailsClass ();
					expense.TripId = expenses.GetInt (expenses.GetColumnIndex (ExpenseSqlLiteHelper.COLUMN_TRIP_ID));
					expense.ExpenseId = expenses.GetInt (expenses.GetColumnIndex (ExpenseSqlLiteHelper.COLUMN_EXPENSE_ID));
					expense.ExpenseName = expenses.GetString (expenses.GetColumnIndex (ExpenseSqlLiteHelper.COLUMN_EXPENSE_NAME));
					expense.ExpenseCost = expenses.GetDouble (expenses.GetColumnIndex (ExpenseSqlLiteHelper.COLUMN_EXPENSE_COST));
					CurrentTrip.ExpenseDetails.Add (expense);
					expenses.MoveToNext ();
				}
			}
			return CurrentTrip.ExpenseDetails.Count;
		}

		public int addTripToDataSource(TripDetailsClass trip)
		{
			ContentValues values = new ContentValues ();
			values.Put ( TripSqlLiteHelper.COLUMN_TRIP_NAME , trip.Tripname);
			values.Put (TripSqlLiteHelper.COLUMN_TRIP_DESC, trip.TripDesc);
			values.Put (TripSqlLiteHelper.COLUMN_FROM, trip.From);
			values.Put (TripSqlLiteHelper.COLUMN_TO, trip.To);
			values.Put (TripSqlLiteHelper.COLUMN_FROM_DATE, trip.FromDate);
			values.Put (TripSqlLiteHelper.COLUMN_TO_DATE, trip.ToDate);
			values.Put (TripSqlLiteHelper.COLUMN_ALLOCATED, trip.Allocated);
			if (trip.TripId == -1)
			{
				long newId = _database.Insert (TripSqlLiteHelper.TABLE_NAME, null, values);
				if (newId > 0) 
				{
					trip.TripId = (int)newId;
					_tripDetails.Add (trip);
				}
				return trip.TripId;
			}
			else 
			{
				int result = _database.Update(TripSqlLiteHelper.TABLE_NAME, values, TripSqlLiteHelper.COLUMN_TRIP_ID + " = " + trip.TripId , null);
				CurrentTrip = trip;
				return result;
			}
		}

		public int addExpenseToTripDataSource( int selectedIndex, int selectedexpenseId, ExpenseDetailsClass expense)
		{
			ContentValues values = new ContentValues ();
			values.Put ( ExpenseSqlLiteHelper.COLUMN_TRIP_ID , CurrentTrip.TripId);
			values.Put ( ExpenseSqlLiteHelper.COLUMN_EXPENSE_NAME, expense.ExpenseName);
			values.Put (ExpenseSqlLiteHelper.COLUMN_EXPENSE_COST, expense.ExpenseCost);
			if (selectedexpenseId == -1) 
			{
				int result = (int)_expensedatabase.Insert(ExpenseSqlLiteHelper.TABLE_NAME, null, values);
				if (result > 0) 
				{
					expense.ExpenseId = result;
					expense.TripId = CurrentTrip.TripId;
					CurrentTrip.ExpenseDetails.Add (expense);
					updateTripExpense (1, CurrentTrip.Spent + expense.ExpenseCost);
				}
				return result;
			}
			else 
			{
				int result = _expensedatabase.Update (ExpenseSqlLiteHelper.TABLE_NAME, values, ExpenseSqlLiteHelper.COLUMN_TRIP_ID + " = " + CurrentTrip.TripId + " and " + ExpenseSqlLiteHelper.COLUMN_EXPENSE_ID + " = " + CurrentTrip.ExpenseDetails [selectedexpenseId].ExpenseId, null);
				if (result > 0) 
				{
					double oldvalue = CurrentTrip.Spent;
					double newvalue = expense.ExpenseCost;
					double totalSpent = CurrentTrip.Spent;
					totalSpent += oldvalue > newvalue ? oldvalue - newvalue : newvalue - oldvalue;
					updateTripExpense (1, totalSpent);
					CurrentTrip.ExpenseDetails [selectedexpenseId] = expense;
				}
				return result;
			}
		}

		public int deleteExpense(int expenseId)
		{
			int result = _expensedatabase.Delete (ExpenseSqlLiteHelper.TABLE_NAME, ExpenseSqlLiteHelper.COLUMN_TRIP_ID + " = " + CurrentTrip.TripId + " and " + ExpenseSqlLiteHelper.COLUMN_EXPENSE_ID + " = " + CurrentTrip.ExpenseDetails [expenseId].ExpenseId, null);
			if (result > 0) 
			{
				updateTripExpense (-1, CurrentTrip.Spent - CurrentTrip.ExpenseDetails [expenseId].ExpenseCost);
				CurrentTrip.ExpenseDetails.RemoveAt (expenseId);

			}
			return result;
		}

		public void updateTripExpense(int action, double newValue)
		{
			CurrentTrip.Spent = newValue;
			ContentValues value = new ContentValues ();
			value.Put (TripSqlLiteHelper.COLUMN_SPENT, newValue);
			_database.Update (TripSqlLiteHelper.TABLE_NAME, value, TripSqlLiteHelper.COLUMN_TRIP_ID + " = " + CurrentTrip.TripId, null);
		}
	}
}

