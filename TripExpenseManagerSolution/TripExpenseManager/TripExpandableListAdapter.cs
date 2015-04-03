using System;
using Android.Widget;
using Android.Views;
using Android.Content;
using Android.Util;
using Android.App;
using TripExpenseLibrary;

namespace TripExpenseManager
{
	public class TripExpandableListAdapter : BaseExpandableListAdapter
	{
		Context _context;
		Activity _activity;
		ExpandableListView _expanListView;
		TripExpenseDataSource _dataSource;
		ImageView _statusImageView;
		int myExpandedGroup;

		TextView _fromTextView;
		TextView _toTextView;
		TextView _fromdateTextView;
		TextView _todateTextView;
		TextView _allocatedTextView;
		TextView _spentTextView;

		public TripExpandableListAdapter (Context ctx)
		{
			_context = ctx;
			_activity = (Activity)ctx;
			_dataSource = TripExpenseDataSource.Instance;
			_dataSource.openTripDatabase (ctx);
			_dataSource.getAllTrips ();
			NotifyDataSetChanged ();
		}

		public void initControlsWIthView (View view)
		{
			_fromTextView = view.FindViewById<TextView> (Resource.Id.FromTextView);
			_fromdateTextView = view.FindViewById<TextView> (Resource.Id.FromDateTextView);
			_toTextView = view.FindViewById<TextView> (Resource.Id.ToTextView);
			_todateTextView = view.FindViewById<TextView> (Resource.Id.ToDateTextView);
			_allocatedTextView = view.FindViewById<TextView> (Resource.Id.AllocatedTextView);
			_spentTextView = view.FindViewById<TextView> (Resource.Id.SpentTextView);
		}



		#region implemented abstract members of BaseExpandableListAdapter

		public override View GetGroupView (int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			_expanListView = (ExpandableListView)parent;
			View header = convertView;
			if (header == null) {
				header = _activity.LayoutInflater.Inflate (Resource.Layout.tripListViewHeader, null);
			}

			_dataSource.MoveTripTo (groupPosition);
			header.FindViewById<TextView> (Resource.Id.HeaderTripNameTextView).Text = _dataSource.CurrentTrip.Tripname;

			_statusImageView = header.FindViewById<ImageView> (Resource.Id.HeaderTripStatusImageView);

			double allocated = _dataSource.CurrentTrip.Allocated;
			double spent = _dataSource.CurrentTrip.Spent;

			if (allocated > 0 && spent > 0) 
			{
				if ( spent > allocated )
					_statusImageView.SetImageResource (Resource.Drawable.red32);
				else if ( spent == allocated )
					_statusImageView.SetImageResource (Resource.Drawable.orange32);
				else if ( spent < allocated )
					_statusImageView.SetImageResource (Resource.Drawable.green32);
			}
			else 
			{
				_statusImageView.SetImageResource(Resource.Drawable.grey32);
			}

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
			View row = convertView;
			if (row == null) 
			{
				row = _activity.LayoutInflater.Inflate (Resource.Layout.tripListViewListItem, null);
			}

			initControlsWIthView (row);

			_dataSource.MoveTripTo (myExpandedGroup);
			_fromTextView.Text = _dataSource.CurrentTrip.From;
			_fromdateTextView.Text = _dataSource.CurrentTrip.FromDate;
			_toTextView.Text = _dataSource.CurrentTrip.To;
			_todateTextView.Text = _dataSource.CurrentTrip.ToDate;
			_allocatedTextView.Text = "Rs. " + _dataSource.CurrentTrip.Allocated;
			_spentTextView.Text = "Rs. " + _dataSource.CurrentTrip.Spent;
			return row;
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
			return 1;
		}

		public override int GroupCount 
		{
			get 
			{
				return _dataSource.TripCount;
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

