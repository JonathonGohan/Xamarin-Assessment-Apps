
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TripExpenseManager
{
	[Activity (Label = "TripDetails")]			
	public class TripDetails : Activity
	{
		ExpandableListView _tripexpenseListView;
		TripDetailsExpandandableListAdapter _adapter;
		String selectedIndex;
		Button _addExpense;
		TextView _tripExpenseSpentTextView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.TripExpenseDetails);
			selectedIndex = Intent.GetStringExtra ("SelectedIndex");

			_tripExpenseSpentTextView = FindViewById<TextView>(Resource.Id.TripExpenseBottomSpentTextView);

			_adapter = new TripDetailsExpandandableListAdapter (this, selectedIndex);

			_tripexpenseListView = FindViewById<ExpandableListView> (Resource.Id.TripDetailsExpanListView);
			_tripexpenseListView.SetAdapter (_adapter);

			_tripexpenseListView.ChildClick += (object sender, ExpandableListView.ChildClickEventArgs e) => 
			{
				if( e.GroupPosition == 1)
				{
					_adapter.showExpenseDialog("" + e.ChildPosition);
				}
			};

			_addExpense = FindViewById<Button> (Resource.Id.AddExpenseButton);
			_addExpense.Click += (object sender, EventArgs e) => 
			{
				_adapter.showExpenseDialog("-1");
			};
		}

		public void myShowDialog()
		{
			ShowDialog (1111);
		}

		public void UpdateSpentData(double value)
		{
			_tripExpenseSpentTextView.Text = "" + value;
		}

		protected override Dialog OnCreateDialog (int id)
		{
			switch (id) 
			{
			case 1111:
				DatePickerDialog datePicker = new DatePickerDialog (this, HandleDateSet, 2015, 1, 2);
				datePicker.DatePicker.MinDate = new Java.Util.Date ().Time - 1000;
				return datePicker;
			}
			return null;
		}

		void HandleDateSet (object sender, DatePickerDialog.DateSetEventArgs e)
		{
			_adapter.setTripDate (e.Date.ToString ("dd-MMM-yyyy"));
		}
	}
}

