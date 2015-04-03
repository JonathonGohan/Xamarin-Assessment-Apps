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
using RestaurantLibrary;
using System.Threading.Tasks;
using Android.Text;

namespace RestaurantActivity
{
	[Activity (Label = "User Profile")]			
	public class UserDetailsActivity : Activity
	{
		UserDetailsDataSource _userDetailsDataSource;
		Spinner _bookingDetailsSpinner;
		BookingDetailsSpinnerAdapter _bookingDetailsSpinnerAdapter;
		Button _viewOrderDetailsButton;
		ProgressDialog _progress;
		int orderId;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			View mainView = LayoutInflater.Inflate (Resource.Layout.UserDetails, null);
			SetContentView (mainView);

			_progress = new ProgressDialog (this);
			_progress.SetCancelable (false);
			_progress.SetProgressStyle (ProgressDialogStyle.Spinner);
			_progress.SetMessage (" Please wait !! ");
			_progress.Show ();

			_userDetailsDataSource = UserDetailsDataSource.Instance ;

			_bookingDetailsSpinnerAdapter = new BookingDetailsSpinnerAdapter (this, Resource.Layout.BookingDetailsSpinnerItem, _userDetailsDataSource.UserInfo.EmailId);

			_viewOrderDetailsButton = FindViewById<Button> (Resource.Id.BookDetailsViewOrderDetailsButton);
			_viewOrderDetailsButton.Enabled = false;
			_viewOrderDetailsButton.Click += (object sender, EventArgs e) => 
			{
				displayOrderDetails();
			};

			FindViewById<TextView> (Resource.Id.BookDetailsUsernameTextView).Text = _userDetailsDataSource.UserInfo.Username;
			FindViewById<TextView> (Resource.Id.BookDetailsEmailTextView).Text = _userDetailsDataSource.UserInfo.EmailId;

			_bookingDetailsSpinnerAdapter = new BookingDetailsSpinnerAdapter (this, Resource.Layout.BookingDetailsSpinnerItem, _userDetailsDataSource.UserInfo.EmailId);
			_bookingDetailsSpinner = FindViewById<Spinner> (Resource.Id.BookingDetailsSpinner);
			_bookingDetailsSpinner.Adapter = _bookingDetailsSpinnerAdapter;
			_bookingDetailsSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => 
			{
				_viewOrderDetailsButton.Enabled = true;
				orderId = _bookingDetailsSpinnerAdapter.displaySelectedBookingDetails(mainView, (int)e.Id);
			};
		}

		public void closeProgressBar()
		{
			_progress.Dismiss ();
		}

		void displayOrderDetails()
		{
			getOrderedFoodItems ();
		}

		async void getOrderedFoodItems()
		{
			_progress.Show ();
			AlertDialog.Builder alertBuilder = new AlertDialog.Builder (this);
			alertBuilder.SetTitle ("Order Details");
			alertBuilder.SetPositiveButton ("Ok", (object sender, DialogClickEventArgs e) => {
			});
			View alertView = LayoutInflater.Inflate (Resource.Layout.BookedOrderetails, null);
			ListView _cartListView = alertView.FindViewById<ListView> (Resource.Id.BookedOrderDetailsListView);
			_cartListView.Clickable = false;
			BookedOrederDetailsListAdapter _adapter = new BookedOrederDetailsListAdapter (this);
			double cost = await _adapter.getAllFoodItemsByOrder (orderId);
			double vat = (cost + 250) * ((double)5 / 100);

			alertView.FindViewById<TextView> (Resource.Id.BookedOrderDetailsTotalCost).TextFormatted = Html.FromHtml ("Total Cost : <font color=red>Rs. " + (cost + 250 + vat) + "</font><font color=grey> ( Incl. BookingAdvance <b>Rs. 250</b> <font color=red>+</font> VAT 5%  <b> Rs. " + vat + "  </b> ) </font>");
			_cartListView.Adapter = _adapter;
			alertBuilder.SetView (alertView);
			_progress.Dismiss ();
			alertBuilder.Show ();
		}

		public override void OnBackPressed ()
		{
			Finish ();
			OverridePendingTransition (Resource.Animation.pushupin, Resource.Animation.pushupout);
		}
	}
}

