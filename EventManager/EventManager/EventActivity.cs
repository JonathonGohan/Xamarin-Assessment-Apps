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
using Android.Support.V4.Widget;
using EventLibrary;

namespace EventManager
{
	[Activity (Label = "Create Event")]			
	public class EventActivity : Activity
	{
		DrawerLayout drawerLayout;
		ListView eventsListView;
		EventClassManager eventClassManager;

		TextView eventText;
		Spinner locationSpinner;
		ImageButton DateImageButton;
		ImageButton TimeImageButton;
		Spinner hallSpinner;
		Button saveButton;
		TextView DateTextView;
		TextView TimeTextView;

		TextView ConfHallNameTextView;
		TextView ConfOpenTextView;
		TextView ConfLandTextView;
		TextView ConfAmtextView;
		TextView ConfPhoneTextView;

		IMenuItem deleteMenuItem;
		IMenuItem editMenuItem;

		View AlertView;

		private int _selectedEventId;

		AlertDialog.Builder alertDialog;

		public int selectedEventId
		{
			get
			{
				return _selectedEventId;
			}

			set
			{
				_selectedEventId = value;
				if (deleteMenuItem != null && editMenuItem != null) 
				{
					if (_selectedEventId == -1) 
					{
						setMenuItemEnableState (false, 90);
					} 
					else 
					{
						setMenuItemEnableState (true, 255);
					}
				}
			}
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.EventActivity);

			eventText = FindViewById<TextView> (Resource.Id.EventNameText);
			locationSpinner = FindViewById<Spinner> (Resource.Id.LocationSpinner);
			DateImageButton = FindViewById<ImageButton> (Resource.Id.DateImageButton);
			TimeImageButton = FindViewById<ImageButton> (Resource.Id.TimeImageButton);
			hallSpinner = FindViewById<Spinner> (Resource.Id.HallSpinner);
			saveButton =  FindViewById<Button> (Resource.Id.SaveEventButton);
			DateTextView = FindViewById<TextView> (Resource.Id.DateTextView);
			TimeTextView = FindViewById<TextView> (Resource.Id.TimeTextView);

			DateImageButton = FindViewById<ImageButton> (Resource.Id.DateImageButton);
			TimeImageButton = FindViewById<ImageButton> (Resource.Id.TimeImageButton);

			DateImageButton.Click += DateImageButtonClicked;
			TimeImageButton.Click += TimeImageButtonClicked;

			saveButton.Click += OnSaveEventClicked; 

			drawerLayout = FindViewById<DrawerLayout> (Resource.Id.drawerLayout);
			eventsListView = FindViewById<ListView> (Resource.Id.drawerListView);

			eventClassManager = new EventClassManager (this ,  Intent.GetStringExtra ("LoggedInUserName"));

			eventsListView.Adapter = new EventManagerAdapter (this, Resource.Layout.DrawerEventItem, eventClassManager);
			eventsListView.ItemClick += OnEventListItemClicked;

			locationSpinner.Adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleSpinnerItem, EventClassManager.Location);
			locationSpinner.ItemSelected += LocationSpinnerSelected; 

			selectedEventId = -1;

			alertDialog = new AlertDialog.Builder (this);
			alertDialog.SetTitle ("Confirmation");
			alertDialog.SetMessage("Confirm the Event Details");
			alertDialog.SetCancelable (true);
			AlertView = LayoutInflater.Inflate (Resource.Layout.EventConfirmation, null);
			//alertDialog.SetView (AlertView);
			alertDialog.SetPositiveButton ("Save", (object sender, DialogClickEventArgs e) => 
				{
					SaveData();
				});
			alertDialog.SetNegativeButton ("Cancel", (object sender, DialogClickEventArgs e) => 
				{
				});
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
			DateTextView.Text = e.Date.ToString("dd-MMM-yyyy");
		}

		public void TimeImageButtonClicked(object sender, EventArgs e)
		{
			ShowDialog (0);
		}

		private void TimePickerCallback (object sender, TimePickerDialog.TimeSetEventArgs e)
		{
			int hour = e.HourOfDay;
			TimeTextView.Text = ( hour > 12 ? hour -12 : hour ) + " : " + e.Minute + (hour >= 12 ? "  PM" : "  AM") ;
		}

		public void LocationSpinnerSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			hallSpinner.Adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleSpinnerItem, EventClassManager.Hall[e.Position]);
		}

		public void OnEventListItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			selectedEventId = e.Position;
			UpdateUiData (e.Position);
			drawerLayout.CloseDrawer (eventsListView);
		}

		private void UpdateUiData(int position)
		{
			this.Title = "View Event";
			setControlsData (position);
			setControlsEnabled (false);
		}

		private void setControlsData(int position)
		{
			if (position != -1) 
			{
				eventClassManager.moveTo (position);
				EventClass eventClass = eventClassManager.Current;
				eventText.Text = eventClass.EventName;
				DateTextView.Text = eventClass.Date;
				TimeTextView.Text = eventClass.Time;

				int locationIndex = Array.IndexOf (EventClassManager.Location, eventClass.Location);
				locationSpinner.SetSelection (locationIndex);

				hallSpinner.Adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleSpinnerItem, EventClassManager.Hall[locationIndex]);
				int hallIndex = Array.IndexOf (EventClassManager.Hall[locationIndex], eventClass.hall);
				hallSpinner.SetSelection (hallIndex);
				return;
			}

			eventText.Text = "";
			DateTextView.Text = "";
			TimeTextView.Text = "";
			locationSpinner.SetSelection (0);
		}

		private void setControlsEnabled(bool state)
		{
			eventText.Enabled = state;
			locationSpinner.Enabled = state;
			DateImageButton.Enabled = state;
			TimeImageButton.Enabled = state;
			hallSpinner.Enabled = state;
			saveButton.Enabled = state;
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.EventActivity, menu);
			deleteMenuItem = menu.FindItem (Resource.Id.action_delete);
			editMenuItem = menu.FindItem (Resource.Id.action_edit);
			setMenuItemEnableState (false, 90);
			return true;
		}

		private void setMenuItemEnableState(bool state , int alpha)
		{
			deleteMenuItem.SetEnabled (state);
			deleteMenuItem.Icon.SetAlpha (alpha);
			editMenuItem.SetEnabled (state);
			editMenuItem.Icon.SetAlpha (alpha);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int menuItemId = item.ItemId;
			switch (menuItemId) 
			{
				case Resource.Id.action_add:
					this.Title = "Create Event";
					selectedEventId = -1;
					setControlsData (-1);
					setControlsEnabled (true);
					break;
				case Resource.Id.action_edit:
					this.Title = "Edit Event";
					setControlsEnabled (true);
					break;
				case Resource.Id.action_delete:
					DeleteEvent ();
					this.Title = "Create Event";
					break;
			}
			return true;
		}

		public void DeleteEvent()
		{
			if (eventClassManager.deleteEvent (selectedEventId) == true) 
			{
				setControlsEnabled (true);
				setControlsData (-1);
				selectedEventId = -1;
				Toast.MakeText (this, "Event deleted Successfully", ToastLength.Long).Show();
			}
			else
				Toast.MakeText (this, "Error while deleting Event", ToastLength.Long).Show();
		}

		public void OnSaveEventClicked(object sender, EventArgs e)
		{
			if ( validateControls () == true )
				CallConfirmation ();
		}

		public bool validateControls()
		{
			if (String.IsNullOrEmpty (eventText.Text) == true) 
			{
				Toast.MakeText (this, "Enter the Event Name", ToastLength.Long).Show ();
				return false;
			} 
			else if (String.IsNullOrEmpty (DateTextView.Text) == true) 
			{
				Toast.MakeText (this, "Select the Date of the event", ToastLength.Long).Show ();
				return false;
			}
			else  if (String.IsNullOrEmpty (TimeTextView.Text) == true) 
			{
				Toast.MakeText (this, "Select the Time of the Event", ToastLength.Long).Show ();
				return false;
			}
			return true;
		}

		public void CallConfirmation()
		{
			AlertView = LayoutInflater.Inflate (Resource.Layout.EventConfirmation, null);
			ConfHallNameTextView = AlertView.FindViewById<TextView> (Resource.Id.ConfHallNameText);
			ConfOpenTextView = AlertView.FindViewById<TextView> (Resource.Id.ConfOpeningHrsText);
			ConfLandTextView = AlertView.FindViewById<TextView> (Resource.Id.ConfLandText);
			ConfAmtextView = AlertView.FindViewById<TextView> (Resource.Id.ConfAmtText);
			ConfPhoneTextView = AlertView.FindViewById<TextView> (Resource.Id.ConfPhoneText);
			alertDialog.SetView (AlertView);
			HallDetails halldets = EventClassManager.halls [hallSpinner.SelectedItem.ToString ()];
			ConfHallNameTextView.Text = halldets.HallName;
			ConfOpenTextView.Text = halldets.OpenHours;
			ConfLandTextView.Text = halldets.Landmark;
			ConfAmtextView.Text = halldets.Amount;
			ConfPhoneTextView.Text = halldets.Phone;
			alertDialog.Show ();
		}

		public void SaveData()
		{
			EventClass eventClass = getEventData ();
			if (selectedEventId == -1) 
			{
				if (eventClassManager.insertEvent (eventClass) == true)
					Toast.MakeText (this, "Event created Successfully", ToastLength.Long).Show();
				else
					Toast.MakeText (this, "Error while creating Event", ToastLength.Long).Show();
			} 
			else 
			{
				if (eventClassManager.updateEvent ( selectedEventId , eventClass) == true)
					Toast.MakeText (this, "Event updated Successfully", ToastLength.Long).Show();
				else
					Toast.MakeText (this, "Error while updating Event", ToastLength.Long).Show();
			}
			setControlsData (-1);
		}

		private EventClass getEventData()
		{
			EventClass eventClass = new EventClass ();
			eventClass.EventName = eventText.Text;
			eventClass.Location = locationSpinner.SelectedItem.ToString ();
			eventClass.Date = DateTextView.Text;
			eventClass.Time = TimeTextView.Text;
			eventClass.hall = hallSpinner.SelectedItem.ToString ();
			return eventClass;
		}
	}
}

