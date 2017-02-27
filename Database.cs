using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace rdc_checkoverbooking
{
	public class Database
	{
		public static DataTable GetBookings()
		{
			StringBuilder sql = new StringBuilder();
			sql.AppendLine("SELECT H.CodHotel AS HotelID, H.Nome AS HotelName, SH.CodSlip AS SlpiID, SH.CodSlipHotel AS SlipHotelID,");
			sql.AppendLine("SH.Inicio AS CheckIn, SH.Fim AS CheckOut, C.Nome AS City, HTA.CodHotelTipoAcomodacao AS AccommodationID,");
			sql.AppendLine("TA.Nome AS Accommodation, COUNT(DISTINCT SA.NumeroApto) AS Rooms");
			sql.AppendLine("FROM Slips AS S");
			sql.AppendLine("INNER JOIN Slips_Hoteis AS SH ON SH.CodSlip = S.CodSlip");
			sql.AppendLine("INNER JOIN Hoteis AS H ON H.codhotel = SH.CodHotel");
			sql.AppendLine("INNER JOIN Cidades AS C ON C.CodCidade = H.CodCidade");
			sql.AppendLine("INNER JOIN Slips_Apartamentos AS SA ON SA.CodSlipHotel = SH.CodSlipHotel");
			sql.AppendLine("INNER JOIN Hoteis_TiposAcomodacao AS HTA ON HTA.CodHotelTipoAcomodacao = SA.CodHotelTipoAcomodacao");
			sql.AppendLine("INNER JOIN TiposAcomodacao AS TA ON TA.CodTipoAcomodacao = HTA.CodTipoAcomodacao");
			sql.AppendLine("WHERE SH.Inicio >= GETDATE()");
			//sql.AppendLine("AND H.CodHotel = 328");
			sql.AppendLine("AND SA.CodStatusSlip in (3)");
			sql.AppendLine("GROUP BY H.CodHotel, H.Nome, SH.CodSlip, SH.CodSlipHotel, SH.Inicio, SH.Fim, C.Nome, HTA. CodHotelTipoAcomodacao, TA.Nome");
			sql.AppendLine("ORDER BY H.Nome, SH.Inicio, SH.CodSlip");

			SqlCommand cmd = new SqlCommand(sql.ToString());

			return GetDataTable(cmd);
		}

		public static DataTable GetHotels()
		{
			StringBuilder sql = new StringBuilder();
			sql.AppendLine("SELECT DISTINCT H.CodHotel AS HotelID, H.Nome AS HotelName, C.Nome AS City");
			sql.AppendLine("FROM Slips AS S");
			sql.AppendLine("INNER JOIN Slips_Hoteis AS SH ON SH.CodSlip = S.CodSlip");
			sql.AppendLine("INNER JOIN Hoteis AS H ON H.CodHotel = SH.CodHotel");
			sql.AppendLine("INNER JOIN Slips_Apartamentos AS SA ON SA.CodSlipHotel = SH.CodSlipHotel");
			sql.AppendLine("INNER JOIN Cidades AS C ON C.CodCidade = H.CodCidade");
			sql.AppendLine("WHERE SH.Inicio >= GETDATE()");
			//sql.AppendLine("AND H.CodHotel = 328");
			sql.AppendLine("AND SA.CodStatusSlip IN (3)"); // CONF BLOQUEIO, VOUCHER, PAGA
			sql.AppendLine("ORDER BY H.Nome, H.CodHotel"); // CONF BLOQUEIO, VOUCHER, PAGA

			SqlCommand cmd = new SqlCommand(sql.ToString());
			return GetDataTable(cmd);
		}

		public static DataTable GetAvailability(int hoteID, DateTime fromDate, DateTime toDate)
		{
			StringBuilder sql = new StringBuilder();
			sql.AppendLine("SELECT H.CodHotel AS HotelID, H.Nome AS HotelName, HP.Inicio AS Start, HP.Fim AS [End], ");
			sql.AppendLine("   HTA.CodHotelTipoAcomodacao AS AccommodationID, TA.Nome AS Accommodation, HB.QuantidadeBloqueada AS [Availability]");
			sql.AppendLine("FROM Hoteis AS H");
			sql.AppendLine("INNER JOIN Hoteis_Periodos AS HP ON HP.CodHotel = H.CodHotel");
			sql.AppendLine("INNER JOIN Hoteis_Bloqueio AS HB ON HB.CodHotelPeriodo = HP.CodHotelPeriodo");
			sql.AppendLine("INNER JOIN Hoteis_TiposAcomodacao AS HTA ON HTA.CodHotelTipoAcomodacao = HB.CodHotelTipoAcomodacao");
			sql.AppendLine("INNER JOIN TiposAcomodacao AS TA ON TA.CodTipoAcomodacao = HTA.CodTipoAcomodacao");
			sql.AppendLine("WHERE HP.Fim >= @FromDate");
			sql.AppendLine("AND HP.Inicio <= @ToDate");
			sql.AppendLine("AND H.CodHotel = @CodHotel");
			sql.AppendLine("ORDER BY H.Nome, HP.Inicio, TA.Nome");

			SqlCommand cmd = new SqlCommand(sql.ToString());
			cmd.Parameters.Add(new SqlParameter("@FromDate", fromDate));
			cmd.Parameters.Add(new SqlParameter("@ToDate", toDate));
			cmd.Parameters.Add(new SqlParameter("@CodHotel", hoteID));

			return GetDataTable(cmd);
		}

		private static DataTable GetDataTable(SqlCommand cmd)
		{
			SqlConnection conn = new SqlConnection(global::rdc_checkoverbooking.Properties.Settings.Default.ConnectionString);
			cmd.Connection = conn;
			SqlDataAdapter da = new SqlDataAdapter(cmd);
			DataTable dt = new DataTable();
			conn.Open();
			da.Fill(dt);
			conn.Close();
			da.Dispose();
			conn.Dispose();
			return dt;
		}
	}
}
