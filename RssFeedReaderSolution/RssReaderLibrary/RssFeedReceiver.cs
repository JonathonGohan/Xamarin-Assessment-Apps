using System;
using Android.App;
using Android.Net;
using Android.Content;
using System.Net;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace RssReaderLibrary
{
	public class RssFeedReceiver
	{
		Context context;
		NetworkInfo netInfo;
		List<RssFeedDetails> feedDetails;

		public RssFeedReceiver (Context context)
		{
			this.context = context;
			feedDetails = new List<RssFeedDetails> ();
		}

		public bool isOnline()
		{
			var cm = (ConnectivityManager)context.GetSystemService (Context.ConnectivityService);
			netInfo = cm.ActiveNetworkInfo;
			if (netInfo != null && netInfo.IsConnectedOrConnecting)
				return true;
			return false;
		}

		public List<RssFeedDetails> getFeedData(String url)
		{
			if (isOnline ()) {
				try {
					WebRequest webRequest = WebRequest.Create (url);
					WebResponse webResponse = webRequest.GetResponse ();
					Stream stream = webResponse.GetResponseStream ();
					XmlDocument xmlDocument = new XmlDocument ();
					xmlDocument.Load (stream);
					XmlNamespaceManager nsmgr = new XmlNamespaceManager (xmlDocument.NameTable);
					nsmgr.AddNamespace ("dc", xmlDocument.DocumentElement.GetNamespaceOfPrefix ("dc"));
					nsmgr.AddNamespace ("content", xmlDocument.DocumentElement.GetNamespaceOfPrefix ("content"));
					XmlNodeList itemNodes = xmlDocument.SelectNodes ("rss/channel/item");

					for (int i = 0; i < itemNodes.Count; i++) {
						RssFeedDetails details = new RssFeedDetails ();

						if (itemNodes [i].SelectSingleNode ("title") != null) {
							details.Title = itemNodes [i].SelectSingleNode ("title").InnerText;
						}
						if (itemNodes [i].SelectSingleNode ("link") != null) {
							details.Url = itemNodes [i].SelectSingleNode ("link").InnerText;
						}
						if (itemNodes [i].SelectSingleNode ("description") != null) {
							details.Desc = itemNodes [i].SelectSingleNode ("description").InnerText;
						}
						feedDetails.Add (details);
					}
					return feedDetails;
				} catch (Exception) {
					throw new Exception ("Error while retreiving feed data");
				}
			} else {
				throw new Exception ("No Internet Connection !!!!! Kindly check.");
			}
		}
	}
}

