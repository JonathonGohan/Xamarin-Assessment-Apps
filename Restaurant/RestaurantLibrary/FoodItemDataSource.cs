using System;
using System.Collections.Generic;

namespace RestaurantLibrary
{
	public sealed class FoodItemDataSource
	{
		List<FoodItem> _foodItems;
		List<FoodItemType> _foodItemTypes;
		OrderDetails _currentOrderDetails;

		int _currentFoodItemTypeId = -1;

		static readonly FoodItemDataSource _dataSourceInstance = new FoodItemDataSource();

		#region Properties

		public static FoodItemDataSource Instance
		{
			get
			{
				return _dataSourceInstance;
			}
		}

		public OrderDetails CurrentOrderDetails
		{
			get
			{
				return _currentOrderDetails;
			}
		}

		#endregion

		private FoodItemDataSource ()
		{
			_foodItems = new List<FoodItem> ();
			_foodItemTypes = new List<FoodItemType> ();
			_currentOrderDetails = new OrderDetails ();
			_currentOrderDetails.foodItems = new List<FoodItem> ();
		}

		public void MoveToFoodType(int selectedTypeIndex)
		{
			_currentFoodItemTypeId = _foodItemTypes[selectedTypeIndex].Id;
		}

		public void SetFoodItemTypes(List<FoodItemType> fooditemTypes)
		{
			_foodItemTypes = fooditemTypes;
		}

		public void SetFoodItems(List<FoodItem> fooditems)
		{
			_foodItems = fooditems;
		}

		public FoodItem getFoodItem(int foodItemId)
		{
			return _foodItems.Find (e => e.Id == foodItemId);
		}

		public String getFoodItemTypeByTypeId(int foodTypeId)
		{
			return _foodItemTypes.Find (e => e.Id == foodTypeId).Type;
		}

		public List<FoodItem> getFoodItems(int foodTypeId)
		{
			return _foodItems.FindAll (e => e.TypeId == foodTypeId);
		}

		public void updateBookingDetails(int itemId, int count)
		{
			FoodItem baseitem = getFoodItem (itemId);
			FoodItem bookingItem = _currentOrderDetails.foodItems.Find (e => e.Id == itemId);
			if ( bookingItem != null) 
			{
				_currentOrderDetails.Cost -= (bookingItem.Cost * bookingItem.OrderCount);
				_currentOrderDetails.foodItems.Find (e => e.Id == itemId).OrderCount = count;
				_currentOrderDetails.Cost += (baseitem.Cost * count);
				if (count == 0)
					_currentOrderDetails.foodItems.RemoveAll (e => e.Id == itemId);
			}
			else 
			{
				_currentOrderDetails.Cost += (baseitem.Cost * count);
				baseitem.OrderCount = count;
				_currentOrderDetails.foodItems.Add (baseitem);
			}
		}

		public void resetData()
		{
			_currentOrderDetails = new OrderDetails ();
		}
	}
}

