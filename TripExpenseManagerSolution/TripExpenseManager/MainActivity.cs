using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace TripExpenseManager
{
	[Activity (Label = "Trips", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		ExpandableListView _expanListView;
		TripExpandableListAdapter _tripAdapter;
		Button _addTripbutton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			_addTripbutton = FindViewById<Button> (Resource.Id.AddTripButton);
			_addTripbutton.Click += (object sender, EventArgs e) => 
			{
				Intent intent = new Intent(this, typeof(TripDetails));
				intent.PutExtra("SelectedIndex","-1");
				StartActivity(intent);
				//_tripAdapter.addTrip();
			};

			_expanListView = FindViewById<ExpandableListView> (Resource.Id.TripOverviewListView);

			_expanListView.ChildClick += (object sender, ExpandableListView.ChildClickEventArgs e) => 
			{
				Intent intent = new Intent(this, typeof(TripDetails));
				intent.PutExtra("SelectedIndex", "" + e.GroupPosition );
				StartActivity(intent);
			};

			_tripAdapter = new TripExpandableListAdapter (this);
			_expanListView.SetAdapter(_tripAdapter);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
		}

		protected override void OnPostResume ()
		{
			base.OnPostResume ();
			_tripAdapter.NotifyDataSetChanged ();
		}
	}
}


