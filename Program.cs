using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace rdc_checkoverbooking
{
	class Program
	{
		static void Main(string[] args)
		{
			
			// Busca todos os hotéis que tem alguma reserva com checkin maior ou igual a hoje
			Console.WriteLine("Getting Hotels...");
			Dictionary<int, HotelData> hotel = new Dictionary<int, HotelData>();
			foreach (DataRow rowHotel in Database.GetHotels().Rows)
				hotel.Add((int)rowHotel["HotelID"], new HotelData((int)rowHotel["HotelID"], (string)rowHotel["HotelName"], (string)rowHotel["City"]));

			// Busca todas as reservas no status CONF BLOQUEIO e soma diariamente a quantidade de quartos por hotel
			Console.WriteLine("Getting Bookings...");
			foreach (DataRow rowBooking in Database.GetBookings().Rows)
			{
				int hotelID = (int)rowBooking["HotelID"];
				int rooms = (int)rowBooking["Rooms"];
				int accommodationID = (int)rowBooking["AccommodationID"];
				string accommodationDescription = (string)rowBooking["Accommodation"];
				DateTime checkIn = (DateTime)rowBooking["CheckIn"];
				DateTime checkOut = (DateTime)rowBooking["CheckOut"];

				DateTime iDate = checkIn;
				do
				{
					hotel[hotelID].GetAccommodation(accommodationID, accommodationDescription).IncrementUseness(iDate, rooms);
					iDate = iDate.AddDays(1);
				} while (iDate < checkOut);
			}

			// Para cada hotel, verifica os períodos que compreendem todas as reservas cadastradas e 
			// atualiza a quantidade diária de bloqueios disponíveis para todos estes períodos
			Console.WriteLine("Getting Availabilities...");
			foreach (HotelData hd in hotel.Values)
			{
				DateTime? firstBooking = hd.GetFirstBooking();
				DateTime? lastBooking = hd.GetLastBooking();
				if (firstBooking.HasValue && lastBooking.HasValue)
					foreach (DataRow rowAvailability in Database.GetAvailability(hd.Code, firstBooking.Value, lastBooking.Value).Rows)
					{
						if (hotel.ContainsKey(hd.Code))
						{
							int availability = (int)rowAvailability["Availability"];
							int accommodationID = (int)rowAvailability["AccommodationID"];
							string accommodationDescription = (string)rowAvailability["Accommodation"];
							DateTime start = (DateTime)rowAvailability["Start"];
							DateTime end = (DateTime)rowAvailability["End"];

							DateTime iDate = start;
							do
							{
								hotel[hd.Code].GetAccommodation(accommodationID, accommodationDescription).IncrementAvailability(iDate, availability);
								iDate = iDate.AddDays(1);
							} while (iDate <= end);
						}
					}
			}

			Console.WriteLine("Starting Analysis...");
			foreach (var h in hotel.Values)
			{
				bool err = false;
				foreach (var acc in h.Accommodation.Values)

					foreach (var inv in acc.Inventory.Values)
						if (inv.Balance < 0)
						{
							if (!err)
							{
								err = true;
								Console.ForegroundColor = ConsoleColor.Red;
								Console.WriteLine(string.Format("ERR {0}-{1} ({2})", h.Code, h.Name, h.City));
							}
							Console.ForegroundColor = (inv.Available == 0 ? ConsoleColor.DarkRed : ConsoleColor.Red);
							Console.WriteLine(string.Format("    - {0} ({1}-{2}): BL={3} RSV={4} SLD={5}", inv.Date.ToShortDateString(), acc.ID, acc.Description,  inv.Available, inv.InUse, inv.Balance));
						}
				if (!err)
				{
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine(string.Format("OK  {0}-{1} ({2})", h.Code, h.Name, h.City));
				}
				
			}
			Console.WriteLine("Done!");
			//Console.ReadLine();

		}
	}
}
