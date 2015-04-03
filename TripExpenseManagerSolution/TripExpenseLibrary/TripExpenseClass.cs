using System;
using System.Collections.Generic;

namespace TripExpenseLibrary
{
	public class TripDetailsClass
	{
		public int TripId;
		public String Tripname;
		public String TripDesc;
		public String From;
		public String To;
		public String FromDate;
		public String ToDate;
		public double Allocated;
		public double Spent;
		public List<ExpenseDetailsClass> ExpenseDetails = new List<ExpenseDetailsClass>();
	}

	public class ExpenseDetailsClass
	{
		public int TripId;
		public int ExpenseId;
		public String ExpenseName;
		public double ExpenseCost;
	}
}

