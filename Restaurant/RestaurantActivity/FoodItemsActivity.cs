
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

namespace RestaurantActivity
{
	[Activity (Label = "Menu")]		
	//[MetaData("android.support.PARENT_ACTIVITY", Value = "HomeActivity")]
	public class FoodItemsActivity : Activity
	{
		FoodItemManager _fooditemsManager;
		RestaurantManager _restaurantManager;
		int _restaurantId=-1;
		ProgressDialog _progress;
		String[] tabs;
		int[] tabIds;

		TextView _foodTypeTextView;
		ListView _foodItemsListView;
		TextView _bookingTotalCostTextView;

		FoodItemsListAdapter _foodItemsListAdapter;

		Button _confirmPayButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.FoodItemsParent);

			_restaurantManager = new RestaurantManager (this);
			_restaurantId = _restaurantManager.getCurrentRestaurant ().Id;
			_fooditemsManager = new FoodItemManager ( _restaurantId , this);

			_progress = new ProgressDialog (this);
			_progress.SetCancelable (false);
			_progress.SetProgressStyle (ProgressDialogStyle.Spinner);
			_progress.SetMessage (" Please wait !! ");

			_confirmPayButton = FindViewById<Button> (Resource.Id.ConfirmOrderButton);
			_confirmPayButton.Click += (object sender, EventArgs e) => 
			{
				OnConfirmOrder();
			};

			_foodTypeTextView = FindViewById<TextView> (Resource.Id.FoodItemTypeNameTextView);
			_bookingTotalCostTextView = FindViewById<TextView> (Resource.Id.BookingTotalCostTextView);
			_foodItemsListView = FindViewById<ListView> (Resource.Id.FoodItemsListView);
			_foodItemsListAdapter = new FoodItemsListAdapter (this, _fooditemsManager);

			_foodItemsListView.ItemClick+= (object sender, AdapterView.ItemClickEventArgs e) => 
			{
				TextView tempCountTextView = e.View.FindViewById<TextView>(Resource.Id.FoodItemCountTextView);

				NumberPicker numPicker = new NumberPicker(this);
				numPicker.Orientation = Orientation.Vertical;
				numPicker.MaxValue =20;
				numPicker.MinValue = 0;

				if ( String.IsNullOrEmpty(tempCountTextView.Text) == false )
					numPicker.Value = Convert.ToInt32(tempCountTextView.Text);

				AlertDialog.Builder builder = new AlertDialog.Builder(this);
				builder.SetPositiveButton("Ok", (object sender2, DialogClickEventArgs e2) => 
					{
						TextView countTextView = e.View.FindViewById<TextView>(Resource.Id.FoodItemCountTextView);
						countTextView.Text = "" + numPicker.Value;
						_foodItemsListAdapter.updateBookingDetails( (int)e.Id , numPicker.Value);
						updateTotalCost();
					});
				builder.SetView(numPicker);
				builder.Show();
			};

			AddTabs ();
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);
			MenuInflater.Inflate (Resource.Menu.foodItemsParentMenu, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int id = item.ItemId;
			switch (id) 
			{
			case Resource.Id.contextmenu_shoppingcart:
					populateOrderDetails ();
					break;
			}
			return true;
		}

		async void AddTabs()
		{
			_progress.Show ();
			int result = await _fooditemsManager.getAllFoodItemTypes ();
			if (result > 0) 
			{
				tabs = _restaurantManager.getCurrentRestaurant ().FoodItemTypeAvailable.Split (new String[]{ "," }, StringSplitOptions.RemoveEmptyEntries);
				ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
				tabIds = new int[tabs.Length];
				for (int i = 0; i < tabs.Length; i++) 
				{
					ActionBar.Tab tab = ActionBar.NewTab ();
					tabIds [i] = Convert.ToInt32 (tabs [i]);
					tab.SetText (_fooditemsManager.getFoodItemTypeByTypeId (tabIds [i]));
					tab.TabSelected += (object sender, ActionBar.TabEventArgs e) => 
					{
						int selectedTabIndex = ActionBar.SelectedNavigationIndex;
						populateFoodItems(selectedTabIndex);
					};
					ActionBar.AddTab (tab);
				}
				result = await _fooditemsManager.getAllFoodItems (tabIds);
				_foodItemsListView.Adapter = _foodItemsListAdapter;
				_foodItemsListAdapter.populateFoodItemsByTypeId (tabIds [0]);
			} 

			if (result == -10) 
			{
				Toast.MakeText (this, "No Internet connection", ToastLength.Short).Show ();
			}
			else if (result == 0) 
			{
				Toast.MakeText (this, "No data available", ToastLength.Short).Show ();
			}
			_progress.Dismiss ();
		}

		void populateFoodItems(int selectedTabIndex)
		{
			_foodTypeTextView.Text = _fooditemsManager.getFoodItemTypeByTypeId(tabIds[selectedTabIndex]);
			_foodItemsListAdapter.populateFoodItemsByTypeId( tabIds[selectedTabIndex]);
		}

		void updateTotalCost()
		{
			_bookingTotalCostTextView.Text = _foodItemsListAdapter.getTotalCost ();
		}

		void populateOrderDetails()
		{
			OrderDetails details = _fooditemsManager.getCurrentOrderDetails ();

			if (details.foodItems.Count > 0) 
			{
				AlertDialog.Builder alertBuilder = new AlertDialog.Builder (this);
				alertBuilder.SetTitle ("Order Details");
				alertBuilder.SetPositiveButton ("Ok", (object sender, DialogClickEventArgs e) => {
				});
				View alertView = LayoutInflater.Inflate (Resource.Layout.CartDetails, null);
				ListView _cartListView = alertView.FindViewById<ListView> (Resource.Id.CartListView);
				_cartListView.Clickable = false;
				CartListAdapter _cartListAdapter = new CartListAdapter (this);
				_cartListView.Adapter = _cartListAdapter;
				alertBuilder.SetView (alertView);
				alertBuilder.Show ();
			}
			else 
			{
				Toast.MakeText (this, "No Food Item selected", ToastLength.Short).Show ();
			}
		}

		void OnConfirmOrder()
		{
			_progress.Show ();

			AlertDialog.Builder alertBuilder = new AlertDialog.Builder (this);
			alertBuilder.SetTitle ("Amount Details");
			alertBuilder.SetPositiveButton ("Ok", (object sender, DialogClickEventArgs e) => {	saveOrderDetails ();	});
			alertBuilder.SetNegativeButton ("Cancel", (object sender, DialogClickEventArgs e) => {});
			View alertView = LayoutInflater.Inflate (Resource.Layout.AmountDetails, null);
			double fooditemscost = _fooditemsManager.getCurrentOrderDetails ().Cost;
			alertView.FindViewById<TextView> (Resource.Id.AmtDetailsItemsCostTextView).Text = "  Rs. " + fooditemscost;
			alertView.FindViewById<TextView> (Resource.Id.AmtDetailsAdvanceTextView).Text = "  Rs. 250.00";
			double vat = (fooditemscost + 250) * ((double)5 / 100);
			alertView.FindViewById<TextView> (Resource.Id.AmtDetailsVatTextView).Text = "  Rs. " + vat;
			alertView.FindViewById<TextView> (Resource.Id.AmtDetailsTotalCostTextView).Text = "Rs. " + (fooditemscost + 250 + vat);
			alertBuilder.SetView (alertView);
			alertBuilder.Show ();

			_progress.Dismiss ();
		}

		async void saveOrderDetails()
		{
			_progress.Show ();
			try
			{
				int orderId = await _fooditemsManager.getOrderCount ();
				if( orderId != -10 )
				{
					int bookingId = await _fooditemsManager.getBookingCount ();
					if( bookingId != -10)
					{
						orderId++;
						bookingId++;

						OrderDetails _orderDetails = _fooditemsManager.getCurrentOrderDetails();
						BookingDetails _bookingDetails = UserDetailsDataSource.Instance.BookingInfo;

						_orderDetails.strFoodItems = "";
						_orderDetails.RestaurantId = _bookingDetails.RestaurantId;
						_orderDetails.EmailId = _bookingDetails.EmailId;
						_orderDetails.Id = orderId;

						foreach (FoodItem item in _orderDetails.foodItems) 
						{
							_orderDetails.strFoodItems += item.Id + ":" + item.OrderCount+",";
						}

						_fooditemsManager.saveOrderDetails ( _orderDetails );
						{
							_bookingDetails.Id = bookingId;
							_bookingDetails.OrderId = orderId;

							_fooditemsManager.saveBookingDetails ( _bookingDetails );
							{
								_progress.Dismiss ();
								AlertDialog.Builder alertdialog = new AlertDialog.Builder(this);
								alertdialog.SetTitle("Success !!");
								alertdialog.SetNeutralButton("Ok", (object sender, DialogClickEventArgs e) => 
								{
										Finish();
										OverridePendingTransition (Resource.Animation.rightin, Resource.Animation.rightout);
								});
								alertdialog.SetMessage(" Booking Id : " + _bookingDetails.Id);
								alertdialog.SetCancelable(false);
								alertdialog.Show();
							}
						}
					}
					else
					{
						_progress.Dismiss ();
						Toast.MakeText (this, "No Internet Connection", ToastLength.Short).Show ();
					}
				}
				else
				{
					_progress.Dismiss ();
					Toast.MakeText (this, "No Internet Connection", ToastLength.Short).Show ();
				}
			}
			catch
			{
				_progress.Dismiss ();
				Toast.MakeText (this, "Error while placing order", ToastLength.Short).Show ();
			}
		}

		void myFinish()
		{
			Finish ();
			OverridePendingTransition (Resource.Animation.rightin, Resource.Animation.rightout);
		}

		public override void OnBackPressed ()
		{
			Finish ();
			OverridePendingTransition (Resource.Animation.rightin, Resource.Animation.rightout);
		}
	}
}

