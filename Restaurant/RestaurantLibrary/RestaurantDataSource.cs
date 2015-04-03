using System;
using System.Collections.Generic;
using Parse;

namespace RestaurantLibrary
{
	public sealed class RestaurantDataSource
	{
		int _count;
		int _currentIndex;
		List<Restaurant> _restaurants;

		static readonly RestaurantDataSource _dataSourceInstance = new RestaurantDataSource();
		public static RestaurantDataSource Instance
		{
			get
			{
				return _dataSourceInstance;
			}
		}

		public int Count
		{
			get
			{ 
				_count = _restaurants.Count;
				return _count;
			}
			private set
			{
				_count = value;
			}
		}

		public Restaurant Current
		{
			get
			{
				return _restaurants [_currentIndex];
			}
		}

		private RestaurantDataSource ()
		{
			_restaurants = new List<Restaurant> ();
		}

		public void MoveTo(int position)
		{
			_currentIndex = position;
		}

		public void SetRestaurants(List<Restaurant> rests)
		{
			_restaurants = rests;
		}

		public Restaurant getRestaurantById(int restaurantId)
		{
			return _restaurants.Find (e => e.Id == restaurantId);
		}

	}
}

