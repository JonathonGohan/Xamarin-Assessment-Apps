using System;
using System.Collections;
using Android.Content;
using Android.Database;
using System.Collections.Generic;

namespace EventLibrary
{

	public class HallDetails
	{
		public String HallName{ get; set; }
		public String Amount{ get; set; }
		public String OpenHours{ get; set; }
		public String Landmark{ get; set; }
		public String Phone{ get; set; }
	}

	public class EventClassManager
	{
		public static String[] Location = { "Chennai" , "Bangalore" , "Chandigarh"};
		public static String[][] Hall = {
			new String[]{ "Chennai 1", "Chennai 2", "Chennai 3" },
			new String[]{ "Bangalore 1", "Bangalore 2" },
			new String[]{"Chandigarh 1","Chandigarh 2",	"Chandigarh 3"}
		};

		public static Dictionary<String,HallDetails> halls = new Dictionary<string, HallDetails> ();

		private ArrayList events;
		private int currentIndex;
		private String UserName;

		EventsDataSource dataSource;

		public EventClassManager(Context context , String username)
		{
			this.UserName = username;
			events = initEvents(context );

			halls.Add ("Chennai 1", new HallDetails () {
				HallName = "Chennai 1",
				Amount = "Rs.10,000 / hr",
				OpenHours = " 9 AM to 9 PM",
				Landmark = "near Rlway Station",
				Phone = "1234567890"
			});

			halls.Add ("Chennai 2", new HallDetails () {
				HallName = "Chennai 2",
				Amount = "Rs.20,000 / hr",
				OpenHours = " 9 AM to 9 PM",
				Landmark = "near Airport",
				Phone = "09876543521"
			});

			halls.Add ("Chennai 3", new HallDetails () {
				HallName = "Chennai 3",
				Amount = "Rs.30,000 / hr",
				OpenHours = " 9 AM to 9 PM",
				Landmark = "near Bus Stand",
				Phone = "0123456789"
			});

			halls.Add ("Bangalore 1", new HallDetails () {
				HallName = "Bangalore 1",
				Amount = "Rs.30,000 / hr",
				OpenHours = " 11 AM to 11 PM",
				Landmark = "new Temple",
				Phone = "0123456789"
			});

			halls.Add ("Bangalore 2", new HallDetails () {
				HallName = "Bangalore 2",
				Amount = "Rs.50,000 / hr",
				OpenHours = " 10 AM to 10 PM",
				Landmark = "near Bus Stand",
				Phone = "0123456789"
			});

			halls.Add ("Chandigarh 1", new HallDetails () {
				HallName = "Chandigarh 1",
				Amount = "Rs.8,000 / hr",
				OpenHours = " 11 AM to 11 PM",
				Landmark = "new Temple",
				Phone = "0123456789"
			});

			halls.Add ("Chandigarh 2", new HallDetails () {
				HallName = "Chandigarh 2",
				Amount = "Rs.30,000 / hr",
				OpenHours = " 11 AM to 11 PM",
				Landmark = "",
				Phone = "0123456789"
			});

			halls.Add ("Chandigarh 3", new HallDetails () {
				HallName = "Chandigarh 3",
				Amount = "Rs.50,000 / hr",
				OpenHours = " 2 PM to 11 PM",
				Landmark = "new irport",
				Phone = "0987654321"
			});
		}

		private ArrayList initEvents(Context context )
		{
			events = new ArrayList ();

			dataSource = new EventsDataSource (context);
			dataSource.open ();
			ICursor cursor = dataSource.getAllEvents (UserName);
			EventClass eventClass;

			if (cursor != null && cursor.Count > 0) 
			{
				cursor.MoveToFirst ();
				while (!cursor.IsAfterLast ) 
				{
					eventClass = new EventClass ();
					eventClass.EventId = cursor.GetInt(cursor.GetColumnIndex(EventSqlLiteHelper.EventIdColumnName));
					eventClass.Username = cursor.GetString (cursor.GetColumnIndex(EventSqlLiteHelper.UserName));
					eventClass.EventName = cursor.GetString (cursor.GetColumnIndex(EventSqlLiteHelper.EventColumnName));
					eventClass.Location = cursor.GetString (cursor.GetColumnIndex(EventSqlLiteHelper.LocationColumnName));
					eventClass.Date = cursor.GetString (cursor.GetColumnIndex(EventSqlLiteHelper.DateColumnName));
					eventClass.Time = cursor.GetString (cursor.GetColumnIndex(EventSqlLiteHelper.TimeColumnName));
					eventClass.hall = cursor.GetString (cursor.GetColumnIndex (EventSqlLiteHelper.HallColumnName));

					events.Add (eventClass);
					cursor.MoveToNext ();
				}
				cursor.Close ();
			}
			return events;
		}

		public void MoveFirst()
		{
			currentIndex = 0;
		}

		public void MoveNext()
		{
			if (canMoveNext)
				currentIndex++;
		}

		public void MovePrev()
		{
			if (canMovePrev)
				currentIndex--;
		}

		public void MoveLast()
		{
			currentIndex = events.Count - 1;
		}

		public EventClass Current
		{
			get { return (EventClass)events[currentIndex]; }
		}

		public int length
		{
			get { return events.Count; }
		}

		public void moveTo(int position)
		{
			if (position >= 0 && position <= events.Count - 1)
				currentIndex = position;
			else
				throw new IndexOutOfRangeException( position + " position is invalid");
		}

		public bool canMovePrev
		{
			get { return currentIndex > 0; }
		}

		public bool canMoveNext
		{
			get { return currentIndex < events.Count - 1; }
		}

		public bool insertEvent(EventClass eventClass)
		{
			bool result = false;
			eventClass.Username = UserName;
			int resultId = dataSource.insertEvent (eventClass) ;
			if ( resultId > 0) 
			{
				eventClass.EventId = resultId;
				events.Add (eventClass);
				result = true;
			}
			return result;
		}

		public bool deleteEvent(int position)
		{
			bool result = false;
			if (dataSource.deleteEvent ((EventClass)events [position]) == true) 
			{
				events.RemoveAt (position);
				result = true;
			}
			return result;
		}

		public bool updateEvent(int position , EventClass eventClass)
		{
			bool result = false;
			EventClass oldEventClass = (EventClass)events [position];
			eventClass.EventId = oldEventClass.EventId;
			eventClass.Username = oldEventClass.Username;
			if (dataSource.updateEvent (eventClass) == true) 
			{
				events [position] = eventClass;
				result = true;
			}
			return result;
		}
	}
}

