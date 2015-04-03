using System;
using Android.Widget;
using EventLibrary;
using Android.Content;
using Android.Views;

namespace EventManager
{
	public class EventManagerAdapter : BaseAdapter<EventClass>
	{
		Context context;
		int layoutResourceId;
		EventClassManager eventClassManager;

		public override int Count 
		{
			get { return eventClassManager.length; }

		}

		public override EventClass this [int index] 
		{
			get 
			{ 
				eventClassManager.moveTo (index);
				return eventClassManager.Current;
			}
		}

		public EventManagerAdapter (Context ctx, int layoutResId, EventClassManager evntManager)
		{
			context = ctx;
			layoutResourceId = layoutResId;
			eventClassManager = evntManager;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			View view = convertView;

			if (view == null) 
			{
				LayoutInflater inflater = context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				view = inflater.Inflate (layoutResourceId, null);
			}

			TextView textView = view.FindViewById<TextView> (Resource.Id.drawerText);
			textView.Text = this [position].EventName;
			return view;
		}
	}
}

