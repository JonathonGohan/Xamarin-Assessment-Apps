using System;
using Android.Widget;
using Android.App;
using Android.Content;
using Android.Views;
using TripExpenseLibrary;

namespace TripExpenseManager
{
	public class TripDetailsExpandandableListAdapter : BaseExpandableListAdapter
	{

		ExpandableListView _expanListView;
		Context _context;
		Activity _activity;
		int myExpandedGroup=-1;
		View _tripdetailsView;
		bool isTripViewNew = true;
		int DateControlClicked = -1;
		TextView TripNameTextView;
		TextView TripDescTextView;
		TextView TripFromTextView;
		TextView TripToTextView;
		TextView TripFromDateTextView;
		TextView TripToDateTextView;
		TextView TripAllocatedTextView;
		Button SaveButton;
		TripExpenseDataSource _dataSource;
		String _selectedIndex;
		TripDetails _tripActivity;
		String _selectedExpenseIndex ="-1";
		AlertDialog.Builder _builder;
		AlertDialog _alertDialog;
		View _alertView;
		TextView _expenseAlertNameTextView;
		TextView _expenseAlertCostTextView;
		String _expenseName="";
		String _expenseCost="";
		ImageView _expenseHeaderImageView;

		public TripDetailsExpandandableListAdapter (Context ctx , String SelectedIndex)
		{
			_dataSource = TripExpenseDataSource.Instance;
			_context = ctx;
			_activity = (Activity)ctx;
			_tripActivity = (TripDetails)ctx;
			_selectedIndex = SelectedIndex;
			_builder = new AlertDialog.Builder (_context);
			_builder.SetTitle ("Expense Details");
			_builder.SetCancelable (false);
			_builder.SetPositiveButton ("Save", (EventHandler<DialogClickEventArgs>)null);
			_builder.SetNegativeButton ("Cancel", (object sender, DialogClickEventArgs e) => {
			});
			_builder.SetNeutralButton ("Delete", (object sender, DialogClickEventArgs e) => 
			{
					int result = _dataSource.deleteExpense( Convert.ToInt32(_selectedExpenseIndex));
					if( result > 0 )
					{
						NotifyDataSetChanged();
						_tripActivity.UpdateSpentData(_dataSource.CurrentTrip.Spent);
						Toast.MakeText(_context, "Expense deleted successfully",ToastLength.Short).Show();
					}
					else 
						Toast.MakeText(_context, "Error while deleting expense",ToastLength.Short).Show();
			});
			if (_selectedIndex != "-1") 
			{
				_dataSource.getAllExpense (Convert.ToInt32 (_selectedIndex));
				_tripActivity.UpdateSpentData(_dataSource.CurrentTrip.Spent);
				NotifyDataSetChanged ();
			}
		}

		public void InflateTripDetails()
		{
			TripNameTextView = _tripdetailsView.FindViewById<TextView> (Resource.Id.TDTripNameTextView);
			TripDescTextView = _tripdetailsView.FindViewById<TextView> (Resource.Id.TDTripDescTextView);
			TripFromTextView = _tripdetailsView.FindViewById<TextView> (Resource.Id.TDFromTextView);
			TripToTextView = _tripdetailsView.FindViewById<TextView> (Resource.Id.TDToTextView);
			TripFromDateTextView = _tripdetailsView.FindViewById<TextView> (Resource.Id.TDFromDateTextView);
			TripToDateTextView = _tripdetailsView.FindViewById<TextView> (Resource.Id.TDToDateTextView);
			TripAllocatedTextView  = _tripdetailsView.FindViewById<TextView> (Resource.Id.TDAllocatedAmtTextView);

			/*TripNameTextView.Text = "Trip 1";
			TripDescTextView.Text = "Trip Description 1";
			TripFromTextView.Text = "Chn";
			TripToTextView.Text = "Svks";
			TripFromDateTextView.Text = "2-FEB-2015";
			TripToDateTextView.Text = "2-FEB-2015";
			TripAllocatedTextView.Text= "5000";*/

			SaveButton = _tripdetailsView.FindViewById<Button> (Resource.Id.SaveButton);
			SaveButton.Click += (object sender, EventArgs e) => { SaveTripData(); };

			ImageView TripFromDateImageView = _tripdetailsView.FindViewById<ImageView> (Resource.Id.TDFromDateImageControl);
			TripFromDateImageView.Click += (object sender, EventArgs e) => { ShowDialog(1); };
			ImageView TripToDateImageView = _tripdetailsView.FindViewById<ImageView> (Resource.Id.TDToDateImageControl);
			TripToDateImageView.Click += (object sender, EventArgs e) => { ShowDialog(2); };

			if( _selectedIndex != "-1" )
				populateValues ();
		}

		void populateValues()
		{
			_dataSource.MoveTripTo (Convert.ToInt32(_selectedIndex));
			TripDetailsClass trip = _dataSource.CurrentTrip;
			TripNameTextView.Text = trip.Tripname;
			TripDescTextView.Text = trip.TripDesc;
			TripFromTextView.Text = trip.From;
			TripToTextView.Text = trip.To;
			TripFromDateTextView.Text = trip.FromDate;
			TripToDateTextView.Text = trip.ToDate;
			TripAllocatedTextView.Text= "" + trip.Allocated;
		}

		void SaveTripData()
		{
			TripDetailsClass trip = new TripDetailsClass ();
			if (_selectedIndex == "-1")
				trip.TripId = -1;
			else 
			{
				_dataSource.MoveTripTo (Convert.ToInt32 (_selectedIndex));
				trip.TripId = _dataSource.CurrentTrip.TripId;
			}
			trip.Tripname = TripNameTextView.Text ;
			trip.TripDesc = TripDescTextView.Text;
			trip.From = TripFromTextView.Text;
			trip.To = TripToTextView.Text;
			trip.FromDate = TripFromDateTextView.Text;
			trip.ToDate = TripToDateTextView.Text;
			trip.Allocated = String.IsNullOrEmpty(TripAllocatedTextView.Text) ? 0.0 : Convert.ToDouble(TripAllocatedTextView.Text);
			int result = _dataSource.addTripToDataSource (trip);
			if (result > 0) 
			{
				if (_selectedIndex == "-1") 
				{
					_selectedIndex = "" + (_dataSource.TripCount - 1);
				}
				NotifyDataSetChanged ();
				Toast.MakeText (_context, "Data Updated Successfully", ToastLength.Short).Show ();
			}
			else
				Toast.MakeText (_context, "Error while saving Data", ToastLength.Short).Show ();
		}

		void ShowDialog(int control)
		{
			DateControlClicked = control;
			_tripActivity.myShowDialog ();
		}

		public void showExpenseDialog(String selectedExpenseIndex)
		{
			_selectedExpenseIndex = selectedExpenseIndex;
			if (_selectedIndex != "-1") 
			{
				_alertView = _activity.LayoutInflater.Inflate (Resource.Layout.ExpenseAlert, null);
				_expenseAlertNameTextView = _alertView.FindViewById<TextView> (Resource.Id.ExpenseNameTextView);
				_expenseAlertCostTextView = _alertView.FindViewById<TextView> (Resource.Id.ExpenseCostTextView);
				_builder.SetView (_alertView);
				_alertDialog = _builder.Create ();
				if (_selectedExpenseIndex != "-1") 
				{
					_dataSource.MoveExpenseTo (Convert.ToInt32 (_selectedExpenseIndex));
					_expenseAlertNameTextView.Text = _dataSource.CurrentExpense.ExpenseName;
					_expenseAlertCostTextView.Text = "" + _dataSource.CurrentExpense.ExpenseCost;
				}
				_alertDialog.Show ();
				_alertDialog.GetButton ((int)DialogButtonType.Positive).Click += OnPositiveButtonClicked;
			}
			else 
			{
				Toast.MakeText (_context, "Save the Trip Details above and create Expense", ToastLength.Short).Show ();
			}
		}


		void OnPositiveButtonClicked (object sender, EventArgs e)
		{
			_expenseName = _expenseAlertNameTextView.Text;
			_expenseCost = _expenseAlertCostTextView.Text;

			if (String.IsNullOrEmpty (_expenseName) == false && String.IsNullOrEmpty (_expenseName) == false) 
			{
				ExpenseDetailsClass expense = new ExpenseDetailsClass ();
				expense.TripId = Convert.ToInt32 (_selectedIndex);
				expense.ExpenseName = _expenseName;
				expense.ExpenseCost = Convert.ToDouble (_expenseCost);
				int result = _dataSource.addExpenseToTripDataSource (Convert.ToInt32 (_selectedIndex), Convert.ToInt32 (_selectedExpenseIndex), expense);
				if (result > 0) 
				{
					Toast.MakeText (_context, "Data Saved Successfully", ToastLength.Short).Show ();
					_tripActivity.UpdateSpentData (_dataSource.CurrentTrip.Spent);
					NotifyDataSetChanged ();
				}
				else 
				{
					Toast.MakeText (_context, "Error while saving data", ToastLength.Short).Show ();
				}
				_alertDialog.Dismiss ();
			}
			else 
			{
				Toast.MakeText (_context, "Expense Name and Cost is mandatory", ToastLength.Short).Show ();
			}
		}

		public void setTripDate(String date)
		{
			if (DateControlClicked == 1)
				TripFromDateTextView.Text = date;
			else
				TripToDateTextView.Text = date;
		}

		#region implemented abstract members of BaseExpandableListAdapter

		public override View GetGroupView (int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			_expanListView = (ExpandableListView)parent;
			View header = convertView;
			if (header == null) 
			{
				header = _activity.LayoutInflater.Inflate (Resource.Layout.TripExpenseDetailsHeader, null);
			}

			_expenseHeaderImageView = header.FindViewById<ImageView> (Resource.Id.TripExpenseHeaderStatusImageView);

			String headerText = "";
			if (groupPosition == 0) 
			{
				headerText = "Trip Details";
				_expenseHeaderImageView.Visibility = ViewStates.Invisible;
			}
			else 
			{
				_expenseHeaderImageView.Visibility = ViewStates.Visible;
				if (_selectedIndex != "-1") 
				{
					double allocated = _dataSource.CurrentTrip.Allocated;
					double spent = _dataSource.CurrentTrip.Spent;

					if (allocated > 0 && spent > 0) {
						if (spent > allocated)
							_expenseHeaderImageView.SetImageResource (Resource.Drawable.red32);
						else if (spent == allocated)
							_expenseHeaderImageView.SetImageResource (Resource.Drawable.orange32);
						else if (spent < allocated)
							_expenseHeaderImageView.SetImageResource (Resource.Drawable.green32);
					} else {
						_expenseHeaderImageView.SetImageResource (Resource.Drawable.grey32);
					}
				}
				headerText = "Expense Details";
			}

			header.FindViewById<TextView> (Resource.Id.TDHeaderTextView).Text = headerText ;
			return header;
		}

		public override void OnGroupExpanded (int groupPosition)
		{
			base.OnGroupExpanded (groupPosition);  
			if (myExpandedGroup != groupPosition)
				_expanListView.CollapseGroup (myExpandedGroup);

			myExpandedGroup = groupPosition;
		}

		public override View GetChildView (int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
			if (groupPosition == 0 )
			{
				if (_tripdetailsView == null)
					_tripdetailsView = _activity.LayoutInflater.Inflate (Resource.Layout.TripDetails, null);

				if (isTripViewNew == true)
						InflateTripDetails ();
				isTripViewNew = false;

				return _tripdetailsView;
			}
			else
			{
				//View row = convertView;
				//if ( row == null )
				View row = _activity.LayoutInflater.Inflate (Resource.Layout.ExpenseListItem, null);

				row.FindViewById<TextView> (Resource.Id.ListExpenseNameTextView).Text = _dataSource.CurrentTrip.ExpenseDetails [childPosition].ExpenseName;
				row.FindViewById<TextView> (Resource.Id.ListExpenseCostTextView).Text = "" + _dataSource.CurrentTrip.ExpenseDetails [childPosition].ExpenseCost;

				return row;
			}
		}

		public override Java.Lang.Object GetChild (int groupPosition, int childPosition)
		{
			return null;
		}

		public override long GetChildId (int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override Java.Lang.Object GetGroup (int groupPosition)
		{
			return null;
		}

		public override long GetGroupId (int groupPosition)
		{
			return groupPosition;
		}

		public override bool IsChildSelectable (int groupPosition, int childPosition)
		{
			return true;
		}

		public override int GetChildrenCount (int groupPosition)
		{
			if (groupPosition == 0)
				return 1;
			else if (_selectedIndex == "-1")
				return 0;
			else
				return _dataSource.CurrentTrip.ExpenseDetails.Count;
		}

		public override int GroupCount 
		{
			get 
			{
				return 2;
			}
		}

		public override bool HasStableIds 
		{
			get 
			{
				return true;
			}
		}

		#endregion
	}
}

