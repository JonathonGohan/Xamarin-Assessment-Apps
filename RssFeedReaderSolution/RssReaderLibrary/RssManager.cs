using System;
using System.Collections.Generic;
using Android.Content;
using Android.Database;
using Android.Util;
using System.Threading.Tasks;

namespace RssReaderLibrary
{
	public class RssManager
	{
		private List<RssFeed> rssFeeds;
		private RssDataSource rssDataSource;
		private Context context;

		private int _currentIndex;

		public int CurrentIndex 
		{
			get
			{
				return _currentIndex;
			}
			set 
			{
				if (value >= 0 && value < rssFeeds.Count)
					_currentIndex = value;
				else
					throw new IndexOutOfRangeException ("The index : " + value + " is invalid ");
			}
		}

		public int rssCount 
		{
			get 
			{
				return rssFeeds.Count;
			}
		}

		public RssFeed Current
		{
			get
			{
				return rssFeeds [CurrentIndex];
			}
		}

		public RssManager (Context myContext)
		{

			context = myContext;
			rssFeeds = new List<RssFeed> ();
			rssDataSource = new RssDataSource (myContext);
		}

		public void populateRssFeeds ()
		{
			ICursor rssCursorFeeds = rssDataSource.getAllRssFeeds ();
			if (rssCursorFeeds != null && rssCursorFeeds.Count > 0) 
			{
				rssCursorFeeds.MoveToFirst ();
				while (!rssCursorFeeds.IsAfterLast) 
				{
					RssFeed rssClass = new RssFeed ();
					rssClass.Name = rssCursorFeeds.GetString (rssCursorFeeds.GetColumnIndex (RssSqlLiteHelperClass.RSS_FEED_NAME));
					rssClass.Url = rssCursorFeeds.GetString (rssCursorFeeds.GetColumnIndex (RssSqlLiteHelperClass.RSS_FEED_URL));
					rssFeeds.Add (rssClass);
					rssCursorFeeds.MoveToNext ();
				}
				rssCursorFeeds.Close ();
			}
		}

		public void MoveTo(int position)
		{
			CurrentIndex = position;
		}

		public int addNewRssFeed(RssFeed rssFeed)
		{
			int result = rssDataSource.addRssFeed (rssFeed);
			if (result > 0)
				rssFeeds.Add (rssFeed);
			return result;
		}

		public async Task<IEnumerable<RssFeedDetails>> getFeedDetailsAsync(int position)
		{
			RssFeedReceiver rssRecevier = new RssFeedReceiver (context);
			if (rssRecevier.isOnline ()) {
				return await Task.Run (() => rssRecevier.getFeedData (rssFeeds [position].Url));
			} else {
				throw new Exception ("Internet Connection unavailable. Kindly check and try again");
			}
		}
	}
}

