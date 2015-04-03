using System;
using Android.Widget;
using Android.Views;
using Android.Util;
using RssReaderLibrary;
using Android.Content;
using Android.App;
using System.Collections.Generic;

namespace RssReader
{
	public class RssListAdapter : BaseAdapter<RssFeedDetails>
	{
		Context _context;
		RssManager _rssManager;
		ProgressDialog progressDialog;

		public override int Count 
		{
			get 
			{
				return _rssManager.Current.feedDetails.Count;
			}
		}

		public RssListAdapter (Context ctx, RssManager rssmanage)
		{
			_context = ctx;
			_rssManager = rssmanage;
			progressDialog= new ProgressDialog (ctx);
			progressDialog.SetMessage ("Retreiving Rss Feed details......Please wait !!!!");
			progressDialog.SetProgressStyle (ProgressDialogStyle.Spinner);
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return base.GetItem (position);
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override RssFeedDetails this[int index] 
		{
			get 
			{
				return _rssManager.Current.feedDetails[index];
			}
		}

		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			View drawerListItemView = convertView;
			if (drawerListItemView == null) 
			{
				LayoutInflater inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				drawerListItemView = inflater.Inflate (Resource.Layout.RssViewDetails, null);
			}

			TextView titleText = drawerListItemView.FindViewById<TextView> (Resource.Id.RssTitleTextView);
			TextView descText = drawerListItemView.FindViewById<TextView> (Resource.Id.RssDescTextView);
			TextView urlText = drawerListItemView.FindViewById<TextView> (Resource.Id.RssUrlTextView);

			titleText.Text = this[position].Title;
			descText.Text = this [position].Desc;
			urlText.Text = this [position].Url;
			return drawerListItemView;
		}

		public String getFeedDetailsItemUrl(int position)
		{
			return _rssManager.Current.feedDetails[position].Url;
		}

		public void populateRssFeedDetails(int position)
		{
			progressDialog.Show ();
			try 
			{
				progressDialog.SetMessage ("Retreiving Rss Feed details......Please wait !!!!");
				populateRssFeedDetailsAsync(position);
			}
			catch(Exception e) 
			{
				Toast.MakeText (_context, e.Message, ToastLength.Long).Show ();
			}
		}

		private async void populateRssFeedDetailsAsync(int position)
		{
			try 
			{
				_rssManager.Current.feedDetails = (List<RssFeedDetails>) await _rssManager.getFeedDetailsAsync (position);
				if( _rssManager.Current.feedDetails.Count > 0 )
				{
					progressDialog.SetMessage("Populating the Rss Feed Details");
					this.NotifyDataSetChanged();
				}
			}
			catch(Exception e) 
			{
				Toast.MakeText (_context, e.Message, ToastLength.Long).Show ();
			}
			finally
			{
				progressDialog.Dismiss ();
			}
		}
	}
}

