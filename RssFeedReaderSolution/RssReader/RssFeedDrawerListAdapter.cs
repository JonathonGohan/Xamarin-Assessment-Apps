using System;
using Android.Widget;
using RssReaderLibrary;
using Android.Content;
using Android.Views;
using Android.App;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RssReader
{
	public class RssFeedDrawerListAdapter : BaseAdapter<RssFeed>
	{
		Context _context;
		public RssManager _rssManager;

		public override int Count 
		{
			get 
			{
				return _rssManager.rssCount;
			}
		}

		public RssFeedDrawerListAdapter (Context ctx)
		{
			_context = ctx;
			_rssManager = new RssManager (ctx);
			_rssManager.populateRssFeeds ();
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override RssFeed this[int index] 
		{
			get 
			{
				_rssManager.MoveTo (index);
				return _rssManager.Current;
			}
		}

		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			View drawerListItemView = convertView;
			if (drawerListItemView == null) 
			{
				LayoutInflater inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				drawerListItemView = inflater.Inflate (Resource.Layout.RssViewHeader, null);
			}

			TextView feedName = drawerListItemView.FindViewById<TextView> (Resource.Id.rssHeaderTextView);
			feedName.Text = this[position].Name;
			return drawerListItemView;
		}

		public int addNewRssFeed(String feedName, String url)
		{
			RssFeed newFeed = new RssFeed ();
			newFeed.Name = feedName;
			newFeed.Url = url;
			int result = _rssManager.addNewRssFeed (newFeed);
			if (result > 0)
				NotifyDataSetChanged ();
			return result;
		}

		public String getFeedUrl(int position)
		{
			_rssManager.MoveTo (position);
			return _rssManager.Current.Url;
		}

		public String getFeedName(int position)
		{
			_rssManager.MoveTo (position);
			return _rssManager.Current.Name;
		}


	}
}

