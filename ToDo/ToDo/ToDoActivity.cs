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
using ToDoLibrary;
using Android.Net;

namespace ToDo
{
	[Activity (Label = "ToDo")]			
	public class ToDoActivity : Activity
	{
		ExpandableListView ToDoListView;
		IMenuItem addNewTodoButton;
		ToDoListViewAdapter todoListAdapter;
		public ProgressDialog progressDialog;
		public static String AlertDialogParent=null;
		TextView todoTitleTextView;
		TextView todoDescTextView;
		TextView todoDateTextView;
		TextView todoTimeTextView;
		Button SaveTodoButton;
		AlertDialog.Builder alertDialogBuilder;
		AlertDialog alertDialog;
		View AlertView;
		ImageButton dateButton;
		ImageButton timeButton;
		Button markCompletedButton;
		String Username;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ToDoMain);
			Username = (String)Intent.GetStringExtra("UserName");
			progressDialog = new ProgressDialog (this);
			progressDialog.SetProgressStyle (ProgressDialogStyle.Spinner);
			progressDialog.SetMessage ("Retrieving Data. Please wait....");
			progressDialog.SetCancelable (false);
			//progressDialog.Show ();

			todoListAdapter = new ToDoListViewAdapter (this, Username);
			ToDoListView = FindViewById<ExpandableListView> (Resource.Id.ToDoExpandableListView);
			ToDoListView.SetAdapter (todoListAdapter);

			ToDoListView.ChildClick+= (object sender, ExpandableListView.ChildClickEventArgs e) => 
			{
				Toast.MakeText(this, "Child Click at : " + e.ChildPosition,ToastLength.Short);
			};

			alertDialogBuilder = new AlertDialog.Builder (this);
			alertDialogBuilder.SetTitle ("New");
			alertDialogBuilder.SetMessage("Enter details for the ToDo");
			alertDialogBuilder.SetCancelable (true);
			alertDialogBuilder.SetNegativeButton ("Cancel", (object sender, DialogClickEventArgs e) => 
				{
				});
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);
			MenuInflater.Inflate (Resource.Menu.ToDoMenu, menu);
			addNewTodoButton = menu.FindItem(Resource.Id.action_add);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int menuId = item.ItemId;

			switch (menuId) 
			{
				case Resource.Id.action_add:
					AddToDoClicked ();
					break;
			}
			return true;
		}

		public void AddToDoClicked()
		{
			AlertDialogParent = "Add";

			AlertView = LayoutInflater.Inflate (Resource.Layout.ToDoLayout, null);
			todoTitleTextView = AlertView.FindViewById<TextView> (Resource.Id.ToDoTitleEditText);
			todoDescTextView = AlertView.FindViewById<TextView> (Resource.Id.ToDoDescEditText);
			todoDateTextView = AlertView.FindViewById<TextView> (Resource.Id.ToDoDureDateTextView);
			todoTimeTextView = AlertView.FindViewById<TextView> (Resource.Id.ToDoDureTimeTextView);
			SaveTodoButton = AlertView.FindViewById<Button> (Resource.Id.CreateToDoButton);
			dateButton = AlertView.FindViewById<ImageButton> (Resource.Id.DateButton);
			timeButton = AlertView.FindViewById<ImageButton> (Resource.Id.TimeButton);
			markCompletedButton = AlertView.FindViewById<Button> (Resource.Id.MarkCompleted);
			markCompletedButton.Visibility = ViewStates.Gone;

			dateButton.Click += DateImageButtonClicked;

			SaveTodoButton.Click += AddToDo;
			alertDialogBuilder.SetView (AlertView);
			alertDialog = alertDialogBuilder.Create ();
			alertDialog.Show ();
		}

		public void DateImageButtonClicked(object sender, EventArgs e)
		{
			ShowDialog (1111);
		}

		protected override Dialog OnCreateDialog (int id)
		{
			if (id == 1111) 
			{
				DatePickerDialog datePicker = new DatePickerDialog (this, HandleDateSet, 2015, 1, 2);
				datePicker.DatePicker.MinDate = new Java.Util.Date ().Time - 1000;
				return datePicker;
			}
			else if( id == 0 )
			{
				TimePickerDialog timePicker = new TimePickerDialog (this, TimePickerCallback, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, false);
				return timePicker;
			}
			return null;
		}

		void HandleDateSet (object sender, DatePickerDialog.DateSetEventArgs e)
		{
			if (AlertDialogParent.Equals ("Add"))
				todoDateTextView.Text = e.Date.ToString ("dd-MMM-yyyy");
			else
				todoListAdapter.AlertDateImageButtonClicked (e.Date.ToString ("dd-MMM-yyyy"));
		}

		public void TimeImageButtonClicked(object sender, EventArgs e)
		{
			ShowDialog (0);
		}

		public void myRunOnUiThread()
		{
			RunOnUiThread (() => todoListAdapter.NotifyDataSetChanged ());
		}

		private void TimePickerCallback (object sender, TimePickerDialog.TimeSetEventArgs e)
		{
			/*int hour = e.HourOfDay;
			String time = (hour > 12 ? hour - 12 : hour) + " : " + e.Minute + (hour >= 12 ? "  PM" : "  AM");
			if (AlertDialogParent.Equals ("Add"))
				todoTimeTextView.Text = time;
			else
				todoListAdapter.AlertTimeImageButtonClicked (time);*/
		}

		public async void AddToDo(object sender, EventArgs e)
		{
			if (verifyControlsData () == true) 
			{
				ToDoClass todo = new ToDoClass ();
				todo.Title = todoTitleTextView.Text;
				todo.Description = todoDescTextView.Text;
				todo.DueDate = todoDateTextView.Text;
				todo.DueTime = todoTimeTextView.Text;
				todo.isCompleted = false;
				todo.Username = Username;
				alertDialog.Dismiss ();
				todoListAdapter.NewToDo (todo);
				Toast.MakeText (this, "ToDo created successfully", ToastLength.Short).Show ();
			}
		}

		private bool verifyControlsData()
		{
			String msg = null;

			if( String.IsNullOrEmpty(todoTitleTextView.Text) == true )
				msg = "Enter ToDo Title";
			//else if( String.IsNullOrEmpty(todoDescTextView.Text) == true )
			//	msg = "Enter ToDo Title";

			if (String.IsNullOrEmpty (msg))
				return true;
			else 
			{
				Toast.MakeText (this, msg, ToastLength.Short).Show();
				return false;
			}
		}
	}
}

