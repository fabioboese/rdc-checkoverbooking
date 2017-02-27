-- Hotels
select 
	distinct h.codhotel, nome
	--top 100
	--h.*,
	--sa.*
from slips as s
	inner join Slips_Hoteis as sh on sh.CodSlip = s.codslip
	inner join Hoteis as h on h.codhotel = sh.codhotel
	inner join Slips_Apartamentos as sa on sa.CodSlipHotel = sh.CodSlipHotel
where sh.inicio >= getdate()
  and sa.codstatusslip in (3, 5, 6) -- CONF BLOQUEIO, VOUCHER, PAGA



-- Bookings
SELECT 
	H.CodHotel,
	H.Nome AS Name,
	SH.CodSlip,
	SH.CodSlipHotel,
	SH.Inicio AS CheckIn,
	SH.Fim AS CheckOut,
	C.Nome AS City,
	HTA.CodHotelTipoAcomodacao AS AccommodationID,
	TA.Nome AS Accommodation,
	COUNT(DISTINCT SA.NumeroApto) AS Rooms
FROM Slips AS S
	INNER JOIN Slips_Hoteis AS SH ON SH.CodSlip = S.codslip
	INNER JOIN Hoteis AS H ON H.codhotel = SH.codhotel
	INNER JOIN Cidades AS C ON C.CodCidade = H.CodCidade
	INNER JOIN Slips_Apartamentos AS SA ON SA.CodSlipHotel = SH.CodSlipHotel
	INNER JOIN Hoteis_TiposAcomodacao AS HTA ON HTA.CodHotelTipoAcomodacao = SA.CodHotelTipoAcomodacao
	INNER JOIN TiposAcomodacao AS TA ON TA.CodTipoAcomodacao = HTA.CodTipoAcomodacao
WHERE SH.Inicio >= GETDATE()
  AND SA.CodStatusSlip in (3, 5, 6) -- CONF BLOQUEIO, VOUCHER, PAGA
GROUP BY H.CodHotel, H.Nome, SH.CodSlip, SH.CodSlipHotel, SH.Inicio, SH.Fim, C.Nome, TA.Nome
ORDER BY H.Nome, SH.Inicio, SH.CodSlip


-- Availability

SELECT H.CodHotel AS HotelID, H.Nome AS Name, HP.Inicio AS Start, HP.Fim AS [End], HTA.CodHotelTipoAcomodacao, TA.Nome AS Accommodation, HB.QuantidadeBloqueada AS [Availability]
FROM Hoteis AS H
	INNER JOIN Hoteis_Periodos AS HP ON HP.CodHotel = H.CodHotel
	INNER JOIN Hoteis_Bloqueio AS HB ON HB.CodHotelPeriodo = HP.CodHotelPeriodo
	INNER JOIN Hoteis_TiposAcomodacao AS HTA ON HTA.CodHotelTipoAcomodacao = HB.CodHotelTipoAcomodacao
	INNER JOIN TiposAcomodacao AS TA ON TA.CodTipoAcomodacao = HTA.CodTipoAcomodacao
WHERE HP.Fim >= '20170301'
  AND HP.Inicio <= '20170525'
  AND H.CodHotel = 328
ORDER BY 2, 3, 5

