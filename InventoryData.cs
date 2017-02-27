using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rdc_checkoverbooking
{
	public class InventoryData
	{
		public DateTime Date { get; set; }
		public int InUse { get; set; }
		public int Available { get; set; }
		public int Balance
		{
			get { return Available - InUse; }
		}

		public InventoryData(DateTime date, int inuse, int available)
		{
			Date = date;
			InUse = inuse;
			Available = available;
		}
	}
}
