using System;
using System.Collections.Generic;

namespace RssReaderLibrary
{
	public  class RssFeed
	{
		public String Name{ get; set; }
		public String Url{ get; set; }
		public List<RssFeedDetails> feedDetails = new List<RssFeedDetails>();
	}

	public  class RssFeedDetails
	{
		public String Title{ get; set; }
		public String Desc{ get; set; }
		public String Url{ get; set; }
	}
}

