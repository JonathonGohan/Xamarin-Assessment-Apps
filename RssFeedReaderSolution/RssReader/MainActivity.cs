using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using Android.Net;
using Android.Support.V4.Widget;

namespace RssReader
{
	[Activity (Label = "RssFeedReader", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		WebView myWebView;
		WebSettings myWebSettings;
		View tabWebPageView;
		View tabRssView;
		ListView rssDrawerListView;
		ListView rssDetailsListView;
		AlertDialog.Builder alertBuilder;
		AlertDialog alertDialog;
		EditText alertRssFeedName;
		RssFeedDrawerListAdapter rssDrawerListAdapter;
		RssListAdapter rssListAdapter;
		TextView rssFeedUrlTextView;
		TextView selectedRssFeedNameTextView;
		ImageButton addRssFeedButton;

		ActionBar.Tab tab1 ;
		ActionBar.Tab tab2 ;

		NetworkInfo netInfo;

		DrawerLayout drawerLayout;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			LoadAlertDialog ();
			LoadWebViewTab ();
			LoadRssViewTab ();
		}

		public void LoadAlertDialog()
		{
			alertBuilder = new AlertDialog.Builder (this);
			alertBuilder.SetCancelable (false);
			alertBuilder.SetTitle ("Rss Feed name");
			alertBuilder.SetPositiveButton ("Save", (EventHandler<DialogClickEventArgs>)null);
			alertBuilder.SetNegativeButton("Cancel",(object sender, DialogClickEventArgs e) => 
				{
				});
		}

		public void LoadWebViewTab()
		{
			tabWebPageView = LayoutInflater.Inflate (Resource.Layout.WebPage, null);
			myWebView = tabWebPageView.FindViewById<WebView> (Resource.Id.webView1);
			myWebView.SetWebViewClient (new RssWebViewClient ());

			myWebSettings = myWebView.Settings;
			myWebSettings.JavaScriptEnabled = true;
		}

		public void LoadRssViewTab()
		{
			tabRssView = LayoutInflater.Inflate (Resource.Layout.RssViewer, null);
			rssFeedUrlTextView = tabRssView.FindViewById<TextView> (Resource.Id.RssFeedUrlTextView);
			selectedRssFeedNameTextView = tabRssView.FindViewById<TextView> (Resource.Id.RssSelectedFeedName);
			addRssFeedButton = tabRssView.FindViewById<ImageButton> (Resource.Id.AddRssFeedButton);
			addRssFeedButton.Click += (object sender, EventArgs e) => 
			{
				if ( String.IsNullOrEmpty (rssFeedUrlTextView.Text) == false )
				{
					alertRssFeedName = new EditText (this);
					alertRssFeedName.Hint = "Name for Rss Feed";
					alertBuilder.SetView (alertRssFeedName);
					alertDialog = alertBuilder.Create();
					alertDialog.Show();
					alertDialog.GetButton((int)DialogButtonType.Positive).Click += OnPositiveButtonClicked;
				}
				else
					Toast.MakeText(this,"Enter Rss Feed Url", ToastLength.Short).Show();
			};

			AddTabs ();
			LoadDrawer ();

			rssDetailsListView = tabRssView.FindViewById<ListView> (Resource.Id.RssFeedDetailsListView);
			rssListAdapter = new RssListAdapter (this, rssDrawerListAdapter._rssManager);
			rssDetailsListView.Adapter = rssListAdapter;
			rssDetailsListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
			{
				if ( checkInternetConnection() )
				{
					tab2.Select();
					myWebView.LoadUrl( rssListAdapter.getFeedDetailsItemUrl(e.Position));
				}
				else
					Toast.MakeText(this,"Internet Connection unavailable !!!!. Kindly check and try again",ToastLength.Long).Show();
			};
		}

		public void AddTabs()
		{
			SetContentView (tabRssView);

			tab1 = ActionBar.NewTab ();
			tab1.SetText("Rss Feeds");

			tab1.TabSelected += (sender, e) => 
			{
					SetContentView(tabRssView);
			};

			tab2 = ActionBar.NewTab();
			tab2.SetText(" Web View");
			tab2.TabSelected += (sender, e) => 
			{
					SetContentView(tabWebPageView);
			};

			ActionBar.AddTab (tab1);
			ActionBar.AddTab (tab2);

			ActionBar.SelectTab (tab1);
		}

		public void LoadDrawer()
		{
			rssDrawerListAdapter = new RssFeedDrawerListAdapter (this);
			drawerLayout = FindViewById<DrawerLayout> (Resource.Id.drawerLayout);
			rssDrawerListView = FindViewById<ListView> (Resource.Id.drawerListView);
			rssDrawerListView.Adapter = rssDrawerListAdapter;

			rssDrawerListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
			{
				rssFeedUrlTextView.Text = rssDrawerListAdapter.getFeedUrl(e.Position);
				selectedRssFeedNameTextView.Text = rssDrawerListAdapter.getFeedName(e.Position);
				drawerLayout.CloseDrawer(rssDrawerListView);
				rssListAdapter.populateRssFeedDetails(e.Position);
			};
		}

		public void OnPositiveButtonClicked(object sender, EventArgs e)
		{
			String msg = "";
			if (String.IsNullOrEmpty (alertRssFeedName.Text) == true)
				msg = "Enter the name for Rss Feed";
			else
			{
				int result = rssDrawerListAdapter.addNewRssFeed(alertRssFeedName.Text, rssFeedUrlTextView.Text);
				if (result > 0) 
				{
					msg = "RssFeed added successfully";
					alertDialog.Dismiss ();
				}
				else if( result == -2)
					msg = "RssFeed name already Exists";
				else
					msg = "Error while adding RssFeed";
			}
			Toast.MakeText (this, msg, ToastLength.Short).Show ();
		}

		public bool checkInternetConnection()
		{
			var cm = (ConnectivityManager)GetSystemService (Context.ConnectivityService);
			netInfo = cm.ActiveNetworkInfo;
			if (netInfo != null && netInfo.IsConnectedOrConnecting)
				return true;
			return false;
		}
	}
}


