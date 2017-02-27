using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rdc_checkoverbooking
{
	public class AccommodationData
	{
		public int ID { get; set; }
		public string Description { get; set; }
		public Dictionary<DateTime, InventoryData> Inventory { get; }

		public AccommodationData(int id, string description)
		{
			ID = id;
			Description = description;
			Inventory = new Dictionary<DateTime, InventoryData>();
		}

		public void IncrementUseness(DateTime date, int increment)
		{
			if (Inventory.ContainsKey(date))
				Inventory[date].InUse += increment;
			else
				Inventory.Add(date, new InventoryData(date, increment, 0));
		}

		public void IncrementAvailability(DateTime date, int increment)
		{
			if (Inventory.ContainsKey(date))
				Inventory[date].Available += increment;
			else
				Inventory.Add(date, new InventoryData(date, 0, increment));
		}
	}
}
