using System;
using Android.Webkit;

namespace RssReader
{
	public class RssWebViewClient : WebViewClient
	{
		public RssWebViewClient ()
		{
		}

		public override bool ShouldOverrideUrlLoading (WebView view, string url)
		{
			view.LoadUrl (url);
			return true;
		}
	}
}

