using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rdc_checkoverbooking
{
	public class HotelData
	{
		public int Code { get; set; }
		public string Name { get; set; }
		public string City { get; set; }
		
		public Dictionary<int, AccommodationData> Accommodation { get; }

		public HotelData(int code, string name, string city)
		{
			Accommodation = new Dictionary<int, AccommodationData>();
			Code = code;
			Name = name;
			City = city;
		}

		public AccommodationData GetAccommodation(int id, string description)
		{
			if (Accommodation.ContainsKey(id))
				return Accommodation[id];
			else
			{
				var acc = new AccommodationData(id, description);
				Accommodation.Add(id, acc);
				return acc;
			}
		}

		public DateTime? GetFirstBooking()
		{
			DateTime? ret = null;
			foreach (AccommodationData acc in Accommodation.Values)
			{
				DateTime? min = acc.Inventory.Keys.Count() > 0 ? (DateTime?)acc.Inventory.Keys.Min() : null;
				if (min.HasValue && (!ret.HasValue || min < ret.Value))
					ret = min;
			}
			return ret;
		}

		public DateTime? GetLastBooking()
		{
			DateTime? ret = null;
			foreach (AccommodationData acc in Accommodation.Values)
			{
				DateTime? max = acc.Inventory.Keys.Count() > 0 ? (DateTime?)acc.Inventory.Keys.Max() : null;
				if (max.HasValue && (!ret.HasValue || max > ret.Value))
					ret = max;
			}
			return ret;
		}
	}
}
